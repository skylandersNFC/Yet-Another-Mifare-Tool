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
        private ToyFactory _toyFactory;
        private Toy _toyToWrite;

        public Form1()
        {
            InitializeComponent();

            Text += ThisAssembly.Git.Tag.Equals(ThisAssembly.Git.BaseTag) && !string.IsNullOrEmpty(ThisAssembly.Git.Tag)
                ? $" v{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}{ThisAssembly.Git.SemVer.DashLabel}"
                : $" ({ThisAssembly.Git.Commit})";

            _toyFactory = new ToyFactory();
            _toyFactory.OnLogging += (sender, e) => Log(e);
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
                _toyToWrite = new Toy(ofd.FileName);

                if (_toyToWrite.IsDataValid)
                {
                    tb_dumpSelect.Text = _toyToWrite.Name;

                    Log($"Loaded valid dump: {_toyToWrite.Name}");
                }
                else
                {
                    _toyToWrite = null;
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
            if (_toyToWrite == null)
            {
                Log("No dump selected!");
            }
            else
            if (_toyToWrite.IsDataValid)
            {
                Task.Run(async () =>
                {
                    var uid = await _toyFactory.GetUid();
                    if (uid == null)
                    {
                        Log("No Tag found!");
                        return;
                    }

                    var manufacturerBlock = await _toyFactory.ReadManufacturerBlock();
                    if (manufacturerBlock == null)
                    {
                        Log("Failed to read manufacturer block!");
                        return;
                    }

                    var manufacturerBlockEquals = _toyToWrite.ManufacturerBlock.SequenceEqual(manufacturerBlock);
                    var writeManufacturerBlock = cb_writeManufacturerBlock.Checked;
                    byte[] data;

                    if (!manufacturerBlockEquals && !writeManufacturerBlock)
                    {
                        data = ToyGenerator.Generate(manufacturerBlock, _toyToWrite.Id, _toyToWrite.IdExt);
                    }
                    else
                    {
                        data = _toyToWrite.Data
                            .WithRecalculatedKeys()
                            .WithUnlockedAccessConditions();
                    }

                    var keys = Magic.CalculateKeys(uid);

                    await _toyFactory.Write(keys, data, writeManufacturerBlock);

                }).ConfigureAwait(false);
            }
        }

        private void btn_clearLog_Click(object sender, EventArgs e)
        {
            tb_logWrite.Clear();
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
            _toyFactory?.Dispose();
        }

    }
}