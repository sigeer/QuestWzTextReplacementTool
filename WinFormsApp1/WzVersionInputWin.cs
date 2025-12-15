using MapleLib.WzLib;

namespace WinFormsApp1
{
    public partial class WzVersionInputWin : Form
    {
        public event EventHandler<WzVersion>? OnSubmit;
        public WzVersionInputWin(bool showGameVersion = true)
        {
            InitializeComponent();

            if (!showGameVersion)
            {
                Label_GameVerion.Visible = false;
                Text_Version.Visible = false;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Btn_Submit.Focus();
        }

        private void Btn_Submit_Click(object sender, EventArgs e)
        {
            this.Close();

            if (!short.TryParse(Text_Version.Text, out var gameVersion))
            {
                MessageBox.Show("游戏版本是数字类型");
                return;
            }

            OnSubmit?.Invoke(this, new WzVersion(Enum.Parse<WzMapleVersion>(Combo_Type.Text), gameVersion));
        }

        private void WzVersionInputWin_Load(object sender, EventArgs e)
        {
            Combo_Type.Items.AddRange(Enum.GetNames<WzMapleVersion>());
            Combo_Type.Text = WzMapleVersion.GMS.ToString();
            Text_Version.Text = "83";
        }
    }

    public record WzVersion(WzMapleVersion Version, short GameVersion);
}
