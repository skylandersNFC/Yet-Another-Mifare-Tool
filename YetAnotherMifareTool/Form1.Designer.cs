using System.Drawing;
using System.Windows.Forms;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tabWrite = new TabPage();
            btn_clearLog = new Button();
            tb_logWrite = new TextBox();
            btn_dumpWrite = new Button();
            btn_dumpSelect = new Button();
            tb_dumpSelect = new TextBox();
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
            tabWrite.Location = new Point(4, 29);
            tabWrite.Name = "tabWrite";
            tabWrite.Padding = new Padding(3);
            tabWrite.Size = new Size(1019, 745);
            tabWrite.TabIndex = 1;
            tabWrite.Text = "Write";
            tabWrite.UseVisualStyleBackColor = true;
            // 
            // btn_clearLog
            // 
            btn_clearLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_clearLog.Location = new Point(885, 63);
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
            tb_logWrite.Location = new Point(17, 103);
            tb_logWrite.Multiline = true;
            tb_logWrite.Name = "tb_logWrite";
            tb_logWrite.ScrollBars = ScrollBars.Vertical;
            tb_logWrite.Size = new Size(988, 627);
            tb_logWrite.TabIndex = 5;
            tb_logWrite.TabStop = false;
            // 
            // btn_dumpWrite
            // 
            btn_dumpWrite.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_dumpWrite.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btn_dumpWrite.Location = new Point(17, 63);
            btn_dumpWrite.Name = "btn_dumpWrite";
            btn_dumpWrite.Size = new Size(862, 29);
            btn_dumpWrite.TabIndex = 4;
            btn_dumpWrite.Text = "WRITE DUMP";
            btn_dumpWrite.UseVisualStyleBackColor = true;
            btn_dumpWrite.Click += btn_dumpWrite_Click;
            // 
            // btn_dumpSelect
            // 
            btn_dumpSelect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_dumpSelect.Location = new Point(885, 24);
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
            tb_dumpSelect.Size = new Size(862, 27);
            tb_dumpSelect.TabIndex = 0;
            tb_dumpSelect.TabStop = false;
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(tabWrite);
            tabControl.Location = new Point(3, 3);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1027, 778);
            tabControl.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1033, 784);
            Controls.Add(tabControl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "YetAnotherMifareTool";
            tabWrite.ResumeLayout(false);
            tabWrite.PerformLayout();
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabWrite;
        private TabControl tabControl;
        private TextBox tb_dumpSelect;
        private Button btn_dumpSelect;
        private TextBox tb_logWrite;
        private Button btn_dumpWrite;
        private Button btn_clearLog;
    }
}