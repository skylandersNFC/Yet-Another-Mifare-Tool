using LibnfcSharp;
using LibnfcSharp.Mifare;
using LibnfcSharp.Mifare.Extensions;
using LibnfcSharp.Mifare.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YetAnotherMifareTool.Builder;
using YetAnotherMifareTool.Models;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool
{
    public partial class Form1 : Form
    {
        private DumpFile _dumpFile;

        public Form1()
        {
            InitializeComponent();

            Text += $" v{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}{ThisAssembly.Git.SemVer.DashLabel} [{ThisAssembly.Git.Commit}]";
        }

        private void WriteDump()
        {
            try
            {
                using (var context = new NfcContext())
                using (var device = context.OpenDevice())
                {
                    var mfc = new MifareClassic(device);
                    mfc.RegisterLogCallback(Log);

                    mfc.InitialDevice();

                    if (!mfc.SelectCard())
                    {
                        Log("No card found!");
                        return;
                    }

                    mfc.RegisterKeyAProviderCallback(Crypto.CalculateKeyA);
                    mfc.IdentifyMagicCardType();

                    ManufacturerInfo manufacturerInfo;
                    if (!mfc.ReadManufacturerInfo(out manufacturerInfo))
                    {
                        Log("Failed to read manufacturer block!");
                        return;
                    }

                    byte[] accessConditions;
                    var hasUnlockedAccessConditionsInSectorZero = mfc.HasUnlockedAccessConditions(0, out accessConditions);

                    Log($$"""


                        UID: {{Convert.ToHexString(mfc.Uid)}}
                        BCC: {{Convert.ToHexString(new[] { manufacturerInfo.Bcc })}}
                        SAK: {{Convert.ToHexString(new[] { mfc.Sak })}} ({{Convert.ToHexString(new[] { manufacturerInfo.Sak })}})
                        ATQA: {{Convert.ToHexString(mfc.Atqa)}} ({{Convert.ToHexString(manufacturerInfo.Atqa)}})
                        ATS: {{(mfc.Ats.Length == 0 ? "-" : Convert.ToHexString(mfc.Ats))}}
                        Type: {{mfc.MagicCardType.ToDescription()}}
                        Access Conditions (Sector 0): {{Convert.ToHexString(accessConditions)}} ({{(hasUnlockedAccessConditionsInSectorZero ? "unlocked" : "locked")}})

                        """);

                    var manufacturerBlockEquals = manufacturerInfo.RawData.SequenceEqual(_dumpFile.ManufacturerBlock);

                    Toy toyToWrite;
                    switch (mfc.MagicCardType)
                    {
                        case LibnfcSharp.Mifare.Enums.MifareMagicCardType.GEN_1:
                            if (hasUnlockedAccessConditionsInSectorZero) // is sector 0 unlocked?
                            {
                                if (manufacturerBlockEquals)
                                {
                                    toyToWrite = new ToyBuilder()
                                        .WithRecalculatedKeys()
                                        .WithUnlockedAccessConditions()
                                        .BuildFromDumpFile(_dumpFile);
                                }
                                else
                                {
                                    toyToWrite = new ToyBuilder()
                                        .WithManufacturerBlock(manufacturerInfo.RawData)
                                        .WithId(_dumpFile.Id)
                                        .WithVariant(_dumpFile.Variant)
                                        .WithRecalculatedKeys()
                                        .WithUnlockedAccessConditions()
                                        .BuildFromScratch();
                                }
                            }
                            else
                            {
                                Log("Error: Uid is locked! Sector 0 is locked (by access conditions)! Use another card...");
                                return;
                            }
                            break;


                        case LibnfcSharp.Mifare.Enums.MifareMagicCardType.GEN_1A:
                        case LibnfcSharp.Mifare.Enums.MifareMagicCardType.GEN_1B:
                            toyToWrite = new ToyBuilder()
                                .WithRecalculatedKeys()
                                .WithUnlockedAccessConditions()
                                .BuildFromDumpFile(_dumpFile);
                            break;

                        case LibnfcSharp.Mifare.Enums.MifareMagicCardType.GEN_2:
                            if (hasUnlockedAccessConditionsInSectorZero) // is sector 0 unlocked?
                            {
                                // same as previous toy (manufacturerBlockEquals) or unused Gen 2 card (unlocked acs in all sectors)
                                if (manufacturerBlockEquals || mfc.HasUnlockedAccessConditions(1, out _))
                                {
                                    toyToWrite = new ToyBuilder()
                                        .WithRecalculatedKeys()
                                        .WithUnlockedAccessConditions()
                                        .BuildFromDumpFile(_dumpFile);
                                }
                                else                         // if sector 1 not unlocked => toy has already been written
                                if (!_dumpFile.HasSignature) // will only work for toys without signature
                                {
                                    toyToWrite = new ToyBuilder()
                                        .WithManufacturerBlock(manufacturerInfo.RawData)
                                        .WithId(_dumpFile.Id)
                                        .WithVariant(_dumpFile.Variant)
                                        .WithRecalculatedKeys()
                                        .WithUnlockedAccessConditions()
                                        .BuildFromScratch();
                                }
                                else
                                {
                                    Log("Error: Unable to write toys with signature to a used Gen2 card. Use a toy without signature or another card...");
                                    return;
                                }
                            }
                            else
                            {
                                Log("Error: Sector 0 is locked (by access conditions). Use another card...");
                                return;
                            }
                            break;

                        default:
                            toyToWrite = null;
                            break;
                    }

                    if (toyToWrite != null)
                    {
                        mfc.WriteDump(toyToWrite.Data);
                    }
                    else
                    {
                        Log("Error: Unknown magic card type. Use another card...");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"{ex.GetType()}: {ex.Message}");
            }
        }

        private void Log(string message)
        {
            if (tb_logWrite.InvokeRequired)
            {
                tb_logWrite.Invoke((MethodInvoker)delegate
                {
                    Log(message);
                });
            }
            else
            {
                tb_logWrite.AppendText($"{DateTime.Now} - {message}{Environment.NewLine}");
            }
        }

        #region UI events

        private void btn_dumpSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select dump...",
                DefaultExt = "dump",
                Filter = "Dumps (*.dump, *.bin)|*.dump;*.bin|All files (*.*)|*.*",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _dumpFile = new DumpFile(ofd.FileName);

                if (_dumpFile.IsValid)
                {
                    tb_dumpSelect.Text = _dumpFile.FilePath;

                    Log($"Loaded valid dump: {_dumpFile.FilePath}");
                }
                else
                {
                    _dumpFile = null;
                    tb_dumpSelect.Text = string.Empty;

                    Log("Dump is not valid!");
                }
            }
        }

        private void btn_dumpWrite_Click(object sender, EventArgs e)
        {
            if (_dumpFile == null)
            {
                Log("No dump selected!");
            }
            else
            if (_dumpFile.IsValid)
            {
                Task.Run(() =>
                {
                    WriteDump();
                });
            }
        }

        private void btn_clearLog_Click(object sender, EventArgs e)
        {
            tb_logWrite.Clear();
        }

        #endregion UI events
    }
}