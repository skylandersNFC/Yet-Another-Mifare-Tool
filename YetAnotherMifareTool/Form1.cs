namespace YetAnotherMifareTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Text += ThisAssembly.Git.Tag.Equals(ThisAssembly.Git.BaseTag) && !string.IsNullOrEmpty(ThisAssembly.Git.Tag)
                ? $" v{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}{ThisAssembly.Git.SemVer.DashLabel}"
                : $" ({ThisAssembly.Git.Commit})";
        }
    }
}