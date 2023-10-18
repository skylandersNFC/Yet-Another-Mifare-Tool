using YetAnotherMifareTool.Core;

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
                    var firmware = await _toyFactory.GetFirmware();
                    if (string.IsNullOrEmpty(firmware))
                    {
                        Log("CardReader not found!");
                        return;
                    }
                    else
                    {
                        Log(firmware);
                    }

                    var blockZero = await _toyFactory.GetBlockZero();
                    if (blockZero == null)
                    {
                        Log("Failed to read block zero!");
                        return;
                    }

                    var blockZeroEquals = _toyToWrite.BlockZero.SequenceEqual(blockZero);
                    var writeBlockZero = cb_writeBlockZero.Checked;

                    if (!blockZeroEquals && !writeBlockZero)
                    {
                        _toyToWrite.Data = ToyGenerator.Generate(blockZero, _toyToWrite.Id, _toyToWrite.IdExt);
                    }
                    else
                    {
                        _toyToWrite.Data = _toyToWrite.Data
                            .WithCalculatingKeys()
                            .WithUnlockedAccessConditions();
                    }

                    await _toyFactory.Write(_toyToWrite.Data, !blockZeroEquals && writeBlockZero);

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
    }
}