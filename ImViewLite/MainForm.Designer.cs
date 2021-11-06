
namespace ImViewLite
{
    partial class MainForm
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
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Directories", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Files", System.Windows.Forms.HorizontalAlignment.Left);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.scMainContainer = new System.Windows.Forms.SplitContainer();
            this.listView1 = new ImViewLite.Controls.LISTVIEW();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageDisplay1 = new ImViewLite.Controls.ImageDisplay();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslItemOfItems = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslFilePath = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSPACE1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslFileSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslImageSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.tseMainToolstrip = new ImViewLite.Controls.ToolStripEx();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButton4 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tscbInterpolationMode = new System.Windows.Forms.ToolStripComboBox();
            this.tsbSettings = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.scMainContainer)).BeginInit();
            this.scMainContainer.Panel1.SuspendLayout();
            this.scMainContainer.Panel2.SuspendLayout();
            this.scMainContainer.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tseMainToolstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(0, 25);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(800, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.InputTextbox_TextChanged);
            // 
            // scMainContainer
            // 
            this.scMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scMainContainer.Location = new System.Drawing.Point(0, 45);
            this.scMainContainer.Margin = new System.Windows.Forms.Padding(0);
            this.scMainContainer.Name = "scMainContainer";
            // 
            // scMainContainer.Panel1
            // 
            this.scMainContainer.Panel1.Controls.Add(this.listView1);
            // 
            // scMainContainer.Panel2
            // 
            this.scMainContainer.Panel2.Controls.Add(this.imageDisplay1);
            this.scMainContainer.Size = new System.Drawing.Size(800, 385);
            this.scMainContainer.SplitterDistance = 483;
            this.scMainContainer.TabIndex = 2;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chSize,
            this.chPath});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewGroup3.Header = "Directories";
            listViewGroup3.Name = "Directories";
            listViewGroup4.Header = "Files";
            listViewGroup4.Name = "Files";
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(483, 385);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // chName
            // 
            this.chName.Tag = "";
            this.chName.Text = "Name";
            this.chName.Width = 250;
            // 
            // chSize
            // 
            this.chSize.Text = "Size";
            this.chSize.Width = 80;
            // 
            // chPath
            // 
            this.chPath.Text = "Path";
            this.chPath.Width = 145;
            // 
            // imageDisplay1
            // 
            this.imageDisplay1.CellColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.imageDisplay1.CellColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.imageDisplay1.CellScale = 2F;
            this.imageDisplay1.CellSize = 32;
            this.imageDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageDisplay1.DrawMode = ImViewLite.Controls.DrawMode.FitImage;
            this.imageDisplay1.Image = null;
            this.imageDisplay1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.imageDisplay1.Location = new System.Drawing.Point(0, 0);
            this.imageDisplay1.Name = "imageDisplay1";
            this.imageDisplay1.Size = new System.Drawing.Size(313, 385);
            this.imageDisplay1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslItemOfItems,
            this.tsslFilePath,
            this.tsslSPACE1,
            this.tsslFileSize,
            this.tsslImageSize});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslItemOfItems
            // 
            this.tsslItemOfItems.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslItemOfItems.Name = "tsslItemOfItems";
            this.tsslItemOfItems.Size = new System.Drawing.Size(4, 17);
            this.tsslItemOfItems.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslFilePath
            // 
            this.tsslFilePath.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslFilePath.Name = "tsslFilePath";
            this.tsslFilePath.Size = new System.Drawing.Size(4, 17);
            this.tsslFilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslSPACE1
            // 
            this.tsslSPACE1.Name = "tsslSPACE1";
            this.tsslSPACE1.Size = new System.Drawing.Size(773, 17);
            this.tsslSPACE1.Spring = true;
            // 
            // tsslFileSize
            // 
            this.tsslFileSize.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslFileSize.Name = "tsslFileSize";
            this.tsslFileSize.Size = new System.Drawing.Size(4, 17);
            // 
            // tsslImageSize
            // 
            this.tsslImageSize.Name = "tsslImageSize";
            this.tsslImageSize.Size = new System.Drawing.Size(0, 17);
            // 
            // tseMainToolstrip
            // 
            this.tseMainToolstrip.ClickThrough = false;
            this.tseMainToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton3,
            this.toolStripDropDownButton4,
            this.tsbSettings});
            this.tseMainToolstrip.Location = new System.Drawing.Point(0, 0);
            this.tseMainToolstrip.Name = "tseMainToolstrip";
            this.tseMainToolstrip.Size = new System.Drawing.Size(800, 25);
            this.tseMainToolstrip.TabIndex = 0;
            this.tseMainToolstrip.Text = "toolStripEx1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.newWindowToolStripMenuItem});
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // newWindowToolStripMenuItem
            // 
            this.newWindowToolStripMenuItem.Name = "newWindowToolStripMenuItem";
            this.newWindowToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.newWindowToolStripMenuItem.Text = "New Window";
            this.newWindowToolStripMenuItem.Click += new System.EventHandler(this.newWindowToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(40, 22);
            this.toolStripDropDownButton2.Text = "Edit";
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(55, 22);
            this.toolStripDropDownButton3.Text = "Search";
            // 
            // toolStripDropDownButton4
            // 
            this.toolStripDropDownButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.tscbInterpolationMode});
            this.toolStripDropDownButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton4.Name = "toolStripDropDownButton4";
            this.toolStripDropDownButton4.Size = new System.Drawing.Size(45, 22);
            this.toolStripDropDownButton4.Text = "View";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // tscbInterpolationMode
            // 
            this.tscbInterpolationMode.Name = "tscbInterpolationMode";
            this.tscbInterpolationMode.Size = new System.Drawing.Size(121, 23);
            // 
            // tsbSettings
            // 
            this.tsbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSettings.Name = "tsbSettings";
            this.tsbSettings.Size = new System.Drawing.Size(53, 22);
            this.tsbSettings.Text = "Settings";
            this.tsbSettings.Click += new System.EventHandler(this.tsbSettings_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.scMainContainer);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tseMainToolstrip);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.scMainContainer.Panel1.ResumeLayout(false);
            this.scMainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMainContainer)).EndInit();
            this.scMainContainer.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tseMainToolstrip.ResumeLayout(false);
            this.tseMainToolstrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImViewLite.Controls.ToolStripEx tseMainToolstrip;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.SplitContainer scMainContainer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private Controls.ImageDisplay imageDisplay1;
        private ImViewLite.Controls.LISTVIEW listView1;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chSize;
        private System.Windows.Forms.ColumnHeader chPath;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton4;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tsslFileSize;
        private System.Windows.Forms.ToolStripStatusLabel tsslFilePath;
        private System.Windows.Forms.ToolStripStatusLabel tsslSPACE1;
        private System.Windows.Forms.ToolStripStatusLabel tsslImageSize;
        private System.Windows.Forms.ToolStripStatusLabel tsslItemOfItems;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripComboBox tscbInterpolationMode;
        private System.Windows.Forms.ToolStripButton tsbSettings;
    }
}

