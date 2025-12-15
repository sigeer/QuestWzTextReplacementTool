namespace WinFormsApp1
{
    partial class WzVersionInputWin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Combo_Type = new ComboBox();
            Text_Version = new TextBox();
            Btn_Submit = new Button();
            Label_GameVerion = new Label();
            Label_Type = new Label();
            SuspendLayout();
            // 
            // Combo_Type
            // 
            Combo_Type.FormattingEnabled = true;
            Combo_Type.Location = new Point(124, 34);
            Combo_Type.Name = "Combo_Type";
            Combo_Type.Size = new Size(121, 25);
            Combo_Type.TabIndex = 0;
            // 
            // Text_Version
            // 
            Text_Version.Location = new Point(124, 65);
            Text_Version.Name = "Text_Version";
            Text_Version.Size = new Size(121, 23);
            Text_Version.TabIndex = 1;
            // 
            // Btn_Submit
            // 
            Btn_Submit.Location = new Point(327, 147);
            Btn_Submit.Name = "Btn_Submit";
            Btn_Submit.Size = new Size(75, 23);
            Btn_Submit.TabIndex = 2;
            Btn_Submit.Text = "确认";
            Btn_Submit.UseVisualStyleBackColor = true;
            Btn_Submit.Click += Btn_Submit_Click;
            // 
            // Label_GameVerion
            // 
            Label_GameVerion.AutoSize = true;
            Label_GameVerion.Location = new Point(61, 68);
            Label_GameVerion.Name = "Label_GameVerion";
            Label_GameVerion.Size = new Size(32, 17);
            Label_GameVerion.TabIndex = 3;
            Label_GameVerion.Text = "版本";
            // 
            // Label_Type
            // 
            Label_Type.AutoSize = true;
            Label_Type.Location = new Point(61, 37);
            Label_Type.Name = "Label_Type";
            Label_Type.Size = new Size(32, 17);
            Label_Type.TabIndex = 4;
            Label_Type.Text = "类型";
            // 
            // WzVersionInputWin
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(414, 182);
            Controls.Add(Label_Type);
            Controls.Add(Label_GameVerion);
            Controls.Add(Btn_Submit);
            Controls.Add(Text_Version);
            Controls.Add(Combo_Type);
            Name = "WzVersionInputWin";
            Text = "选择版本";
            Load += WzVersionInputWin_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox Combo_Type;
        private TextBox Text_Version;
        private Button Btn_Submit;
        private Label Label_GameVerion;
        private Label Label_Type;
    }
}