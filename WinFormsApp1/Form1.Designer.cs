namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            visualStudioToolStripExtender1 = new WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(components);
            toolStripContainer1 = new ToolStripContainer();
            menuStrip1 = new MenuStrip();
            菜单ToolStripMenuItem = new ToolStripMenuItem();
            Menu_SelectWz = new ToolStripMenuItem();
            保存ToolStripMenuItem = new ToolStripMenuItem();
            设置ToolStripMenuItem = new ToolStripMenuItem();
            Menu_Run = new ToolStripMenuItem();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // visualStudioToolStripExtender1
            // 
            visualStudioToolStripExtender1.DefaultRenderer = null;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Size = new Size(800, 425);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(800, 450);
            toolStripContainer1.TabIndex = 0;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(menuStrip1);
            // 
            // menuStrip1
            // 
            menuStrip1.Dock = DockStyle.None;
            menuStrip1.Items.AddRange(new ToolStripItem[] { 菜单ToolStripMenuItem, 设置ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 菜单ToolStripMenuItem
            // 
            菜单ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { Menu_SelectWz, 保存ToolStripMenuItem });
            菜单ToolStripMenuItem.Name = "菜单ToolStripMenuItem";
            菜单ToolStripMenuItem.Size = new Size(44, 21);
            菜单ToolStripMenuItem.Text = "开始";
            // 
            // Menu_SelectWz
            // 
            Menu_SelectWz.Name = "Menu_SelectWz";
            Menu_SelectWz.Size = new Size(180, 22);
            Menu_SelectWz.Text = "选择Quest.wz";
            Menu_SelectWz.Click += Menu_SelectWz_Click;
            // 
            // 保存ToolStripMenuItem
            // 
            保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            保存ToolStripMenuItem.Size = new Size(180, 22);
            保存ToolStripMenuItem.Text = "保存";
            保存ToolStripMenuItem.Click += 保存ToolStripMenuItem_Click;
            // 
            // 设置ToolStripMenuItem
            // 
            设置ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { Menu_Run });
            设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            设置ToolStripMenuItem.Size = new Size(44, 21);
            设置ToolStripMenuItem.Text = "更新";
            // 
            // Menu_Run
            // 
            Menu_Run.Name = "Menu_Run";
            Menu_Run.Size = new Size(124, 22);
            Menu_Run.Text = "执行更新";
            Menu_Run.Click += Menu_Run_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(toolStripContainer1);
            Name = "Form1";
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender visualStudioToolStripExtender1;
        private ToolStripContainer toolStripContainer1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 菜单ToolStripMenuItem;
        private ToolStripMenuItem Menu_SelectWz;
        private ToolStripMenuItem 保存ToolStripMenuItem;
        private ToolStripMenuItem 设置ToolStripMenuItem;
        private ToolStripMenuItem Menu_Run;
    }
}
