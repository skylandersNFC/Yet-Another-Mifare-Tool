using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibnfcSharp;
using LibnfcSharp.Mifare;
using LibnfcSharp.Mifare.Enums;
using LibnfcSharp.Mifare.Extensions;
using YetAnotherMifareTool.Builder;
using YetAnotherMifareTool.Extensions;
using YetAnotherMifareTool.Models;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool
{
    public class MainForm : Form
    {
        private DumpFile _dumpFile;
        private IContainer components;

        private TabPage write_tab;
        private TabControl tabControl;
        private TextBox tb_dumpSelect;
        private Button btn_dumpSelect;
        private RichTextBox rtb_logWrite;
        private Button btn_dumpWrite;
        private Button btn_clearLogWrite;

        private TabPage identify_tab;
        private RichTextBox rtb_logIdentify;
        private Button btn_clearLogIdentify;
        private Button btn_identifyCard;

        private TabPage read_tab;
        private Button btn_clearLogRead;
        private Button btn_readCard;
        private RichTextBox rtb_logRead;

        private TabPage reset_tab;
        private Button btn_clearLogReset;
        private Button btn_resetCard;
        private RichTextBox rtb_logReset;

        private TabPage setuid_tab;
        private Button btn_clearLogSetUid;
        private Button btn_setUid;
        private RichTextBox rtb_logSetUid;

        public MainForm()
        {
            InitializeComponent();
			
		    // Update the version
            Text += " v1.4.0 [YAMT]";
			
			this.Shown += MainForm_Shown;
			
        }
		
		// Intro Message
		private void MainForm_Shown(object sender, EventArgs e)
		{
			string welcomeMessage =
				Environment.NewLine + Environment.NewLine +
				"Yet Another Mifare Tool (YAMT) supports ACR122U and PN532 V2.0 NFC devices."
				+ Environment.NewLine +
				"You can write to Mifare S50 1K Gen1 UID Unlocked, Gen1 UID Locked and Gen2 CUID tags."
				+ Environment.NewLine +
				"This software is intended for Skylanders games only, as it processes data specific to the games."
				+ Environment.NewLine;

			Log(rtb_logIdentify, LogLevel.Debug, welcomeMessage);

		}

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

            write_tab = new TabPage();
            btn_clearLogWrite = new Button();
            rtb_logWrite = new RichTextBox();
            btn_dumpWrite = new Button();
            btn_dumpSelect = new Button();
            tb_dumpSelect = new TextBox();
            tabControl = new TabControl();
            identify_tab = new TabPage();
            btn_clearLogIdentify = new Button();
            btn_identifyCard = new Button();
            rtb_logIdentify = new RichTextBox();
            read_tab = new TabPage();
            btn_clearLogRead = new Button();
            btn_readCard = new Button();
            rtb_logRead = new RichTextBox();
            reset_tab = new TabPage();
            btn_clearLogReset = new Button();
            btn_resetCard = new Button();
            rtb_logReset = new RichTextBox();
            setuid_tab = new TabPage();
            btn_clearLogSetUid = new Button();
            btn_setUid = new Button();
            rtb_logSetUid = new RichTextBox();

            // ----- Layout suspend -----
            write_tab.SuspendLayout();
            identify_tab.SuspendLayout();
            read_tab.SuspendLayout();
            reset_tab.SuspendLayout();
            setuid_tab.SuspendLayout();
            SuspendLayout();

            // ----- Write Tab -----
            write_tab.Controls.Add(btn_clearLogWrite);
            write_tab.Controls.Add(rtb_logWrite);
            write_tab.Controls.Add(btn_dumpWrite);
            write_tab.Controls.Add(btn_dumpSelect);
            write_tab.Controls.Add(tb_dumpSelect);
            write_tab.Location = new Point(4, 29);
            write_tab.Name = "write_tab";
            write_tab.Padding = new Padding(3);
            write_tab.Size = new Size(1019, 745);
            write_tab.TabIndex = 1;
            write_tab.Text = "Write";
            write_tab.UseVisualStyleBackColor = true;

            btn_clearLogWrite.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_clearLogWrite.Location = new Point(885, 63);
            btn_clearLogWrite.Name = "btn_clearLogWrite";
            btn_clearLogWrite.Size = new Size(120, 29);
            btn_clearLogWrite.TabIndex = 6;
            btn_clearLogWrite.Text = "CLEAR LOG";
            btn_clearLogWrite.UseVisualStyleBackColor = true;
            btn_clearLogWrite.Click += btn_clearLogWrite_Click;

            rtb_logWrite.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtb_logWrite.Location = new Point(17, 103);
            rtb_logWrite.Name = "rtb_logWrite";
            rtb_logWrite.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtb_logWrite.Size = new Size(988, 627);
            rtb_logWrite.TabIndex = 5;
            rtb_logWrite.TabStop = false;
            rtb_logWrite.Text = "";

            btn_dumpWrite.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_dumpWrite.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point);
            btn_dumpWrite.Location = new Point(17, 63);
            btn_dumpWrite.Name = "btn_dumpWrite";
            btn_dumpWrite.Size = new Size(862, 29);
            btn_dumpWrite.TabIndex = 4;
            btn_dumpWrite.Text = "WRITE DUMP";
            btn_dumpWrite.UseVisualStyleBackColor = true;
            btn_dumpWrite.Click += btn_dumpWrite_Click;

            btn_dumpSelect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btn_dumpSelect.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point);
            btn_dumpSelect.Location = new Point(885, 24);
            btn_dumpSelect.Name = "btn_dumpSelect";
            btn_dumpSelect.Size = new Size(120, 29);
            btn_dumpSelect.TabIndex = 3;
            btn_dumpSelect.Text = "SELECT DUMP";
            btn_dumpSelect.UseVisualStyleBackColor = true;
            btn_dumpSelect.Click += btn_dumpSelect_Click;

            tb_dumpSelect.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_dumpSelect.Location = new Point(17, 25);
            tb_dumpSelect.Name = "tb_dumpSelect";
            tb_dumpSelect.ReadOnly = true;
            tb_dumpSelect.Size = new Size(862, 27);
            tb_dumpSelect.TabIndex = 0;
            tb_dumpSelect.TabStop = false;

            // ----- TabControl -----
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(identify_tab);
            tabControl.Controls.Add(read_tab);
            tabControl.Controls.Add(write_tab);
            tabControl.Controls.Add(reset_tab);
            tabControl.Controls.Add(setuid_tab);
            tabControl.Location = new Point(3, 3);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1027, 778);
            tabControl.TabIndex = 0;

            // ----- Identify Tab -----
            identify_tab.Controls.Add(btn_clearLogIdentify);
            identify_tab.Controls.Add(btn_identifyCard);
            identify_tab.Controls.Add(rtb_logIdentify);
            identify_tab.Location = new Point(4, 29);
            identify_tab.Name = "identify_tab";
            identify_tab.Padding = new Padding(3);
            identify_tab.Size = new Size(1019, 745);
            identify_tab.TabIndex = 2;
            identify_tab.Text = "Identify";
            identify_tab.UseVisualStyleBackColor = true;

            btn_clearLogIdentify.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_clearLogIdentify.Location = new Point(885, 24);
            btn_clearLogIdentify.Name = "btn_clearLogIdentify";
            btn_clearLogIdentify.Size = new Size(120, 29);
            btn_clearLogIdentify.TabIndex = 8;
            btn_clearLogIdentify.Text = "CLEAR LOG";
            btn_clearLogIdentify.UseVisualStyleBackColor = true;
            btn_clearLogIdentify.Click += btn_clearLogIdentify_Click;

            btn_identifyCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_identifyCard.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point);
            btn_identifyCard.Location = new Point(17, 24);
            btn_identifyCard.Name = "btn_identifyCard";
            btn_identifyCard.Size = new Size(862, 29);
            btn_identifyCard.TabIndex = 7;
            btn_identifyCard.Text = "IDENTIFY TAG";
            btn_identifyCard.UseVisualStyleBackColor = true;
            btn_identifyCard.Click += btn_identifyCard_Click;

            rtb_logIdentify.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtb_logIdentify.Location = new Point(17, 63);
            rtb_logIdentify.Name = "rtb_logIdentify";
            rtb_logIdentify.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtb_logIdentify.Size = new Size(988, 667);
            rtb_logIdentify.TabIndex = 5;
            rtb_logIdentify.TabStop = false;
            rtb_logIdentify.Text = "";

            // ----- Read Tab -----
            read_tab.Controls.Add(btn_clearLogRead);
            read_tab.Controls.Add(btn_readCard);
            read_tab.Controls.Add(rtb_logRead);
            read_tab.Location = new Point(4, 29);
            read_tab.Name = "read_tab";
            read_tab.Size = new Size(1019, 745);
            read_tab.TabIndex = 3;
            read_tab.Text = "Read";
            read_tab.UseVisualStyleBackColor = true;

            btn_clearLogRead.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_clearLogRead.Location = new Point(885, 24);
            btn_clearLogRead.Name = "btn_clearLogRead";
            btn_clearLogRead.Size = new Size(120, 29);
            btn_clearLogRead.TabIndex = 11;
            btn_clearLogRead.Text = "CLEAR LOG";
            btn_clearLogRead.UseVisualStyleBackColor = true;
            btn_clearLogRead.Click += btn_clearLogRead_Click;

            btn_readCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_readCard.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point);
            btn_readCard.Location = new Point(17, 24);
            btn_readCard.Name = "btn_readCard";
            btn_readCard.Size = new Size(862, 29);
            btn_readCard.TabIndex = 10;
            btn_readCard.Text = "READ TAG";
            btn_readCard.UseVisualStyleBackColor = true;
            btn_readCard.Click += btn_readCard_Click;

            rtb_logRead.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtb_logRead.Location = new Point(17, 63);
            rtb_logRead.Name = "rtb_logRead";
            rtb_logRead.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtb_logRead.Size = new Size(988, 667);
            rtb_logRead.TabIndex = 9;
            rtb_logRead.TabStop = false;
            rtb_logRead.Text = "";

            // ----- Reset Tab -----
            reset_tab.Controls.Add(btn_clearLogReset);
            reset_tab.Controls.Add(btn_resetCard);
            reset_tab.Controls.Add(rtb_logReset);
            reset_tab.Location = new Point(4, 29);
            reset_tab.Name = "reset_tab";
            reset_tab.Size = new Size(1019, 745);
            reset_tab.TabIndex = 4;
            reset_tab.Text = "Reset";
            reset_tab.UseVisualStyleBackColor = true;

            btn_clearLogReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_clearLogReset.Location = new Point(885, 24);
            btn_clearLogReset.Name = "btn_clearLogReset";
            btn_clearLogReset.Size = new Size(120, 29);
            btn_clearLogReset.TabIndex = 11;
            btn_clearLogReset.Text = "CLEAR LOG";
            btn_clearLogReset.UseVisualStyleBackColor = true;
            btn_clearLogReset.Click += btn_clearLogReset_Click;

            btn_resetCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_resetCard.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point);
            btn_resetCard.Location = new Point(17, 24);
            btn_resetCard.Name = "btn_resetCard";
            btn_resetCard.Size = new Size(862, 29);
            btn_resetCard.TabIndex = 10;
            btn_resetCard.Text = "RESET TAG";
            btn_resetCard.UseVisualStyleBackColor = true;
            btn_resetCard.Click += btn_resetCard_Click;

            rtb_logReset.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtb_logReset.Location = new Point(17, 63);
            rtb_logReset.Name = "rtb_logReset";
            rtb_logReset.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtb_logReset.Size = new Size(988, 667);
            rtb_logReset.TabIndex = 9;
            rtb_logReset.TabStop = false;
            rtb_logReset.Text = "";

            // ----- Set Uid Tab -----
            setuid_tab.Controls.Add(btn_clearLogSetUid);
            setuid_tab.Controls.Add(btn_setUid);
            setuid_tab.Controls.Add(rtb_logSetUid);
            setuid_tab.Location = new Point(4, 29);
            setuid_tab.Name = "setuid_tab";
            setuid_tab.Size = new Size(1019, 745);
            setuid_tab.TabIndex = 5;
            setuid_tab.Text = "Format";
            setuid_tab.UseVisualStyleBackColor = true;

            btn_clearLogSetUid.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_clearLogSetUid.Location = new Point(885, 24);
            btn_clearLogSetUid.Name = "btn_clearLogSetUid";
            btn_clearLogSetUid.Size = new Size(120, 29);
            btn_clearLogSetUid.TabIndex = 14;
            btn_clearLogSetUid.Text = "CLEAR LOG";
            btn_clearLogSetUid.UseVisualStyleBackColor = true;
            btn_clearLogSetUid.Click += btn_clearLogSetUid_Click;

            btn_setUid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_setUid.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point);
            btn_setUid.Location = new Point(17, 24);
            btn_setUid.Name = "btn_setUid";
            btn_setUid.Size = new Size(862, 29);
            btn_setUid.TabIndex = 13;
            btn_setUid.Text = "FORMAT TAG";
            btn_setUid.UseVisualStyleBackColor = true;
            btn_setUid.Click += btn_setUid_Click;

            rtb_logSetUid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtb_logSetUid.Location = new Point(17, 63);
            rtb_logSetUid.Name = "rtb_logSetUid";
            rtb_logSetUid.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtb_logSetUid.Size = new Size(988, 667);
            rtb_logSetUid.TabIndex = 12;
            rtb_logSetUid.TabStop = false;
            rtb_logSetUid.Text = "";

            // ----- Form -----
            AutoScaleDimensions = new SizeF(8f, 20f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1033, 784);
            Controls.Add(tabControl);
            try
            {
                var ico = resources.GetObject("$this.Icon") as Icon;
                if (ico != null) Icon = ico;
            }
            catch { /* ignore missing icon at design time */ }
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Yet Another Mifare Tool";

            // ----- Layout resume -----
            write_tab.ResumeLayout(false);
            write_tab.PerformLayout();
            identify_tab.ResumeLayout(false);
            read_tab.ResumeLayout(false);
            reset_tab.ResumeLayout(false);
            setuid_tab.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ---------- Messages ----------
		private static class Messages
		{
			public const string NoNfcTagFound = "No tag detected on the NFC reader.";
			public const string UnableToReadManufacturerBlock = "This is not a MIFARE S50 1K tag. Only this type works with Skylanders.";
		}

        // ---------- Logging ----------
        private void Log(RichTextBox rtb, LogLevel level, string message)
        {
            if (rtb.InvokeRequired)
            {
                rtb.Invoke((MethodInvoker)(() => Log(rtb, level, message)));
                return;
            }

			Color color = level switch
			{
				LogLevel.Trace => Color.Gray,
				LogLevel.Debug => Color.Blue,
				LogLevel.Information => Color.Green,
				LogLevel.Warning => Color.Orange,
				LogLevel.Error => Color.Red,
				LogLevel.Critical => Color.DarkRed,
				_ => rtb.ForeColor
			};


            rtb.SelectionStart = rtb.Text.Length;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = color;
            rtb.AppendText($"{DateTime.Now} - {message}{Environment.NewLine}");
            rtb.SelectionColor = rtb.ForeColor;
            rtb.ScrollToCaret();
        }

        // ---------- Identify Tab ----------
        private void btn_identifyCard_Click(object sender, EventArgs e) => Task.Run(IdentifyDump);
        private void btn_clearLogIdentify_Click(object sender, EventArgs e) => rtb_logIdentify.Clear();

        private void IdentifyDump()
        {
            try
            {
                using var nfcContext = new NfcContext();
                using var device = nfcContext.OpenDevice();
                var mifare = new MifareClassic(device);

                mifare.RegisterLogCallback((lvl, msg) => Log(rtb_logIdentify, lvl, msg));
				
                mifare.InitialDevice();

                if (!mifare.SelectCard())
                {
                    Log(rtb_logIdentify, LogLevel.Error, Messages.NoNfcTagFound);
                    return;
                }

                mifare.RegisterKeyAProviderCallback(Crypto.CalculateKeyA);
                mifare.IdentifyMagicCardType();

                if (!mifare.ReadManufacturerInfo(out var manufacturerInfo))
                {
                    Log(rtb_logIdentify, LogLevel.Error, Messages.UnableToReadManufacturerBlock);
                    return;
                }

                bool sector0Unlocked = mifare.HasUnlockedAccessConditions(0, out var ac0);
                Log(rtb_logIdentify, LogLevel.Information,
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    $"UID: {Convert.ToHexString(mifare.Uid)}{Environment.NewLine}" +
                    $"BCC: {Convert.ToHexString(new[] { manufacturerInfo.Bcc })}{Environment.NewLine}" +
                    $"SAK: {Convert.ToHexString(new[] { mifare.Sak })} ({Convert.ToHexString(new[] { manufacturerInfo.Sak })}){Environment.NewLine}" +
                    $"ATQA: {Convert.ToHexString(mifare.Atqa)} ({Convert.ToHexString(manufacturerInfo.Atqa)}){Environment.NewLine}" +
                    $"Type: {mifare.MagicCardType.ToDescription()}{Environment.NewLine}" +
                    $"AC Sector 0: {Convert.ToHexString(ac0)} ({(sector0Unlocked ? "Unlocked" : "Locked")}){Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Log(rtb_logIdentify, LogLevel.Error, $"{ex.GetType()}: {ex.Message}");
            }
        }

        // ---------- Read Tab ----------
        private void btn_readCard_Click(object sender, EventArgs e) => Task.Run(ReadCard);
        private void btn_clearLogRead_Click(object sender, EventArgs e) => rtb_logRead.Clear();

        private void ReadCard()
        {
            try
            {
                using var nfcContext = new NfcContext();
                using var device = nfcContext.OpenDevice();
                var mifare = new MifareClassic(device);

                mifare.RegisterLogCallback((lvl, msg) => Log(rtb_logRead, lvl, msg));
                mifare.InitialDevice();

                if (!mifare.SelectCard())
                {
                    Log(rtb_logRead, LogLevel.Error, Messages.NoNfcTagFound);
                    return;
                }

                mifare.RegisterKeyAProviderCallback(Crypto.CalculateKeyA);
                mifare.IdentifyMagicCardType();

                if (!mifare.ReadManufacturerInfo(out var manufacturerInfo))
                {
                    Log(rtb_logRead, LogLevel.Error, Messages.UnableToReadManufacturerBlock);
                    return;
                }

                bool sector0Unlocked = mifare.HasUnlockedAccessConditions(0, out var ac0);
                Log(rtb_logRead, LogLevel.Information,
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    $"UID: {Convert.ToHexString(mifare.Uid)}{Environment.NewLine}" +
                    $"BCC: {Convert.ToHexString(new[] { manufacturerInfo.Bcc })}{Environment.NewLine}" +
                    $"SAK: {Convert.ToHexString(new[] { mifare.Sak })} ({Convert.ToHexString(new[] { manufacturerInfo.Sak })}){Environment.NewLine}" +
                    $"ATQA: {Convert.ToHexString(mifare.Atqa)} ({Convert.ToHexString(manufacturerInfo.Atqa)}){Environment.NewLine}" +
                    $"Type: {mifare.MagicCardType.ToDescription()}{Environment.NewLine}" +
                    $"AC Sector 0: {Convert.ToHexString(ac0)} ({(sector0Unlocked ? "Unlocked" : "Locked")}){Environment.NewLine}");

                if (mifare.ReadCard(out var cardData))
                {
                    cardData = cardData.UnlockAccessConditions();

                    string dumpsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dumps");
                    if (!Directory.Exists(dumpsDir)) Directory.CreateDirectory(dumpsDir);

                    string filePath = Path.Combine(dumpsDir,
                        $"{Convert.ToHexString(mifare.Uid)}_{DateTime.Now:yyyyMMdd_HHmmss}.dump");

                    File.WriteAllBytes(filePath, cardData);
                    Log(rtb_logRead, LogLevel.Information, "Dump saved to " + filePath);
                }
            }
            catch (Exception ex)
            {
                Log(rtb_logRead, LogLevel.Error, $"{ex.GetType()}: {ex.Message}");
            }
        }

        // ---------- Write Tab ----------
        private void btn_dumpSelect_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "SELECT DUMP",
                DefaultExt = "dump",
                Filter = "Dumps (*.dump)|*.dump|All files (*.*)|*.*",
                CheckFileExists = true,
                CheckPathExists = true
            };
			
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					_dumpFile = new DumpFile(ofd.FileName);

					tb_dumpSelect.Text = _dumpFile.FilePath;
					Log(rtb_logWrite, LogLevel.Information, "Valid dump loaded: " + _dumpFile.FilePath);
				}
				catch
				{
					_dumpFile = null;
					tb_dumpSelect.Text = string.Empty;
					Log(rtb_logWrite, LogLevel.Error, "Failed to load Skylanders dump. The file may be corrupted.");
				}
			}
			
        }

        private void btn_dumpWrite_Click(object sender, EventArgs e)
        {
            if (_dumpFile == null)
            {
                Log(rtb_logWrite, LogLevel.Error, "No Skylanders dump selected.");
                return;
            }
            if (_dumpFile.IsValid)
                Task.Run(WriteDump);
        }

        private void btn_clearLogWrite_Click(object sender, EventArgs e) => rtb_logWrite.Clear();

        private void WriteDump()
        {
            try
            {
                using var nfcContext = new NfcContext();
                using var device = nfcContext.OpenDevice();
                var mifare = new MifareClassic(device);

                mifare.RegisterLogCallback((lvl, msg) => Log(rtb_logWrite, lvl, msg));
                mifare.InitialDevice();

                if (!mifare.SelectCard())
                {
                    Log(rtb_logWrite, LogLevel.Error, Messages.NoNfcTagFound);
                    return;
                }

                mifare.RegisterKeyAProviderCallback(Crypto.CalculateKeyA);
                mifare.IdentifyMagicCardType();

                if (!mifare.ReadManufacturerInfo(out var manufacturerInfo))
                {
                    Log(rtb_logWrite, LogLevel.Error, Messages.UnableToReadManufacturerBlock);
                    return;
                }

                bool sector0Unlocked = mifare.HasUnlockedAccessConditions(0, out var ac0);
                Log(rtb_logWrite, LogLevel.Information,
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    $"UID: {Convert.ToHexString(mifare.Uid)}{Environment.NewLine}" +
                    $"BCC: {Convert.ToHexString(new[] { manufacturerInfo.Bcc })}{Environment.NewLine}" +
                    $"SAK: {Convert.ToHexString(new[] { mifare.Sak })} ({Convert.ToHexString(new[] { manufacturerInfo.Sak })}){Environment.NewLine}" +
                    $"ATQA: {Convert.ToHexString(mifare.Atqa)} ({Convert.ToHexString(manufacturerInfo.Atqa)}){Environment.NewLine}" +
                    $"Type: {mifare.MagicCardType.ToDescription()}{Environment.NewLine}" +
                    $"AC Sector 0: {Convert.ToHexString(ac0)} ({(sector0Unlocked ? "Unlocked" : "Locked")}){Environment.NewLine}");

                bool sameManufacturer = manufacturerInfo.RawData.SequenceEqual(_dumpFile.ManufacturerBlock);
                Toy toy;

                switch (mifare.MagicCardType)
                {
                    case MifareMagicCardType.GEN_1:
                        if (sector0Unlocked)
                        {
                            if (sameManufacturer)
                            {
                                toy = new ToyBuilder()
                                    .WithRecalculatedKeys()
                                    .WithUnlockedAccessConditions()
                                    .BuildFromDumpFile(_dumpFile);
                            }
                            else if (_dumpFile.HasSignature)
                            {
                                Log(rtb_logWrite, LogLevel.Error,
                                    "Error: Unable to write an Imaginator to a Gen1 UID locked tag. Use an older Skylander or a Gen1A UID unlocked tag.");
                                return;
                            }
                            else
                            {
                                toy = new ToyBuilder()
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
                            if (!sameManufacturer)
                            {
                                Log(rtb_logWrite, LogLevel.Error,
                                    "Error: UID locked and Sector 0 locked (by access conditions). Use another NFC tag.");
                                return;
                            }
                            toy = new ToyBuilder()
                                .WithRecalculatedKeys()
                                .WithUnlockedAccessConditions()
                                .BuildFromDumpFile(_dumpFile);
                        }
                        break;

                    case MifareMagicCardType.GEN_1A:
                    case MifareMagicCardType.GEN_1B:
                        toy = new ToyBuilder()
                            .WithRecalculatedKeys()
                            .WithUnlockedAccessConditions()
                            .BuildFromDumpFile(_dumpFile);
                        break;

                    case MifareMagicCardType.GEN_2:
                        if (sector0Unlocked)
                        {
                            if (sameManufacturer || mifare.HasUnlockedAccessConditions(1, out _))
                            {
                                toy = new ToyBuilder()
                                    .WithRecalculatedKeys()
                                    .WithUnlockedAccessConditions()
                                    .BuildFromDumpFile(_dumpFile);
                            }
                            else if (!_dumpFile.HasSignature)
                            {
                                toy = new ToyBuilder()
                                    .WithManufacturerBlock(manufacturerInfo.RawData)
                                    .WithId(_dumpFile.Id)
                                    .WithVariant(_dumpFile.Variant)
                                    .WithRecalculatedKeys()
                                    .WithUnlockedAccessConditions()
                                    .BuildFromScratch();
                            }
                            else
                            {
                                Log(rtb_logWrite, LogLevel.Error,
                                    "Error: Unable to write an Imaginator to a previously used Gen2 CUID tag. Please use an older Skylander or another unused Gen2 tag.");
                                return;
                            }
                        }
                        else
                        {
                            Log(rtb_logWrite, LogLevel.Error,
                                "Error: Sector 0 locked (by access conditions). Use another NFC tag.");
                            return;
                        }
                        break;

                    default:
                        toy = null;
                        break;
                }

                if (toy != null)
                {
                    mifare.WriteDump(toy.Data);
                }
                else
                {
                    Log(rtb_logWrite, LogLevel.Error, "Error: Unknown NFC tag type. Use another one.");
                }
            }
            catch (Exception ex)
            {
                Log(rtb_logWrite, LogLevel.Error, $"{ex.GetType()}: {ex.Message}");
            }
        }

        // ---------- Reset Tab ----------
        private void btn_resetCard_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Resetting the NFC tag will erase all character progress.\nThe Skylander on it will be reset to level one.\nDo you wish to continue?",
                "Confirm Tag Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Task.Run(ResetCard);
            }
        }

        private void btn_clearLogReset_Click(object sender, EventArgs e) => rtb_logReset.Clear();

        private void ResetCard()
        {
            try
            {
                using var nfcContext = new NfcContext();
                using var device = nfcContext.OpenDevice();
                var mifare = new MifareClassic(device);

                mifare.RegisterLogCallback((lvl, msg) => Log(rtb_logReset, lvl, msg));
                mifare.InitialDevice();

                if (!mifare.SelectCard())
                {
                    Log(rtb_logReset, LogLevel.Error, Messages.NoNfcTagFound);
                    return;
                }

                mifare.RegisterKeyAProviderCallback(Crypto.CalculateKeyA);
                mifare.IdentifyMagicCardType();

                if (!mifare.ReadManufacturerInfo(out var manufacturerInfo))
                {
                    Log(rtb_logReset, LogLevel.Error, Messages.UnableToReadManufacturerBlock);
                    return;
                }

                bool sector0Unlocked = mifare.HasUnlockedAccessConditions(0, out var ac0);
                Log(rtb_logReset, LogLevel.Information,
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    $"UID: {Convert.ToHexString(mifare.Uid)}{Environment.NewLine}" +
                    $"BCC: {Convert.ToHexString(new[] { manufacturerInfo.Bcc })}{Environment.NewLine}" +
                    $"SAK: {Convert.ToHexString(new[] { mifare.Sak })} ({Convert.ToHexString(new[] { manufacturerInfo.Sak })}){Environment.NewLine}" +
                    $"ATQA: {Convert.ToHexString(mifare.Atqa)} ({Convert.ToHexString(manufacturerInfo.Atqa)}){Environment.NewLine}" +
                    $"Type: {mifare.MagicCardType.ToDescription()}{Environment.NewLine}" +
                    $"AC Sector 0: {Convert.ToHexString(ac0)} ({(sector0Unlocked ? "Unlocked" : "Locked")}){Environment.NewLine}");

                mifare.ResetCard(MagicExtension.RESET_BLOCK_IDXS, MifareClassic.EMPTY_BLOCK);
            }
            catch (Exception ex)
            {
                Log(rtb_logReset, LogLevel.Error, $"{ex.GetType()}: {ex.Message}");
            }
        }

        // ---------- Format Tab ----------
        private void btn_setUid_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
				"Factory format the tag and erase everything.\nDo you wish to continue?",
                "Confirm Factory Format", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Task.Run(SetUid);
            }
        }

        private void btn_clearLogSetUid_Click(object sender, EventArgs e) => rtb_logSetUid.Clear();

        private void SetUid()
        {
            try
            {
                using var nfcContext = new NfcContext();
                using var device = nfcContext.OpenDevice();
                var mifare = new MifareClassic(device);

                mifare.RegisterLogCallback((lvl, msg) => Log(rtb_logSetUid, lvl, msg));
                mifare.InitialDevice();

                if (!mifare.SelectCard())
                {
                    Log(rtb_logSetUid, LogLevel.Error, Messages.NoNfcTagFound);
                    return;
                }

                if (mifare.SetUid(null, format: true))
                {
                    Log(rtb_logSetUid, LogLevel.Information, "NFC tag factory formatted.");
                }
                else
                {
                    Log(rtb_logSetUid, LogLevel.Error,
                        $"Error: Failed to factory format the NFC tag. This feature only works for {MifareMagicCardType.GEN_1A.ToDescription()} & {MifareMagicCardType.GEN_1B.ToDescription()} tags!");
                }
            }
            catch (Exception ex)
            {
                Log(rtb_logSetUid, LogLevel.Error, $"{ex.GetType()}: {ex.Message}");
            }
        }
    }
}
