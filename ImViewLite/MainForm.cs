using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ImViewLite.Forms;
using ImViewLite.Helpers;
using ImViewLite.Settings;
using ImViewLite.Controls;
using ImViewLite.Enums;
using ImViewLite.Misc;
using System.Text;

namespace ImViewLite
{
    public partial class MainForm : Form
    {

        /// <summary>
        /// The current worker directory / displayed directory
        /// </summary>
        public string CurrentDirectory
        {
            get { return _CurrentDirectory; }
            set
            {
                if (this._CurrentDirectory == value)
                    return;

                if (!_Undo)
                {
                    _FolderUndoHistory.Push(this._CurrentDirectory);
                    _FolderRedoHistory.Clear();
                }
                
                if (this.TopMost)
                {
                    this.Text = "+ " + value;
                }
                else
                {
                    this.Text = value;
                }

                this.DirectorySelectedIndexCache[_CurrentDirectory] = listView1.NewestSelectedIndex;
                this._CurrentDirectory = value;
                this.LoadDirectory(value);
            }
        }
        private string _CurrentDirectory = "";

        /// <summary>
        /// Keeps track of the first index of the first item in the item cache
        /// </summary>
        private int _CahceItem1;                     

        /// <summary>
        /// Items currently displayed in the listview
        /// </summary>
        private ListViewItemEx[] _ListViewItemCache;

        /// <summary>
        /// Keeps track of the current directory, watches for system changes and contains FileCache and DirectoryCache
        /// </summary>
        private FolderWatcher _FolderWatcher;

        /// <summary>
        /// The previously visited directories 
        /// </summary>
        private Stack<string> _FolderUndoHistory = new Stack<string>();

        /// <summary>
        /// The previously visited directories after they've been undone
        /// </summary>
        private Stack<string> _FolderRedoHistory = new Stack<string>();
        
        /// <summary>
        /// Holds the last selected index before changing a directory.
        /// </summary>
        private Dictionary<string, int> DirectorySelectedIndexCache = new Dictionary<string, int>();

        /// <summary>
        /// Timer to load the image 
        /// </summary>
        private TIMER _LoadImageTimer = new TIMER() { Interval = 50 };

        /// <summary>
        /// Is the textbox currently active / selected
        /// </summary>
        private bool _IsUsingTextbox = false;

        /// <summary>
        /// Prevents overflow errors from event callbacks looping
        /// </summary>
        private bool _PreventOverflow = false;

        /// <summary>
        /// Is an undo / redo occuring
        /// </summary>
        private bool _Undo = false;

        public MainForm()
        {
            InitializeComponent();
            SuspendLayout();
            this.tseMainToolstrip.ClickThrough = InternalSettings.Toolstrip_Click_Through;
            //this.imageDisplay1.CenterImage = false;
            this.imageDisplay1.CellScale = 1;
            this.imageDisplay1.CellSize = InternalSettings.Grid_Cell_Size;
            this.imageDisplay1.CellColor1 = InternalSettings.Image_Box_Back_Color;
            this.imageDisplay1.CellColor2 = InternalSettings.Image_Box_Back_Color_Alternate;
            this.imageDisplay1.InterpolationMode = InternalSettings.Default_Interpolation_Mode;
            this.imageDisplay1.DrawMode = InternalSettings.Default_Draw_Mode;

            tscbInterpolationMode.Items.Add(System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);
            tscbInterpolationMode.Items.Add(System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);
            tscbInterpolationMode.Items.Add(System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear);
            tscbInterpolationMode.SelectedItem = InternalSettings.Default_Interpolation_Mode;
            tscbInterpolationMode.SelectedIndexChanged += TscbInterpolationMode_SelectedIndexChanged;

            tscbDrawMode.Items.Add(ImViewLite.Controls.DrawMode.ActualSize);
            tscbDrawMode.Items.Add(ImViewLite.Controls.DrawMode.FitImage);
            tscbDrawMode.Items.Add(ImViewLite.Controls.DrawMode.ScaleImage);
            tscbDrawMode.SelectedItem = InternalSettings.Default_Draw_Mode;
            tscbDrawMode.SelectedIndexChanged += TscbDrawMode_SelectedIndexChanged;

            _FolderWatcher = new FolderWatcher();
            _FolderWatcher.FileRemoved += _FolderWatcher_FileRemoved;
            _FolderWatcher.DirectoryRemoved += _FolderWatcher_DirectoryRemoved;
            _FolderWatcher.FileAdded += _FolderWatcher_FileAdded;
            _FolderWatcher.DirectoryAdded += _FolderWatcher_DirectoryAdded;
            _FolderWatcher.DirectoryRenamed += _FolderWatcher_DirectoryRenamed;
            _FolderWatcher.FileRenamed += _FolderWatcher_FileRenamed;
            _FolderWatcher.ItemChanged += _FolderWatcher_ItemChanged;

            this.listView1.VirtualMode = true;
            this.listView1.VirtualListSize = 0;
            this.listView1.Sorting = SortOrder.None;
            this.listView1.FullRowSelect = InternalSettings.Full_Row_Select;

            listView1.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(listView1_RetrieveVirtualItem);
            listView1.CacheVirtualItems += new CacheVirtualItemsEventHandler(listView1_CacheVirtualItems);
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            listView1.ItemActivate += ListView1_ItemActivate;
            listView1.RightClicked += ListView1_RightClicked;

            this.KeyUp += MainForm_KeyUp;
            _LoadImageTimer.SetInterval(InternalSettings.Image_Delay_Load_Time);
            _LoadImageTimer.Tick += LoadImageTimer_Tick;

            this.textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.textBox1.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
            this.textBox1.Text = "";
            this.TopMost = InternalSettings.Always_On_Top;
            this.KeyPreview = true;
            ResumeLayout();

            // empty string to just list all drives 
            _CurrentDirectory = "";
            LoadDirectory("");
            LoadFavoriteDirectories();
        }




        /// <summary>
        /// Empties the listview item cache and redraws the listview
        /// </summary>
        public void RefreshListView()
        {
            //lock (_CacheLock)
            //{
                this._ListViewItemCache = new ListViewItemEx[] { };
                this.listView1.Invalidate();
            //}

            if (InternalSettings.Agressive_Image_Unloading)
                DelayLoadImage(this.listView1.GetSelectedItemText2());
        }

        /// <summary>
        /// Updates the text of the textbox to the current directory
        /// </summary>
        public void UpdateTextbox()
        {
            this._PreventOverflow = true;
            this.textBox1.Text = $"{_CurrentDirectory}";
            this._PreventOverflow = false;
        }

        /// <summary>
        /// Takes the user back to the directory before the previous directory
        /// </summary>
        public void UndoPreviousDirectory()
        {
            if (_FolderRedoHistory.Count < 1)
                return;

            this._Undo = true;
            string newDir = this._FolderRedoHistory.Pop();
            
            if (newDir != "" && !Directory.Exists(newDir)) 
            { 
                UndoPreviousDirectory();
                this._Undo = false;
                return;
            }
            this._FolderUndoHistory.Push(this._CurrentDirectory);
            this.LoadDirectory(newDir);
            this.LastDirectoryIndex();
            
            this._Undo = false;
            UpdateTextbox();
        }

        /// <summary>
        /// Takes the user back to the previous directory
        /// </summary>
        public void PreviousDirectory()
        {
            if (_FolderUndoHistory.Count < 1)
                return;
            
            this._Undo = true;
            string newDir = this._FolderUndoHistory.Pop();

            if (newDir != "" && !Directory.Exists(newDir))
            {
                PreviousDirectory();
                this._Undo = false;
                return;
            }

            this._FolderRedoHistory.Push(this._CurrentDirectory);
            this.LoadDirectory(newDir);
            this.LastDirectoryIndex();

            this._Undo = false;
            UpdateTextbox();
        }

        /// <summary>
        /// Updates variables on the mainfrom which reference the InternalSettings class
        /// </summary>
        public void UpdateSettings()
        {
            this.tseMainToolstrip.ClickThrough = InternalSettings.Toolstrip_Click_Through;
            this.TopMost = InternalSettings.Always_On_Top;
            this.imageDisplay1.CellSize = InternalSettings.Grid_Cell_Size;
            this.imageDisplay1.CellColor1 = InternalSettings.Image_Box_Back_Color;
            this.imageDisplay1.CellColor2 = InternalSettings.Image_Box_Back_Color_Alternate;
            this.listView1.FullRowSelect = InternalSettings.Full_Row_Select;
            this.imageDisplay1.InterpolationMode = InternalSettings.Default_Interpolation_Mode;
            this.imageDisplay1.DrawMode = InternalSettings.Default_Draw_Mode;
            this._LoadImageTimer.SetInterval(InternalSettings.Image_Delay_Load_Time);
            
            if (this.TopMost)
            {
                this.Text = "+ " + this._CurrentDirectory;
            }
            else
            {
                this.Text = this._CurrentDirectory;
            }
        }

        /// <summary>
        /// Changes the current working directory into the given directory
        /// And loads all the files and sub directories from the given into sorted lists
        /// </summary>
        /// <param name="path">The new directory path.</param>
        /// <param name="update">Forces a reload, even if the current directory is the same as the given.</param>
        public void LoadDirectory(string path, bool update = false)
        {
            if (_CurrentDirectory != path && !update)
            {
                CurrentDirectory = path;
                return;
            }

            if (!string.IsNullOrEmpty(path))
            {
                Directory.SetCurrentDirectory(path);
            }
            else
            {
                Directory.SetCurrentDirectory("C:\\");
            }
            _CurrentDirectory = path;
            _FolderWatcher.UpdateDirectory(path);
            Console.WriteLine(_FolderWatcher.CurrentDirectory);
            this.listView1.VirtualListSize = _FolderWatcher.GetTotalCount();
            RefreshListView();

            if (InternalSettings.Agressive_Image_Unloading)
            {
                this.ReloadCurrentImage();
            }

            this.listView1.Invalidate();
            GC.Collect();
        }

        /// <summary>
        /// Changes the current directory to the given path and loads the new directory
        /// </summary>
        /// <param name="path">The new directory path.</param>
        /// <param name="updateTextbox">Should the textbox at the top be changed.</param>
        public void UpdateDirectory(string path, bool updateTextbox = false)
        {
            if (!Directory.Exists(path))
                return;

            this._PreventOverflow = true;
            path = new DirectoryInfo(path).FullName;

            if (path[path.Length - 1] != '\\')
                path += '\\';

            this.CurrentDirectory = path;

            if (updateTextbox)
            {
                UpdateTextbox();
            }
            this._PreventOverflow = false;
        }

        /// <summary>
        /// Moves the given file to the given path.
        /// </summary>
        /// <param name="path">The path of the file to move.</param>
        /// <param name="args">The args of the keybind, arg[0] should be the new directory path.</param>
        public void MoveImage(string path, string[] args = null)
        {
            if (!File.Exists(path))
                return;

            if (args != null && args.Length > 0 && Directory.Exists(args[0]))
            {
                PathHelper.MoveFile(path, Path.Combine(args[0], Path.GetFileName(path)));
            }
            else
            {
                PathHelper.MoveFile(path, Helper.MoveFileDialog(path));
            }
        }

        public void LastDirectoryIndex()
        {
            if (this.DirectorySelectedIndexCache.ContainsKey(_CurrentDirectory))
            {
                int index = this.DirectorySelectedIndexCache[_CurrentDirectory];
                if (this.listView1.Items.Count >= index || index < 0)
                    return;

                this.listView1.DeselectAll();
                this.listView1.OldestSelectedIndex = index;
                this.listView1.NewestSelectedIndex = index;
                this.listView1.SelectedItemsCount = 1;
                this.listView1.SelectedIndices.Clear();
                this.listView1.SelectedIndices.Add(index);
                this.listView1.Items[index].Focused = true;
                this.listView1.Invalidate();
            }
        }

        /// <summary>
        /// Moves the current directory to its parent.
        /// </summary>
        public void UpDirectoryLevel()
        {
            if (!PathHelper.IsValidDirectoryPath(this.CurrentDirectory))
                return;

            DirectoryInfo info = new DirectoryInfo(this.CurrentDirectory);
            if (info.Parent != null)
            {
                this.UpdateDirectory(info.Parent.FullName, true);
                this.LastDirectoryIndex();
            }
            else
            {
                this.LoadDirectory("", true);
            }
        }

        /// <summary>
        /// Toggles the always on top setting and applies changes.
        /// </summary>
        public void ToggleAlwaysOnTop()
        {
            this.TopMost = !this.TopMost;
            InternalSettings.Always_On_Top = this.TopMost;
            if (this.TopMost)
            {
                this.Text = "+ " + this._CurrentDirectory;
            }
            else
            {
                this.Text = this._CurrentDirectory;
            }
        }

        /// <summary>
        /// Opens the settings file dialog.
        /// </summary>
        public void OpenSettings()
        {
            using (SettingsForm sf = new SettingsForm())
            {
                sf.TopMost = this.TopMost;
                sf.Owner = this;
                sf.StartPosition = FormStartPosition.CenterScreen;
                sf.ShowDialog();
                UpdateSettings();
            }
        }

        /// <summary>
        /// Opens a new color picker.
        /// </summary>
        public void OpenColorPicker()
        {
            ColorPickerForm cpf = new ColorPickerForm();
            cpf.TopMost = this.TopMost;
            cpf.StartPosition = FormStartPosition.CenterScreen;
            cpf.Show();
        }

        /// <summary>
        /// Deletes the given path.
        /// Asks the user to confirm if the setting is enabled.
        /// </summary>
        /// <param name="path">The pat hto delete.</param>
        public void DeletePath(string path)
        {
            if (InternalSettings.Ask_Delete_Confirmation_Single)
            {
                if (MessageBox.Show(this,
                    $"Are you sure you want to delete this item?\n{path}",
                    "Delete File?",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }
            PathHelper.DeleteFileOrPath(path);
        }

        /// <summary>
        /// Reloads the currently displayed image if the path is different
        /// </summary>
        public void ReloadCurrentImage()
        {
            string newPath = listView1.GetSelectedItemText2();

            FileInfo path;
            if (PathHelper.IsValidFilePath(newPath, out path))
            {
                if (imageDisplay1.ImagePath == null || 
                    path.FullName != imageDisplay1.ImagePath.FullName || 
                    path.Length != imageDisplay1.ImagePath.Length)
                {
                    LoadImage(newPath);
                }
            }
            
        }

        /// <summary>
        /// Copies the text of the currently selected subitems to clipboard
        /// </summary>
        /// <param name="index"></param>
        public void CopySelectedItemText(int index)
        {
            if (listView1.SelectedIndices.Count < 2)
            {
                if (listView1.FocusedItem != null)
                {
                    ClipboardHelper.CopyText(listView1.FocusedItem.SubItems[index].Text);
                }
                return;
            }

            StringBuilder paths = new StringBuilder();

            int i = 0;
            foreach (int ii in listView1.SelectedIndices)
            {
                paths.AppendLine(listView1.Items[ii].SubItems[index].Text);
                i++;
            }
            ClipboardHelper.CopyText(paths.ToString());
        }

        /// <summary>
        /// Copies the file size in the given unit for all selected listview items.
        /// </summary>
        /// <param name="size">The size unit.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public void CopySelectedItemsSize(FileSizeUnit size, int decimalPlaces = 1)
        {
            FileInfo info;

            if (listView1.SelectedIndices.Count < 2)
            {
                if (listView1.FocusedItem != null)
                {
                    info = new FileInfo(listView1.FocusedItem.SubItems[2].Text);
                    ClipboardHelper.CopyText(Helper.SizeSuffix(info.Length, size, decimalPlaces));
                }
                return;
            }

            StringBuilder paths = new StringBuilder();

            int i = 0;
            foreach (int ii in listView1.SelectedIndices)
            {
                info = new FileInfo(listView1.Items[ii].SubItems[2].Text);
                paths.AppendLine(Helper.SizeSuffix(info.Length, size, decimalPlaces));
                i++;
            }
            ClipboardHelper.CopyText(paths.ToString());
        }

        /// <summary>
        /// Copies the file names of the selected listview items
        /// </summary>
        public void CopySelectedFileNames()
        {
            CopySelectedItemText(0);
        }

        /// <summary>
        /// Copies the file paths of the selected listview items
        /// </summary>
        public void CopySelectedPaths()
        {
            CopySelectedItemText(2);
        }

        /// <summary>
        /// Copies the dimensions of the selected listview items, if they're images
        /// </summary>
        /// <param name="formatDims">The format of the widthxheight as a interpolated string -> "{0}x{1}"</param>
        public void CopyDimensionsSelectedFiles(string formatDims)
        {
            Size dims;
            if (listView1.SelectedIndices.Count < 2)
            {
                if (listView1.FocusedItem != null)
                {
                    dims = ImageHelper.GetImageDimensionsFromFile(listView1.FocusedItem.SubItems[2].Text);
                    ClipboardHelper.CopyText(string.Format(formatDims, dims.Width, dims.Height));
                }
                return;
            }

            StringBuilder paths = new StringBuilder();

            int i = 0;
            foreach (int ii in listView1.SelectedIndices)
            {
                dims = ImageHelper.GetImageDimensionsFromFile(listView1.FocusedItem.SubItems[2].Text);
                paths.AppendLine(string.Format(formatDims, dims.Width, dims.Height));
                i++;
            }
            ClipboardHelper.CopyText(paths.ToString());
        }

        /// <summary>
        /// Copies the selected files / folders to the clipboard
        /// </summary>
        public void CopySelectedFiles()
        {
            if (listView1.SelectedIndices.Count < 2)
            {
                if (listView1.FocusedItem != null)
                {
                    ClipboardHelper.CopyFile(listView1.FocusedItem.SubItems[2].Text);
                }
                return;
            }

            string[] files = new string[listView1.SelectedIndices.Count];

            int i = 0;
            foreach (int ii in listView1.SelectedIndices)
            {
                files[i] = listView1.Items[ii].SubItems[2].Text;
                i++;
            }
            ClipboardHelper.CopyFile(files);
        }

        /// <summary>
        /// Deletes the selected listview items
        /// </summary>
        public void DeleteSelectedItems()
        {
            if (listView1.SelectedIndices.Count > 1)
            {
                if (InternalSettings.Ask_Delete_Confirmation_Multiple)
                {
                    if (MessageBox.Show(this,
                        $"Are you sure you want to delete {listView1.SelectedIndices.Count} items?\n",
                        "Delete Files?",
                        MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }

                string[] del = new string[listView1.SelectedIndices.Count];
                int c = 0;
                foreach (int i in listView1.SelectedIndices)
                {
                    del[c] = listView1.Items[i].SubItems[2].Text;
                    c++;
                }
                this.listView1.DeselectAll();

                Task.Run(() =>
                {
                    foreach (string i in del)
                    {
                        PathHelper.DeleteFileOrPath(i);
                    }
                });
                return;
            }

            if (listView1.FocusedItem != null)
            {
                DeletePath(listView1.FocusedItem.SubItems[2].Text);
            }
        }

        /// <summary>
        /// Renames the selected listview items
        /// </summary>
        public void RenameSelectedItems()
        {
            if (listView1.SelectedIndices.Count > 1)
            {
                DialogResult dr;
                if (InternalSettings.Ask_Rename_Multiple_Files)
                {
                    dr = MessageBox.Show(
                        this,
                        "You are trying to rename multiple files\nWould you like to rename all files?",
                        "Rename Multiple Or Single",
                        MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    dr = DialogResult.Yes;
                }

                if (dr == DialogResult.Cancel)
                {
                    return;
                }

                if (dr == DialogResult.Yes)
                {
                    foreach (int i in listView1.SelectedIndices)
                    {
                        RenameFileForm.RenamePath(listView1.Items[i].SubItems[2].Text);
                    }
                    return;
                }
            }

            if (listView1.FocusedItem != null)
            {
                RenameFileForm.RenamePath(listView1.FocusedItem.SubItems[2].Text);
            }
        }

        /// <summary>
        /// Opens the selected listview items
        /// </summary>
        public void OpenSelectedItems()
        {
            if (listView1.SelectedIndices.Count > 1)
            {
                DialogResult dr;
                if (InternalSettings.Ask_Open_Multiple_Files)
                {
                    dr = MessageBox.Show(
                        this,
                        $"You are trying to open {listView1.SelectedIndices.Count} files\nAre you sure you want to open all of them?",
                        "Open Multiple Or Single",
                        MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    dr = DialogResult.Yes;
                }

                if (dr == DialogResult.Cancel)
                {
                    return;
                }

                if (dr == DialogResult.Yes)
                {
                    foreach (int i in listView1.SelectedIndices)
                    {
                        string p = listView1.Items[i].SubItems[2].Text;

                        if (File.Exists(p))
                        {
                            PathHelper.OpenWithDefaultProgram(p);
                        }
                        else if (Directory.Exists(p))
                        {
                            PathHelper.OpenExplorerAtLocation(p);
                        }
                    }
                    return;
                }
            }

            if (listView1.FocusedItem != null)
            {
                PathHelper.OpenWithDefaultProgram(listView1.FocusedItem.SubItems[2].Text);
            }
        }

        /// <summary>
        /// Opens explorer with the selected items highlighted
        /// </summary>
        public void OpenExplorerAtSelectedItems()
        {
            if (listView1.SelectedIndices.Count > 1)
            {
                DialogResult dr;
                if (InternalSettings.Ask_Open_Multiple_Files_In_Explorer)
                {
                    dr = MessageBox.Show(
                        this,
                        $"You are trying to open {listView1.SelectedIndices.Count} with explorer\nAre you sure you want to open all of them?",
                        "Open Multiple Or Single",
                        MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    dr = DialogResult.Yes;
                }

                if (dr == DialogResult.Cancel)
                {
                    return;
                }

                if (dr == DialogResult.Yes)
                {
                    foreach (int i in listView1.SelectedIndices)
                    {
                        PathHelper.OpenExplorerAtLocation(listView1.Items[i].SubItems[2].Text);
                    }
                    return;
                }
            }

            if (listView1.FocusedItem != null)
            {
                PathHelper.OpenExplorerAtLocation(listView1.FocusedItem.SubItems[2].Text);
            }
        }

        /// <summary>
        /// Moves the selected items to the given path
        /// </summary>
        /// <param name="path"></param>
        public void MoveSelectedFiles(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                try
                {
                    // need to fix this, it doesn't fire DirectoryCreated Event in the file sytstem watcher
                    // when used to create a directory
                    path = PathHelper.SelectFolderDialog(Path.GetDirectoryName(CurrentDirectory));
                }
                catch
                {
                    path = PathHelper.SelectFolderDialog();
                }
            }

            string file;

            if (listView1.SelectedIndices.Count > 1)
            {
                foreach (int i in listView1.SelectedIndices)
                {
                    file = listView1.Items[i].SubItems[2].Text;
                    PathHelper.MoveFile(file, Path.Combine(path, Path.GetFileName(file)));
                }
                return;
            }

            if (listView1.FocusedItem != null)
            {
                file = listView1.FocusedItem.SubItems[2].Text;
                PathHelper.MoveFile(file, Path.Combine(path, Path.GetFileName(file)));
            }
        }

        /// <summary>
        /// Loads the given image in the display box
        /// </summary>
        /// <param name="path">The path to the image.</param>
        public void LoadImage(string path)
        {
            if (!PathHelper.IsValidFilePath(path))
                return;
            
            FileInfo finfo = new FileInfo(path);

            if (!finfo.Exists)
            {
                imageDisplay1.Image = null;
                return;
            }

            tsslFileSize.Text = Helper.SizeSuffix(finfo.Length);
            tsslFilePath.Text = finfo.Name;

            if (imageDisplay1.TryLoadImage(path))
            {
                tsslImageSize.Text = $"{imageDisplay1.Image.Width} x {imageDisplay1.Image.Height}";
            }
            else if (InternalSettings.Agressive_Image_Unloading)
            {
                imageDisplay1.Image = null;
                tsslImageSize.Text = "0 x 0";
            }
        }

        /// <summary>
        /// Starts the LoadImageTimer and sets the given path to the next image
        /// </summary>
        /// <param name="path">The image that will be loaded on tick</param>
        public void DelayLoadImage(string path)
        {
            //this.InvokeSafe(() => { 
            _LoadImageTimer.Stop();
            _LoadImageTimer.Start();
            //});
        }

        public void OpenFavoriteDirectory(int index)
        {
            index += 1;

            if (index >= this.toolStripDropDownButton3.DropDownItems.Count)
                return;
            
            this.CurrentDirectory = ((DirectoryInfo)this.toolStripDropDownButton3.DropDownItems[index].Tag).FullName;
        }

        /// <summary>
        /// Executes a commad with arg as the first argument.
        /// </summary>
        /// <param name="cmd">The command to run.</param>
        /// <param name="arg">The first and only argument.</param>
        public void ExecuteCommand(Command cmd, string arg)
        {
            ExecuteCommand(cmd, new string[] { arg });
        }

        /// <summary>
        /// Executes a command. This is used for when a user presses keybinds.
        /// Args[0] should almost always be a file path
        /// </summary>
        /// <param name="cmd">The command to run.</param>
        /// <param name="args">The arguments for the command.</param>
        public void ExecuteCommand(Command cmd, string[] args = null)
        {
            string path;

            if (args != null && args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                path = listView1.GetSelectedItemText2();
            }

            switch (cmd)
            {
                case Command.Nothing: break;
                case Command.CopyImage: imageDisplay1.CopyImage(); break;
                case Command.PauseGif: imageDisplay1.AnimationPaused = !imageDisplay1.AnimationPaused; break;
                case Command.InvertColor: imageDisplay1.InvertColor(); break;
                case Command.Grayscale: imageDisplay1.ConvertGrayscale(); break;
                case Command.NextFrame: imageDisplay1.NextImageFrame(); break;
                case Command.PreviousFrame: imageDisplay1.PreviousImageFrame(); break;
                case Command.UpDirectoryLevel: UpDirectoryLevel(); break;
                case Command.OpenSelectedDirectory: UpdateDirectory(path, true); break;
                case Command.MoveImage: MoveSelectedFiles(path); break;
                case Command.RenameImage: RenameSelectedItems(); break;
                case Command.DeleteImage: DeleteSelectedItems(); break;
                case Command.ToggleAlwaysOnTop: ToggleAlwaysOnTop(); break;
                case Command.OpenColorPicker: OpenColorPicker(); break;
                case Command.OpenSettings: OpenSettings(); break;
                case Command.OpenWithDefaultProgram: OpenSelectedItems(); break;
                case Command.OpenExplorerAtLocation: OpenExplorerAtSelectedItems(); break;
                case Command.LastDirectory: PreviousDirectory(); break;
                case Command.UndoLastDirectory: UndoPreviousDirectory(); break;
                
                case Command.OpenFavoriteDirectory:

                    Console.WriteLine(path);
                    if(int.TryParse(path, out int index))
                        OpenFavoriteDirectory(index);
                    
                    break;
            }
        }

        private void LoadFavoriteDirectories()
        {
            for(int i = 2; i < this.toolStripDropDownButton3.DropDownItems.Count; i++)
            {
                this.toolStripDropDownButton3.DropDownItems.RemoveAt(i);
            }

            int c = 0;
            foreach(string i in InternalSettings.Favorite_Directories)
            {
                c++;
                if (PathHelper.IsValidDirectoryPath(i, out DirectoryInfo info))
                {
                    ToolStripMenuItem btn = new ToolStripMenuItem(i, null, FavoriteItemClicked, $"favItem{c}");
                    btn.ShortcutKeyDisplayString = c.ToString();
                    btn.Tag = info;
                    this.toolStripDropDownButton3.DropDownItems.Add(btn);
                }
            }
        }

        private void FavoriteItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem btn = sender as ToolStripMenuItem;

            if (btn == null)
                return;

            this.CurrentDirectory = ((DirectoryInfo)btn.Tag).FullName;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!_IsUsingTextbox)
                if (InternalSettings.CurrentUserSettings.Binds.ContainsKey(e.KeyData))
                {
                    ExecuteCommand(
                        InternalSettings.CurrentUserSettings.Binds[e.KeyData].Function,
                        InternalSettings.CurrentUserSettings.Binds[e.KeyData].Args);
                }
        }

        private void LoadImageTimer_Tick(object sender, EventArgs e)
        {
            // time has passed, stop timer and load the image
            _LoadImageTimer.Stop();
            ReloadCurrentImage();
        }

        private void UpdateAfterIndexChanged()
        {
            string path = listView1.GetSelectedItemText2();

            tsslItemOfItems.Text = $"{listView1.SelectedItemsCount} / {listView1.Items.Count} object(s) selected";

            if (Directory.Exists(path))
            {
                DirectoryInfo dinfo = new DirectoryInfo(path);
                tsslFilePath.Text = dinfo.FullName;
                statusStrip1.ResumeLayout();
                return;
            }

            // this will be called very very fast if the user drags a selection box over many items
            // this will prevent a performance hit at the cost of 50ms delay 
            if (File.Exists(path))
            {
                DelayLoadImage(path);
            }

        }

        private void ListView1_ItemActivate(object sender, EventArgs e)
        {
            if (_PreventOverflow || listView1.NewestSelectedIndex == -1)
                return;
            if (File.Exists(listView1.Items[listView1.NewestSelectedIndex].SubItems[2].Text))
            {
                if (InternalSettings.Open_With_Default_Program_On_Enter)
                {
                    ExecuteCommand(Command.OpenWithDefaultProgram, listView1.Items[listView1.NewestSelectedIndex].SubItems[2].Text);
                }
                return;
            }
            UpdateDirectory(listView1.Items[listView1.NewestSelectedIndex].SubItems[2].Text, true);
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterIndexChanged();
        }


        //The basic VirtualMode function.  Dynamically returns a ListViewItem
        //with the required properties; in this case, the square of the index.
        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //Caching is not required but improves performance on large sets.
            //To leave out caching, don't connect the CacheVirtualItems event 
            //and make sure myCache is null.
            //check to see if the requested item is currently in the cache
            if (_ListViewItemCache != null && e.ItemIndex >= _CahceItem1 && e.ItemIndex < _CahceItem1 + _ListViewItemCache.Length)
            {
                //A cache hit, so get the ListViewItem from the cache instead of making a new one.
                e.Item = _ListViewItemCache[e.ItemIndex - _CahceItem1];
            }
            else
            {
                if (e.ItemIndex < _FolderWatcher.DirectoryCache.Count)
                {
                    _FolderWatcher.WaitThreadsFinished(false);
                    DirectoryInfo dinfo = new DirectoryInfo(_FolderWatcher.DirectoryCache[e.ItemIndex]);
                    ListViewItemEx ditem = new ListViewItemEx(dinfo.Name);
                    ditem.SubItems.Add("");
                    ditem.SubItems.Add(dinfo.FullName);

                    e.Item = ditem;
                    return;
                }
                _FolderWatcher.WaitThreadsFinished();

                int index = e.ItemIndex - _FolderWatcher.DirectoryCache.Count;
                
                FileInfo finfo;
                ListViewItemEx fitem;

                if (index < _FolderWatcher.FileCache.Count)
                {
                    finfo = new FileInfo(_FolderWatcher.FileCache[index]);
                    fitem = new ListViewItemEx(finfo.Name);
                    if (finfo.Exists)
                    {
                        fitem.SubItems.Add(Helper.SizeSuffix(finfo.Length, 2));
                    }
                    else
                    {
                        fitem.SubItems.Add(Helper.SizeSuffix(0, 2));
                    }
                    fitem.SubItems.Add(finfo.FullName);
                }
                else
                {
                    fitem = new ListViewItemEx();
                    fitem.SubItems.Add("");
                    fitem.SubItems.Add("");
                }

                e.Item = fitem;
            }
        }

        //Manages the cache.  ListView calls this when it might need a 
        //cache refresh.
        private void listView1_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            //We've gotten a request to refresh the cache.
            //First check if it's really neccesary.
            if (_ListViewItemCache != null && e.StartIndex >= _CahceItem1 && e.EndIndex <= _CahceItem1 + _ListViewItemCache.Length)
            {
                //If the newly requested cache is a subset of the old cache, 
                //no need to rebuild everything, so do nothing.
                return;
            }
            //Now we need to rebuild the cache.
            _CahceItem1 = e.StartIndex;

            int length = e.EndIndex - e.StartIndex + 1; //indexes are inclusive
            int start = e.StartIndex;
            int end = e.EndIndex;
            int count = 0;

            _ListViewItemCache = new ListViewItemEx[length];

            int dirCount = _FolderWatcher.GetDirectoryCount();

            // start and ends in directory cache
            if (end < dirCount)
            {
                _FolderWatcher.WaitThreadsFinished(false);
                for (int index = start; index <= end; index++)
                {
                    DirectoryInfo dinfo;
                    ListViewItemEx ditem;

                    if (index < _FolderWatcher.DirectoryCache.Count)
                    {
                        dinfo = new DirectoryInfo(_FolderWatcher.DirectoryCache[index]);
                        ditem = new ListViewItemEx(dinfo.Name);

                        ditem.SubItems.Add("");
                        ditem.SubItems.Add(dinfo.FullName);
                    }
                    else
                    {
                        ditem = new ListViewItemEx();
                        ditem.SubItems.Add("");
                        ditem.SubItems.Add("");
                    }
                    _ListViewItemCache[count] = ditem;
                    count++;
                }
                return;
            }

            // starts in directory cache, ends in file cache
            if (start < dirCount)
            {
                _FolderWatcher.WaitThreadsFinished(false);
                for (int index = start; index < _FolderWatcher.DirectoryCache.Count; index++)
                {
                    DirectoryInfo dinfo;
                    ListViewItemEx ditem;

                    if (index < _FolderWatcher.DirectoryCache.Count)
                    {
                        dinfo = new DirectoryInfo(_FolderWatcher.DirectoryCache[index]);
                        ditem = new ListViewItemEx(dinfo.Name);

                        ditem.SubItems.Add("");
                        ditem.SubItems.Add(dinfo.FullName);
                    }
                    else
                    {
                        ditem = new ListViewItemEx();
                        ditem.SubItems.Add("");
                        ditem.SubItems.Add("");
                    }

                    _ListViewItemCache[count] = ditem;
                    count++;
                }

                _FolderWatcher.WaitThreadsFinished(true);
                for (int index = 0; count < length; index++)
                {
                    FileInfo finfo;
                    ListViewItemEx fitem;

                    if (index < _FolderWatcher.FileCache.Count)
                    {
                        finfo = new FileInfo(_FolderWatcher.FileCache[index]);
                        fitem = new ListViewItemEx(finfo.Name);

                        if (finfo.Exists)
                        {
                            fitem.SubItems.Add(Helper.SizeSuffix(finfo.Length, 2));
                        }
                        else
                        {
                            fitem.SubItems.Add("DELETED");
                        }
                        fitem.SubItems.Add(finfo.FullName);
                    }
                    else
                    {
                        fitem = new ListViewItemEx();
                        fitem.SubItems.Add("");
                        fitem.SubItems.Add("");
                    }

                    _ListViewItemCache[count] = fitem;
                    count++;
                }
                return;
            }

            // starts and ends in file cache
            _FolderWatcher.WaitThreadsFinished(true);
            for (int index = start - dirCount; count < length; index++)
            {
                FileInfo finfo;
                ListViewItemEx fitem;

                if (index < _FolderWatcher.FileCache.Count)
                {
                    finfo = new FileInfo(_FolderWatcher.FileCache[index]);

                    fitem = new ListViewItemEx(finfo.Name);

                    if (finfo.Exists)
                    {
                        fitem.SubItems.Add(Helper.SizeSuffix(finfo.Length, 2));
                    }
                    else
                    {
                        fitem.SubItems.Add("DELETED");
                    }

                    fitem.SubItems.Add(finfo.FullName);
                }
                else
                {
                    fitem = new ListViewItemEx();
                    fitem.SubItems.Add("");
                    fitem.SubItems.Add("");
                }

                _ListViewItemCache[count] = fitem;
                count++;
            }
        }


        

        private void ListView1_RightClicked()
        {
            if (listView1.FocusedItem == null)
                return;

            if (listView1.SelectedIndices.Count < 0)
                return;

            contextMenuStrip1.Show(Cursor.Position);
        }

        private void TscbInterpolationMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Default_Interpolation_Mode = (System.Drawing.Drawing2D.InterpolationMode)tscbInterpolationMode.SelectedItem;
            imageDisplay1.InterpolationMode = InternalSettings.Default_Interpolation_Mode;
        }

        private void TscbDrawMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Default_Draw_Mode = (ImViewLite.Controls.DrawMode)tscbDrawMode.SelectedItem;
            imageDisplay1.DrawMode = InternalSettings.Default_Draw_Mode;
        }

        private void InputTextbox_TextChanged(object sender, EventArgs e)
        {
            if (_PreventOverflow)
                return;

            string text = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                LoadDirectory("", true);
            }

            if (PathHelper.IsValidDirectoryPath(text))
            {
                UpdateDirectory(text);
            }
        }

        

        private void _FolderWatcher_DirectoryAdded(string name)
        {
            this.listView1.InvokeSafe((Action)(() =>
            {
                this.listView1.VirtualListSize++;
                this.RefreshListView();
            }));
        }

        private void _FolderWatcher_FileAdded(string name)
        {
            this.listView1.InvokeSafe((Action)(() =>
            {
                this.listView1.VirtualListSize++;
                this.RefreshListView();
            }));
        }

        private void _FolderWatcher_DirectoryRemoved(string name)
        {
            this.listView1.InvokeSafe((Action)(() =>
            {
                this.listView1.VirtualListSize--;
                this.RefreshListView();
            }));
        }

        private void _FolderWatcher_FileRemoved(string name)
        {
            this.listView1.InvokeSafe((Action)(() =>
            {
                this.listView1.VirtualListSize--;
                this.RefreshListView();
            }));
        }

        private void _FolderWatcher_FileRenamed(string newName, string oldName)
        {
            this.InvokeSafe((Action)(() =>
            {
                this.RefreshListView();
            }));
        }

        private void _FolderWatcher_DirectoryRenamed(string newName, string oldName)
        {
            this.InvokeSafe((Action)(() =>
            {
                this.RefreshListView();
            }));
        }

        private void _FolderWatcher_ItemChanged(string name)
        {
            this.InvokeSafe((Action)(() =>
            {
                this.RefreshListView();
            }));
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = PathHelper.SelectFolderDialog(_CurrentDirectory);

            if (string.IsNullOrEmpty(dir))
                return;

            UpdateDirectory(dir, true);
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private void UpArrowButton_Click(object sender, EventArgs e)
        {
            UpDirectoryLevel();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameSelectedItems();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenSelectedItems();
            contextMenuStrip1.Close();
        }

        private void openWithDefaultProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSelectedItems();
        }

        private void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExplorerAtSelectedItems();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveSelectedFiles();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedFiles();
            contextMenuStrip1.Close();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            _IsUsingTextbox = true;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            _IsUsingTextbox = false;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedFiles();
        }

        private void pathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedPaths();
        }

        private void bytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedItemsSize(FileSizeUnit.Byte);
        }

        private void kilobytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedItemsSize(FileSizeUnit.Kilobyte);
        }

        private void megabytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedItemsSize(FileSizeUnit.Megabyte);
        }

        private void gigabytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedItemsSize(FileSizeUnit.Gigabyte);
        }

        private void terabytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedItemsSize(FileSizeUnit.Terabyte, 3);
        }

        private void petabyteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedItemsSize(FileSizeUnit.Petabyte, 5);
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                ClipboardHelper.CopyImageFromFile(listView1.FocusedItem.SubItems[2].Text);
            }
        }

        private void invertedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                ClipboardHelper.CopyImageFromFile(listView1.FocusedItem.SubItems[2].Text, ImageEffect.Invert);
            }
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                ClipboardHelper.CopyImageFromFile(listView1.FocusedItem.SubItems[2].Text, ImageEffect.Grayscale);
            }
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                ClipboardHelper.CopyImageFromFile(listView1.FocusedItem.SubItems[2].Text);
            }
        }

        private void widthXHeightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyDimensionsSelectedFiles("{0}x{1}");
        }

        private void dimensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyDimensionsSelectedFiles("{0}x{1}");
        }

        private void widthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyDimensionsSelectedFiles("{0}");
        }

        private void heightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyDimensionsSelectedFiles("{1}");
        }

        private void fileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedFileNames();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExecuteCommand(Command.OpenColorPicker);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteFiles(false);
        }

        public void PasteFiles(bool Move)
        {
            if (!ClipboardHelper.ContainsFileDropList())
                return;

            System.Collections.Specialized.StringCollection files = Clipboard.GetFileDropList();

            string pasteTarget = _CurrentDirectory;

            if (listView1.FocusedItem != null)
            {
                if (Directory.Exists(listView1.FocusedItem.SubItems[2].Text))
                {
                    pasteTarget = listView1.FocusedItem.SubItems[2].Text;
                }
            }

            foreach (string path in files)
            {
                string newPath = Path.Combine(pasteTarget, Path.GetFileName(path));
                //Console.WriteLine(path);
                if (Move)
                {
                    if (Directory.Exists(path))
                    {
                        PathHelper.MoveDirectory(path, newPath);
                    }
                    if (File.Exists(path))
                    {
                        PathHelper.MoveFile(path, newPath);
                    }
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        //Console.WriteLine("DIR");
                        PathHelper.CopyDirectory(path, newPath);
                    }
                    if (File.Exists(path))
                    {
                        PathHelper.CopyFile(path, newPath);
                    }
                }
            }
        }

        private void openWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem == null)
                return;

            using (OpenWithForm f = new OpenWithForm(listView1.FocusedItem.SubItems[2].Text))
            {
                f.ShowDialog();
            }
        }


        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {   
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }

        private void addCurrentFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string directory = this._CurrentDirectory;
            InternalSettings.Favorite_Directories.Add(directory);
            this.LoadFavoriteDirectories();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToggleAlwaysOnTop();
        }
    }
}
