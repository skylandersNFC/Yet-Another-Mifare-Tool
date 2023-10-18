namespace YetAnotherMifareTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Text += $" v{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}{ThisAssembly.Git.SemVer.DashLabel}";
        }
    }
}