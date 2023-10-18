namespace YetAnotherMifareTool
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tabWrite = new TabPage();
            btn_clearLog = new Button();
            tb_logWrite = new TextBox();
            btn_dumpWrite = new Button();
            btn_dumpSelect = new Button();
            tb_dumpSelect = new TextBox();
            label1 = new Label();
            cb_writeBlockZero = new CheckBox();
            tabControl = new TabControl();
            tabWrite.SuspendLayout();
            tabControl.SuspendLayout();
            SuspendLayout();
            // 
            // tabWrite
            // 
            tabWrite.Controls.Add(btn_clearLog);
            tabWrite.Controls.Add(tb_logWrite);
            tabWrite.Controls.Add(btn_dumpWrite);
            tabWrite.Controls.Add(btn_dumpSelect);
            tabWrite.Controls.Add(tb_dumpSelect);
            tabWrite.Controls.Add(label1);
            tabWrite.Controls.Add(cb_writeBlockZero);
            tabWrite.Location = new Point(4, 29);
            tabWrite.Name = "tabWrite";
            tabWrite.Padding = new Padding(3);
            tabWrite.Size = new Size(634, 390);
            tabWrite.TabIndex = 1;
            tabWrite.Text = "Write";
            tabWrite.UseVisualStyleBackColor = true;
            // 
            // btn_clearLog
            // 
            btn_clearLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_clearLog.Location = new Point(500, 103);
            btn_clearLog.Name = "btn_clearLog";
            btn_clearLog.Size = new Size(120, 29);
            btn_clearLog.TabIndex = 6;
            btn_clearLog.Text = "CLEAR LOG";
            btn_clearLog.UseVisualStyleBackColor = true;
            btn_clearLog.Click += btn_clearLog_Click;
            // 
            // tb_logWrite
            // 
            tb_logWrite.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_logWrite.Location = new Point(17, 150);
            tb_logWrite.Multiline = true;
            tb_logWrite.Name = "tb_logWrite";
            tb_logWrite.ScrollBars = ScrollBars.Vertical;
            tb_logWrite.Size = new Size(603, 232);
            tb_logWrite.TabIndex = 5;
            tb_logWrite.TabStop = false;
            // 
            // btn_dumpWrite
            // 
            btn_dumpWrite.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_dumpWrite.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btn_dumpWrite.Location = new Point(17, 103);
            btn_dumpWrite.Name = "btn_dumpWrite";
            btn_dumpWrite.Size = new Size(477, 29);
            btn_dumpWrite.TabIndex = 4;
            btn_dumpWrite.Text = "WRITE DUMP";
            btn_dumpWrite.UseVisualStyleBackColor = true;
            btn_dumpWrite.Click += btn_dumpWrite_Click;
            // 
            // btn_dumpSelect
            // 
            btn_dumpSelect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_dumpSelect.Location = new Point(500, 24);
            btn_dumpSelect.Name = "btn_dumpSelect";
            btn_dumpSelect.Size = new Size(120, 29);
            btn_dumpSelect.TabIndex = 3;
            btn_dumpSelect.Text = "Select dump...";
            btn_dumpSelect.UseVisualStyleBackColor = true;
            btn_dumpSelect.Click += btn_dumpSelect_Click;
            // 
            // tb_dumpSelect
            // 
            tb_dumpSelect.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_dumpSelect.Location = new Point(17, 25);
            tb_dumpSelect.Name = "tb_dumpSelect";
            tb_dumpSelect.ReadOnly = true;
            tb_dumpSelect.Size = new Size(477, 27);
            tb_dumpSelect.TabIndex = 0;
            tb_dumpSelect.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(331, 70);
            label1.Name = "label1";
            label1.Size = new Size(261, 20);
            label1.TabIndex = 1;
            label1.Text = "Working only with CUID/Gen2 tags!";
            // 
            // cb_writeBlockZero
            // 
            cb_writeBlockZero.AutoSize = true;
            cb_writeBlockZero.Location = new Point(17, 69);
            cb_writeBlockZero.Name = "cb_writeBlockZero";
            cb_writeBlockZero.Size = new Size(320, 24);
            cb_writeBlockZero.TabIndex = 2;
            cb_writeBlockZero.Text = "Enable writing manufacturer block (Block 0)";
            cb_writeBlockZero.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabWrite);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(642, 423);
            tabControl.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(642, 423);
            Controls.Add(tabControl);
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Form1";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Yet Another Mifare Tool";
            tabWrite.ResumeLayout(false);
            tabWrite.PerformLayout();
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabWrite;
        private TabControl tabControl;
        private CheckBox cb_writeBlockZero;
        private TextBox tb_dumpSelect;
        private Label label1;
        private Button btn_dumpSelect;
        private TextBox tb_logWrite;
        private Button btn_dumpWrite;
        private Button btn_clearLog;
    }
}