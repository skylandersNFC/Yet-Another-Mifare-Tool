using LibnfcSharp;
using LibnfcSharp.Mifare;
using LibnfcSharp.Mifare.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YetAnotherMifareTool.Core;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool
{
    public partial class Form1 : Form
    {
        private DumpFile _dumpFile;
        private ToyReader _toyReader;

        public Form1()
        {
            InitializeComponent();

            Text += $" v{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}{ThisAssembly.Git.SemVer.DashLabel} [{ThisAssembly.Git.Commit}]";

            _toyReader = new ToyReader();
            _toyReader.OnLogging += (sender, e) => Log(e);
        }

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

        private void l_writeManufacturerBlock_Click(object sender, EventArgs e)
        {
            cb_writeManufacturerBlock.Checked = !cb_writeManufacturerBlock.Checked;
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

        private void WriteDump()
        {
            try
            {
                using (var context = new NfcContext())
                using (var device = context.OpenDevice())
                {
                    var mfc = new MifareClassic(device);
                    mfc.InitialDevice();

                    if (mfc.SelectCard())
                    {
                        Log("No Tag found!");
                        return;
                    }

                    mfc.RegisterKeyAProviderCallback((sector, uid) => Crypto.CalculateKeyA(sector, uid));
                    mfc.IdentifyMagicCardType();

                    ManufacturerInfo manufacturerInfo;
                    if (mfc.ReadManufacturerInfo(out manufacturerInfo))
                    {
                        Log("Failed to read manufacturer block!");
                        return;
                    }

                    var hasUnlockedAccessConditions = mfc.HasUnlockedAccessConditions(0, out _);
                    var manufacturerBlockEquals = manufacturerInfo.RawData.SequenceEqual(_dumpFile.ManufacturerBlock);

                    Toy toyToWrite;
                    switch (mfc.MagicCardType)
                    {
                        case LibnfcSharp.Mifare.Enums.MifareMagicCardType.GEN_1:
                            if (hasUnlockedAccessConditions)
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
                            if (hasUnlockedAccessConditions)
                            {
                                toyToWrite = new ToyBuilder()
                                    .WithRecalculatedKeys()
                                    .WithUnlockedAccessConditions()
                                    .BuildFromDumpFile(_dumpFile);
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

                    //mfc.Write(data);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _toyReader?.Dispose();
        }
    }
}