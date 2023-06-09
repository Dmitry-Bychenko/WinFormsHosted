namespace WinFormsHosted {
  public partial class MainForm : Form {
    private async Task CoreRun() {
      using var client = Program.Client();
    }

    public MainForm() {
      InitializeComponent();
    }

    private async void rtbRun_Click(object sender, EventArgs e) {
      await CoreRun();
    }
  }
}