﻿using System;
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

        private Regex _MatchFilePath = new Regex("\"(?<path>[^\"]*)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private bool preventOverflow = false;
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

            this.listView1.VirtualMode = true;
            this.listView1.VirtualListSize = 0;
            this.listView1.Sorting = SortOrder.None;

            listView1.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(listView1_RetrieveVirtualItem);
            listView1.CacheVirtualItems += new CacheVirtualItemsEventHandler(listView1_CacheVirtualItems);
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            listView1.ItemActivate += ListView1_ItemActivate;

            this.KeyUp += MainForm_KeyUp;

            this.textBox1.Text = "\"D:\\pictures\\Anime\"";
            this.TopMost = InternalSettings.Always_On_Top;
            this.KeyPreview = true;
            ResumeLayout();
        }

        private void _FolderWatcher_DirectoryAdded(string name)
        {
            this.listView1.Invoke(new UniversalVoidDelegate(() => { this.listView1.VirtualListSize++; }));
            this._ListViewItemCache = null;
        }

        private void _FolderWatcher_FileAdded(string name)
        {
            this.listView1.Invoke(new UniversalVoidDelegate(() => { this.listView1.VirtualListSize++; }));
            this._ListViewItemCache = null;
        }

        private void _FolderWatcher_DirectoryRemoved(string name)
        {
            this.listView1.Invoke(new UniversalVoidDelegate(() => { this.listView1.VirtualListSize--; }));
            this._ListViewItemCache = null;
        }

        private void _FolderWatcher_FileRemoved(string name)
        {
            this.listView1.Invoke(new UniversalVoidDelegate(() => { this.listView1.VirtualListSize--; }));
            this._ListViewItemCache = null;
        }

        public void ExecuteCommand(Command cmd, string[] args = null)
        {
            switch (cmd)
            {
                case Command.Nothing:
                    break;
                case Command.CopyImage:
                    if (this.imageDisplay1.Image != null)
                        ClipboardHelper.CopyImage(this.imageDisplay1.Image);
                    break;
                case Command.UpDirectoryLevel:
                    DirectoryInfo info = new DirectoryInfo(this.CurrentDirectory);
                    if (info.Parent != null)
                        this.UpdateDirectory(info.Parent.FullName, true);
                    break;
                case Command.OpenSelectedDirectory:
                    string dir = listView1.Items[listView1.NewestSelectedIndex].SubItems[2].Text;
                    Console.WriteLine(dir);
                    UpdateDirectory(dir, true);
                    break;
                case Command.PauseGif:
                case Command.NextFrame:
                case Command.PreviousFrame:
                case Command.NextImage:
                case Command.PreviousImage:
                case Command.ToggleAlwaysOnTop:
                case Command.OpenNewInstance:
                case Command.MoveImage:
                    string file = listView1.Items[listView1.NewestSelectedIndex].SubItems[2].Text;
                    if (!File.Exists(file))
                        return;

                    if (args != null && args.Length > 0 && Directory.Exists(args[0]))
                    {
                        Console.WriteLine($"moving: {file} {args[0]}");
                        PathHelper.MoveFile(file, Path.Combine(args[0], Path.GetFileName(file)));
                    }
                    else
                    {
                        PathHelper.MoveFile(file, Helper.MoveFileDialog(file));
                    }

                    break;
                case Command.RenameImage:
                case Command.DeleteImage:
                case Command.InvertColor:
                case Command.Grayscale:
                case Command.OpenColorPicker:
                case Command.OpenSettings:
                    break;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (InternalSettings.CurrentUserSettings.Binds.ContainsKey(e.KeyData))
                ExecuteCommand(
                    InternalSettings.CurrentUserSettings.Binds[e.KeyData].Function,
                    InternalSettings.CurrentUserSettings.Binds[e.KeyData].Args);

        }

        public void UpdateSettings()
        {
            this.TopMost = InternalSettings.Always_On_Top;
            this.imageDisplay1.CellSize = InternalSettings.Grid_Cell_Size;
            this.imageDisplay1.CellColor1 = InternalSettings.Image_Box_Back_Color;
            this.imageDisplay1.CellColor2 = InternalSettings.Image_Box_Back_Color_Alternate;
        }

        public void LoadDirectory(string path, bool update = false)
        {
            if (_CurrentDirectory != path && !update)
            {
                CurrentDirectory = path;
                return;
            }

            _FolderWatcher.UpdateDirectory(path);
            Console.WriteLine(_FolderWatcher.CurrentDirectory);
            this.listView1.VirtualListSize = _FolderWatcher.GetTotalCount();
            this._ListViewItemCache = null;

            this.listView1.Invalidate();
            GC.Collect();
        }

        public void UpdateDirectory(string path, bool updateTextbox = false)
        {
            if (Directory.Exists(path))
            {
                this.preventOverflow = true;
                path = new DirectoryInfo(path).FullName;

                if (path[path.Length - 1] != '\\')
                    path += '\\';

                this.CurrentDirectory = path;

                if (updateTextbox)
                {
                    this.textBox1.Text = $"\"{path}\"";
                }
                this.preventOverflow = false;
            }
        }


        public void UpdateAfterIndexChanged()
        {
            string path = listView1.Items[listView1.NewestSelectedIndex].SubItems[2].Text;


            tsslItemOfItems.Text = $"{Math.Abs(listView1.OldestSelectedIndex - listView1.NewestSelectedIndex) + 1} / {listView1.Items.Count} object(s) selected";
            tsslFileSize.Text = "";
            tsslFilePath.Text = "";
            tsslImageSize.Text = "";

            if (Directory.Exists(path))
            {
                DirectoryInfo dinfo = new DirectoryInfo(path);
                tsslFilePath.Text = dinfo.FullName;
                statusStrip1.ResumeLayout();
                return;
            }

            if (File.Exists(path))
            {
                FileInfo finfo = new FileInfo(path);

                tsslFileSize.Text = Helper.SizeSuffix(finfo.Length);
                tsslFilePath.Text = finfo.FullName;

                if (imageDisplay1.TryLoadImage(path))
                {
                    tsslImageSize.Text = $"{imageDisplay1.Image.Width} x {imageDisplay1.Image.Height}";
                }
            }

        }

        private void ListView1_ItemActivate(object sender, EventArgs e)
        {
            if (preventOverflow || listView1.NewestSelectedIndex == -1)
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
                    DirectoryInfo dinfo = new DirectoryInfo(_FolderWatcher.DirectoryCache[e.ItemIndex]);
                    ListViewItemEx ditem = new ListViewItemEx(dinfo.Name);
                    //ditem.SelectionChanged += ListViewItem_Click;
                    ditem.SubItems.Add("");
                    ditem.SubItems.Add(dinfo.FullName);

                    e.Item = ditem;
                    return;
                }

                int index = e.ItemIndex - _FolderWatcher.DirectoryCache.Count;

                FileInfo finfo = new FileInfo(_FolderWatcher.FileCache[index]);
                ListViewItemEx fitem = new ListViewItemEx(finfo.Name);
                //fitem.SelectionChanged += ListViewItem_Click;
                fitem.SubItems.Add(Helper.SizeSuffix(finfo.Length, 2));
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

                for (int index = 0; count < length; index++)
                {
                    FileInfo finfo = new FileInfo(_FolderWatcher.FileCache[index]);
                    ListViewItemEx fitem = new ListViewItemEx(finfo.Name);
                    //fitem.SelectionChanged += ListViewItem_Click;
                    fitem.SubItems.Add(Helper.SizeSuffix(finfo.Length, 2));
                    fitem.SubItems.Add(finfo.FullName);

                    _ListViewItemCache[count] = fitem;

                    count++;
                }
                return;
            }

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

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void TscbInterpolationMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageDisplay1.InterpolationMode = (System.Drawing.Drawing2D.InterpolationMode)tscbInterpolationMode.SelectedItem;
        }


        private void InputTextbox_TextChanged(object sender, EventArgs e)
        {
            if (preventOverflow)
                return;

            string text = textBox1.Text;

            Match m = _MatchFilePath.Match(text);

            if (m.Success)
            {
                text = m.Groups["path"].Value;

                UpdateDirectory(text);
            }
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new SettingsForm();
            sf.ShowDialog();
            UpdateSettings();
        }
    }
}
