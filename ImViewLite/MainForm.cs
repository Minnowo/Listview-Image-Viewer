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
        delegate void UniversalVoidDelegate();

        public string CurrentDirectory
        {
            get { return _CurrentDirectory; }
            set
            {
                if (this._CurrentDirectory == value)
                    return;

                this.Text = value;
                this._CurrentDirectory = value;
                this.LoadDirectory(value);
            }
        }

        public ListViewItem SelectedItem;



        private string _CurrentDirectory = "";
        private int _CahceItem1;                     // stores the index of the first item in the cache
        private ListViewItemEx[] _ListViewItemCache; // array to cache items for the virtual list
        private FolderWatcher _FolderWatcher;

        private Timer _LoadImageTimer = new Timer() { Interval = 50 };
        private Regex _MatchFilePath = new Regex("\"(?<path>[^\"]*)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private bool _IsUsingTextbox = false;
        private bool _PreventIndexError = false;
        private bool _PreventOverflow = false;
        public MainForm()
        {
            InitializeComponent();
            SuspendLayout();
            this.imageDisplay1.CellScale = 1;
            this.imageDisplay1.CellSize = InternalSettings.Grid_Cell_Size;
            this.imageDisplay1.CellColor1 = InternalSettings.Image_Box_Back_Color;
            this.imageDisplay1.CellColor2 = InternalSettings.Image_Box_Back_Color_Alternate;
            this.imageDisplay1.DrawMode = ImViewLite.Controls.DrawMode.ScaleImage;

            tscbInterpolationMode.Items.Add(System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);
            tscbInterpolationMode.Items.Add(System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);
            tscbInterpolationMode.Items.Add(System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear);
            tscbInterpolationMode.SelectedItem = InternalSettings.Default_Interpolation_Mode;
            tscbInterpolationMode.SelectedIndexChanged += TscbInterpolationMode_SelectedIndexChanged;

            _FolderWatcher = new FolderWatcher();
            _FolderWatcher.FileRemoved += _FolderWatcher_FileRemoved;
            _FolderWatcher.DirectoryRemoved += _FolderWatcher_DirectoryRemoved;
            _FolderWatcher.FileAdded += _FolderWatcher_FileAdded;
            _FolderWatcher.DirectoryAdded += _FolderWatcher_DirectoryAdded;
            _FolderWatcher.DirectoryRenamed += _FolderWatcher_DirectoryRenamed;
            _FolderWatcher.FileRenamed += _FolderWatcher_FileRenamed;

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
            _LoadImageTimer.Tick += LoadImageTimer_Tick;

            this.textBox1.Text = "";
            this.TopMost = InternalSettings.Always_On_Top;
            this.KeyPreview = true;
            ResumeLayout();

            // empty string to just list all drives 
            _CurrentDirectory = "";
            LoadDirectory("");
        }

        

        public void RefreshListView()
        {
            this._ListViewItemCache = new ListViewItemEx[] { };
        }


        /// <summary>
        /// Updates variables on the mainfrom which reference the InternalSettings class
        /// </summary>
        public void UpdateSettings()
        {
            this.TopMost = InternalSettings.Always_On_Top;
            this.imageDisplay1.CellSize = InternalSettings.Grid_Cell_Size;
            this.imageDisplay1.CellColor1 = InternalSettings.Image_Box_Back_Color;
            this.imageDisplay1.CellColor2 = InternalSettings.Image_Box_Back_Color_Alternate;
            this.listView1.FullRowSelect = InternalSettings.Full_Row_Select;
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
                this.textBox1.Text = $"\"{path}\"";
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
                if (InternalSettings.Agressive_Image_Unloading)
                {
                    this.ReloadCurrentImage();
                }
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
            InternalSettings.Always_On_Top = !InternalSettings.Always_On_Top;
            this.TopMost = InternalSettings.Always_On_Top;
        }

        /// <summary>
        /// Opens the settings file dialog.
        /// </summary>
        public void OpenSettings()
        {
            using (SettingsForm sf = new SettingsForm())
            {
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
            cpf.Owner = this;
            cpf.StartPosition = FormStartPosition.CenterScreen;
            cpf.Show();
        }

        public void DeleteFile(string path)
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

        public void ReloadCurrentImage()
        {
            string newPath = listView1.GetSelectedItemText2();
            if(newPath != imageDisplay1.ImagePath)
            {
                LoadImage(newPath);
            }
        }

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

        public void CopySelectedFileNames()
        {
            CopySelectedItemText(0);
        }

        public void CopySelectedPaths()
        {
            CopySelectedItemText(2);
        }

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
            foreach(int ii in listView1.SelectedIndices)
            {
                files[i] = listView1.Items[ii].SubItems[2].Text;
                i++;
            }
            ClipboardHelper.CopyFile(files);            
        }

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

                bool tmp = InternalSettings.Ask_Delete_Confirmation_Single;
                InternalSettings.Ask_Delete_Confirmation_Single = false;

                foreach (int i in listView1.SelectedIndices)
                {
                    DeleteFile(listView1.Items[i].SubItems[2].Text);
                }
                InternalSettings.Ask_Delete_Confirmation_Single = tmp;
                return;
            }

            if (listView1.FocusedItem != null)
            {
                DeleteFile(listView1.FocusedItem.SubItems[2].Text);
            }
        }   

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

        public void MoveSelectedFiles(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                try
                {
                    path = PathHelper.SelectFolderDialog(InternalSettings.Folder_Select_Dialog_Title, Path.GetDirectoryName(CurrentDirectory));
                }
                catch
                {
                    path = PathHelper.SelectFolderDialog(InternalSettings.Folder_Select_Dialog_Title);
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

            if(args != null && args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                path = listView1.GetSelectedItemText2();
            }

            switch (cmd)
            {
                case Command.Nothing:               break;
                case Command.CopyImage:             imageDisplay1.CopyImage(); break;
                case Command.PauseGif:              imageDisplay1.AnimationPaused = !imageDisplay1.AnimationPaused; break;
                case Command.InvertColor:           imageDisplay1.InvertColor(); break;
                case Command.Grayscale:             imageDisplay1.ConvertGrayscale(); break;
                case Command.NextFrame:             imageDisplay1.NextImageFrame(); break;
                case Command.PreviousFrame:         imageDisplay1.PreviousImageFrame(); break;
                case Command.UpDirectoryLevel:      UpDirectoryLevel(); break;
                case Command.OpenSelectedDirectory: UpdateDirectory(path, true); break;
                case Command.MoveImage:             MoveSelectedFiles(path); break;
                case Command.RenameImage:           RenameSelectedItems(); break;
                case Command.DeleteImage:           DeleteSelectedItems(); break;
                case Command.ToggleAlwaysOnTop:     ToggleAlwaysOnTop(); break;
                case Command.OpenColorPicker:       OpenColorPicker(); break;
                case Command.OpenSettings:          OpenSettings(); break;
                case Command.OpenWithDefaultProgram:OpenSelectedItems(); break;
                case Command.OpenExplorerAtLocation:OpenExplorerAtSelectedItems(); break;
            }
        }


        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if(!_IsUsingTextbox)
            if (InternalSettings.CurrentUserSettings.Binds.ContainsKey(e.KeyData))
            {
                ExecuteCommand(
                    InternalSettings.CurrentUserSettings.Binds[e.KeyData].Function,
                    InternalSettings.CurrentUserSettings.Binds[e.KeyData].Args);
            }
        }

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

        // This variable is basically to pass args to the LoadImageTimer_Tick function
        // The purpose of the timer function is to prevent lag due to trying to load 50 images very quickly
        // So the timer puts a delay of 50ms before an image will be loaded, and in that time if another image is loaded
        // It will load that instead
        private string _Image_Delay = "";
        private void DelayLoadImage(string path)
        {
            // start the timer and set the queue
            _LoadImageTimer.Stop();
            _LoadImageTimer.Start();
            _Image_Delay = path;
        }

        private void LoadImageTimer_Tick(object sender, EventArgs e)
        {
            // time has passed, stop timer and load the image
            _LoadImageTimer.Stop();
            LoadImage(_Image_Delay);
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
                    //ditem.SelectionChanged += ListViewItem_Click;
                    ditem.SubItems.Add("");
                    ditem.SubItems.Add(dinfo.FullName);

                    e.Item = ditem;
                    return;
                }
                _FolderWatcher.WaitThreadsFinished();

                int index = e.ItemIndex - _FolderWatcher.DirectoryCache.Count;

                FileInfo finfo = new FileInfo(_FolderWatcher.FileCache[index]);
                ListViewItemEx fitem = new ListViewItemEx(finfo.Name);
                //fitem.SelectionChanged += ListViewItem_Click;
                if (finfo.Exists)
                {
                    fitem.SubItems.Add(Helper.SizeSuffix(finfo.Length, 2));
                }
                else
                {
                    fitem.SubItems.Add(Helper.SizeSuffix(0, 2));
                }
                fitem.SubItems.Add(finfo.FullName);

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
                    DirectoryInfo dinfo = new DirectoryInfo(_FolderWatcher.DirectoryCache[index]);
                    ListViewItemEx ditem = new ListViewItemEx(dinfo.Name);
                    //ditem.SelectionChanged += ListViewItem_Click;
                    ditem.SubItems.Add("");
                    ditem.SubItems.Add(dinfo.FullName);

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
                    DirectoryInfo dinfo = new DirectoryInfo(_FolderWatcher.DirectoryCache[index]);
                    ListViewItemEx ditem = new ListViewItemEx(dinfo.Name);
                    //ditem.SelectionChanged += ListViewItem_Click;
                    ditem.SubItems.Add("");
                    ditem.SubItems.Add(dinfo.FullName);

                    _ListViewItemCache[count] = ditem;
                    count++;
                }

                _FolderWatcher.WaitThreadsFinished(true);
                for (int index = 0; count < length; index++)
                {
                    FileInfo finfo = new FileInfo(_FolderWatcher.FileCache[index]);
                    ListViewItemEx fitem = new ListViewItemEx(finfo.Name);
                    //fitem.SelectionChanged += ListViewItem_Click;
                    if (finfo.Exists)
                    {
                        fitem.SubItems.Add(Helper.SizeSuffix(finfo.Length, 2));
                    }
                    else
                    {
                        fitem.SubItems.Add(Helper.SizeSuffix(0, 2));
                    }
                    fitem.SubItems.Add(finfo.FullName);

                    _ListViewItemCache[count] = fitem;

                    count++;
                }
                return;
            }

            _FolderWatcher.WaitThreadsFinished(true);
            // starts and ends in file cache
            for (int index = start - dirCount; count < length; index++)
            {
                FileInfo finfo = new FileInfo(_FolderWatcher.FileCache[index]);
                ListViewItemEx fitem = new ListViewItemEx(finfo.Name);
                //fitem.SelectionChanged += ListViewItem_Click;
                fitem.SubItems.Add(Helper.SizeSuffix(finfo.Length, 2));
                fitem.SubItems.Add(finfo.FullName);

                _ListViewItemCache[count] = fitem;
                count++;
            }
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

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


        private void InputTextbox_TextChanged(object sender, EventArgs e)
        {
            if (_PreventOverflow)
                return;

            string text = textBox1.Text;

            if(string.IsNullOrEmpty(text) || text == "\"\"")
            {
                LoadDirectory("", true);
            }

            Match m = _MatchFilePath.Match(text);

            if (m.Success)
            {
                text = m.Groups["path"].Value;

                UpdateDirectory(text);
            }
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private void _FolderWatcher_DirectoryAdded(string name)
        {
            this.listView1.InvokeSafe(() => {
                this.listView1.VirtualListSize++; 
                RefreshListView();
            });
        }

        private void _FolderWatcher_FileAdded(string name)
        {
            this.listView1.InvokeSafe(() => { 
                this.listView1.VirtualListSize++; 
                RefreshListView();
            });
        }

        private void _FolderWatcher_DirectoryRemoved(string name)
        {
            this.listView1.InvokeSafe(() => { 
                this.listView1.VirtualListSize--; 
                RefreshListView();

                if (InternalSettings.Agressive_Image_Unloading)
                    ReloadCurrentImage();
            });
        }

        private void _FolderWatcher_FileRemoved(string name)
        {
            this.listView1.InvokeSafe(() => { 
                this.listView1.VirtualListSize--; 
                RefreshListView();
                
                if (InternalSettings.Agressive_Image_Unloading)
                    ReloadCurrentImage();
            });

            
        }

        private void _FolderWatcher_FileRenamed(string newName, string oldName)
        {
            Console.WriteLine("refreshing listview");
            RefreshListView();
            listView1.Invalidate();
        }

        private void _FolderWatcher_DirectoryRenamed(string newName, string oldName)
        {
            RefreshListView();
            listView1.Invalidate();
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
            if(listView1.FocusedItem != null)
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
    }
}
