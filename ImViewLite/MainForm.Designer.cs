
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
            this._FolderWatcher?.Dispose();
            this._LoadImageTimer?.Dispose();
            
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Directories", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Files", System.Windows.Forms.HorizontalAlignment.Left);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.scMainContainer = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslItemOfItems = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslFilePath = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSPACE1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslFileSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslImageSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grayscaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bytesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kilobytesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.megabytesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gigabytesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terabytesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.petabyteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dimensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.widthXHeightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.widthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWithDefaultProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWithToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new ImViewLite.Controls.LISTVIEW();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageDisplay1 = new ImViewLite.Controls.ImageDisplay();
            this.tseMainToolstrip = new ImViewLite.Controls.ToolStripEx();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButton4 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tscbInterpolationMode = new System.Windows.Forms.ToolStripComboBox();
            this.tscbDrawMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbSettings = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.scMainContainer)).BeginInit();
            this.scMainContainer.Panel1.SuspendLayout();
            this.scMainContainer.Panel2.SuspendLayout();
            this.scMainContainer.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tseMainToolstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.AcceptsTab = true;
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(28, 25);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(772, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.InputTextbox_TextChanged);
            this.textBox1.Enter += new System.EventHandler(this.textBox1_Enter);
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            this.textBox1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.textBox1_PreviewKeyDown);
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
            this.scMainContainer.TabStop = false;
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1, 24);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 21);
            this.button1.TabIndex = 4;
            this.button1.Text = "/\\";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.UpArrowButton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.toolStripSeparator1,
            this.openToolStripMenuItem1,
            this.renameToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 148);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.sizeToolStripMenuItem,
            this.dimensionsToolStripMenuItem,
            this.fileNameToolStripMenuItem,
            this.pathToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultToolStripMenuItem,
            this.invertedToolStripMenuItem,
            this.grayscaleToolStripMenuItem});
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.imageToolStripMenuItem.Text = "Image";
            this.imageToolStripMenuItem.Click += new System.EventHandler(this.imageToolStripMenuItem_Click);
            // 
            // defaultToolStripMenuItem
            // 
            this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            this.defaultToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.defaultToolStripMenuItem.Text = "Default";
            this.defaultToolStripMenuItem.Click += new System.EventHandler(this.defaultToolStripMenuItem_Click);
            // 
            // invertedToolStripMenuItem
            // 
            this.invertedToolStripMenuItem.Name = "invertedToolStripMenuItem";
            this.invertedToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.invertedToolStripMenuItem.Text = "Inverted";
            this.invertedToolStripMenuItem.Click += new System.EventHandler(this.invertedToolStripMenuItem_Click);
            // 
            // grayscaleToolStripMenuItem
            // 
            this.grayscaleToolStripMenuItem.Name = "grayscaleToolStripMenuItem";
            this.grayscaleToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.grayscaleToolStripMenuItem.Text = "Grayscale";
            this.grayscaleToolStripMenuItem.Click += new System.EventHandler(this.grayscaleToolStripMenuItem_Click);
            // 
            // sizeToolStripMenuItem
            // 
            this.sizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bytesToolStripMenuItem,
            this.kilobytesToolStripMenuItem,
            this.megabytesToolStripMenuItem,
            this.gigabytesToolStripMenuItem,
            this.terabytesToolStripMenuItem,
            this.petabyteToolStripMenuItem});
            this.sizeToolStripMenuItem.Name = "sizeToolStripMenuItem";
            this.sizeToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.sizeToolStripMenuItem.Text = "Size";
            // 
            // bytesToolStripMenuItem
            // 
            this.bytesToolStripMenuItem.Name = "bytesToolStripMenuItem";
            this.bytesToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.bytesToolStripMenuItem.Text = "Bytes";
            this.bytesToolStripMenuItem.Click += new System.EventHandler(this.bytesToolStripMenuItem_Click);
            // 
            // kilobytesToolStripMenuItem
            // 
            this.kilobytesToolStripMenuItem.Name = "kilobytesToolStripMenuItem";
            this.kilobytesToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.kilobytesToolStripMenuItem.Text = "Kilobytes";
            this.kilobytesToolStripMenuItem.Click += new System.EventHandler(this.kilobytesToolStripMenuItem_Click);
            // 
            // megabytesToolStripMenuItem
            // 
            this.megabytesToolStripMenuItem.Name = "megabytesToolStripMenuItem";
            this.megabytesToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.megabytesToolStripMenuItem.Text = "Megabytes";
            this.megabytesToolStripMenuItem.Click += new System.EventHandler(this.megabytesToolStripMenuItem_Click);
            // 
            // gigabytesToolStripMenuItem
            // 
            this.gigabytesToolStripMenuItem.Name = "gigabytesToolStripMenuItem";
            this.gigabytesToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.gigabytesToolStripMenuItem.Text = "Gigabytes";
            this.gigabytesToolStripMenuItem.Click += new System.EventHandler(this.gigabytesToolStripMenuItem_Click);
            // 
            // terabytesToolStripMenuItem
            // 
            this.terabytesToolStripMenuItem.Name = "terabytesToolStripMenuItem";
            this.terabytesToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.terabytesToolStripMenuItem.Text = "Terabytes";
            this.terabytesToolStripMenuItem.Click += new System.EventHandler(this.terabytesToolStripMenuItem_Click);
            // 
            // petabyteToolStripMenuItem
            // 
            this.petabyteToolStripMenuItem.Name = "petabyteToolStripMenuItem";
            this.petabyteToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.petabyteToolStripMenuItem.Text = "Petabyte";
            this.petabyteToolStripMenuItem.Click += new System.EventHandler(this.petabyteToolStripMenuItem_Click);
            // 
            // dimensionsToolStripMenuItem
            // 
            this.dimensionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.widthXHeightToolStripMenuItem,
            this.widthToolStripMenuItem,
            this.heightToolStripMenuItem});
            this.dimensionsToolStripMenuItem.Name = "dimensionsToolStripMenuItem";
            this.dimensionsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.dimensionsToolStripMenuItem.Text = "Dimensions";
            this.dimensionsToolStripMenuItem.Click += new System.EventHandler(this.dimensionsToolStripMenuItem_Click);
            // 
            // widthXHeightToolStripMenuItem
            // 
            this.widthXHeightToolStripMenuItem.Name = "widthXHeightToolStripMenuItem";
            this.widthXHeightToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.widthXHeightToolStripMenuItem.Text = "Width x Height";
            this.widthXHeightToolStripMenuItem.Click += new System.EventHandler(this.widthXHeightToolStripMenuItem_Click);
            // 
            // widthToolStripMenuItem
            // 
            this.widthToolStripMenuItem.Name = "widthToolStripMenuItem";
            this.widthToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.widthToolStripMenuItem.Text = "Width";
            this.widthToolStripMenuItem.Click += new System.EventHandler(this.widthToolStripMenuItem_Click);
            // 
            // heightToolStripMenuItem
            // 
            this.heightToolStripMenuItem.Name = "heightToolStripMenuItem";
            this.heightToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.heightToolStripMenuItem.Text = "Height";
            this.heightToolStripMenuItem.Click += new System.EventHandler(this.heightToolStripMenuItem_Click);
            // 
            // fileNameToolStripMenuItem
            // 
            this.fileNameToolStripMenuItem.Name = "fileNameToolStripMenuItem";
            this.fileNameToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.fileNameToolStripMenuItem.Text = "File Name";
            this.fileNameToolStripMenuItem.Click += new System.EventHandler(this.fileNameToolStripMenuItem_Click);
            // 
            // pathToolStripMenuItem
            // 
            this.pathToolStripMenuItem.Name = "pathToolStripMenuItem";
            this.pathToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.pathToolStripMenuItem.Text = "Path";
            this.pathToolStripMenuItem.Click += new System.EventHandler(this.pathToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.cutToolStripMenuItem.Text = "Move";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(114, 6);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInExplorerToolStripMenuItem,
            this.openWithDefaultProgramToolStripMenuItem,
            this.openWithToolStripMenuItem});
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // openInExplorerToolStripMenuItem
            // 
            this.openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
            this.openInExplorerToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openInExplorerToolStripMenuItem.Text = "Open In Explorer";
            this.openInExplorerToolStripMenuItem.Click += new System.EventHandler(this.openInExplorerToolStripMenuItem_Click);
            // 
            // openWithDefaultProgramToolStripMenuItem
            // 
            this.openWithDefaultProgramToolStripMenuItem.Name = "openWithDefaultProgramToolStripMenuItem";
            this.openWithDefaultProgramToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openWithDefaultProgramToolStripMenuItem.Text = "Open In Default Program";
            this.openWithDefaultProgramToolStripMenuItem.Click += new System.EventHandler(this.openWithDefaultProgramToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(114, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // openWithToolStripMenuItem
            // 
            this.openWithToolStripMenuItem.Name = "openWithToolStripMenuItem";
            this.openWithToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openWithToolStripMenuItem.Text = "Open With";
            this.openWithToolStripMenuItem.Click += new System.EventHandler(this.openWithToolStripMenuItem_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chSize,
            this.chPath});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewGroup7.Header = "Directories";
            listViewGroup7.Name = "Directories";
            listViewGroup8.Header = "Files";
            listViewGroup8.Name = "Files";
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup7,
            listViewGroup8});
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
            this.imageDisplay1.TabStop = false;
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
            this.openToolStripMenuItem});
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
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
            this.tscbInterpolationMode,
            this.tscbDrawMode,
            this.toolStripMenuItem1});
            this.toolStripDropDownButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton4.Name = "toolStripDropDownButton4";
            this.toolStripDropDownButton4.Size = new System.Drawing.Size(45, 22);
            this.toolStripDropDownButton4.Text = "View";
            // 
            // tscbInterpolationMode
            // 
            this.tscbInterpolationMode.Name = "tscbInterpolationMode";
            this.tscbInterpolationMode.Size = new System.Drawing.Size(121, 23);
            // 
            // tscbDrawMode
            // 
            this.tscbDrawMode.Name = "tscbDrawMode";
            this.tscbDrawMode.Size = new System.Drawing.Size(121, 23);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItem1.Text = "Color Picker";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
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
            this.Controls.Add(this.button1);
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
            this.contextMenuStrip1.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripStatusLabel tsslFileSize;
        private System.Windows.Forms.ToolStripStatusLabel tsslFilePath;
        private System.Windows.Forms.ToolStripStatusLabel tsslSPACE1;
        private System.Windows.Forms.ToolStripStatusLabel tsslImageSize;
        private System.Windows.Forms.ToolStripStatusLabel tsslItemOfItems;
        private System.Windows.Forms.ToolStripComboBox tscbInterpolationMode;
        private System.Windows.Forms.ToolStripButton tsbSettings;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grayscaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bytesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kilobytesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem megabytesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gigabytesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dimensionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem widthXHeightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem widthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWithDefaultProgramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem terabytesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem petabyteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripComboBox tscbDrawMode;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWithToolStripMenuItem;
    }
}

