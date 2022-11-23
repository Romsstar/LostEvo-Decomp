
namespace LostEvoRewrite
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractPAKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractDuskDawnPAKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.decompressAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.importToolStripMenuItem,
            this.compressToolStripMenuItem,
            this.decompressToolStripMenuItem,
            this.compressFolderToolStripMenuItem,
            this.decompressAllToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(800, 30);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractPAKToolStripMenuItem,
            this.extractDuskDawnPAKToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.openToolStripMenuItem.Text = "File";
            // 
            // extractPAKToolStripMenuItem
            // 
            this.extractPAKToolStripMenuItem.Name = "extractPAKToolStripMenuItem";
            this.extractPAKToolStripMenuItem.Size = new System.Drawing.Size(247, 26);
            this.extractPAKToolStripMenuItem.Text = "Extract PAK";
            this.extractPAKToolStripMenuItem.Click += new System.EventHandler(this.extractPAKToolStripMenuItem_Click);
            // 
            // extractDuskDawnPAKToolStripMenuItem
            // 
            this.extractDuskDawnPAKToolStripMenuItem.Name = "extractDuskDawnPAKToolStripMenuItem";
            this.extractDuskDawnPAKToolStripMenuItem.Size = new System.Drawing.Size(247, 26);
            this.extractDuskDawnPAKToolStripMenuItem.Text = "Extract Dusk/Dawn PAK";
            this.extractDuskDawnPAKToolStripMenuItem.Click += new System.EventHandler(this.extractDuskDawnPAKToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(68, 24);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // compressToolStripMenuItem
            // 
            this.compressToolStripMenuItem.Name = "compressToolStripMenuItem";
            this.compressToolStripMenuItem.Size = new System.Drawing.Size(88, 24);
            this.compressToolStripMenuItem.Text = "Compress";
            this.compressToolStripMenuItem.Click += new System.EventHandler(this.compressToolStripMenuItem_Click);
            // 
            // decompressToolStripMenuItem
            // 
            this.decompressToolStripMenuItem.Name = "decompressToolStripMenuItem";
            this.decompressToolStripMenuItem.Size = new System.Drawing.Size(105, 24);
            this.decompressToolStripMenuItem.Text = "Decompress";
            this.decompressToolStripMenuItem.Click += new System.EventHandler(this.decompressToolStripMenuItem_Click);
            // 
            // compressFolderToolStripMenuItem
            // 
            this.compressFolderToolStripMenuItem.Name = "compressFolderToolStripMenuItem";
            this.compressFolderToolStripMenuItem.Size = new System.Drawing.Size(134, 24);
            this.compressFolderToolStripMenuItem.Text = "Compress Folder";
            this.compressFolderToolStripMenuItem.Click += new System.EventHandler(this.compressFolderToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // decompressAllToolStripMenuItem
            // 
            this.decompressAllToolStripMenuItem.Name = "decompressAllToolStripMenuItem";
            this.decompressAllToolStripMenuItem.Size = new System.Drawing.Size(123, 24);
            this.decompressAllToolStripMenuItem.Text = "DecompressAll";
            this.decompressAllToolStripMenuItem.Click += new System.EventHandler(this.decompressAllToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 451);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decompressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractPAKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractDuskDawnPAKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decompressAllToolStripMenuItem;
    }
}

