namespace WinFormsHosted {
  partial class MainForm {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      rtbRun = new Button();
      rtbMain = new RichTextBox();
      SuspendLayout();
      // 
      // rtbRun
      // 
      resources.ApplyResources(rtbRun, "rtbRun");
      rtbRun.Name = "rtbRun";
      rtbRun.UseVisualStyleBackColor = true;
      rtbRun.Click += rtbRun_Click;
      // 
      // rtbMain
      // 
      resources.ApplyResources(rtbMain, "rtbMain");
      rtbMain.BackColor = SystemColors.Info;
      rtbMain.DetectUrls = false;
      rtbMain.Name = "rtbMain";
      rtbMain.ReadOnly = true;
      // 
      // MainForm
      // 
      resources.ApplyResources(this, "$this");
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(rtbMain);
      Controls.Add(rtbRun);
      Name = "MainForm";
      ResumeLayout(false);
    }

    #endregion

    private Button rtbRun;
    private RichTextBox rtbMain;
  }
}