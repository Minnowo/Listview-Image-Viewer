using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ImViewLite.Helpers;
using ImViewLite.Settings;

namespace ImViewLite.Misc
{
    public class FolderWatcher : IDisposable
    {
        public delegate void FileAddedEvent(string name);
        public event FileAddedEvent FileAdded;

        public delegate void FileRemovedEvent(string name);
        public event FileRemovedEvent FileRemoved;

        public delegate void DirectoryAddedEvent(string name);
        public event DirectoryAddedEvent DirectoryAdded;

        public delegate void DirectoryRemovedEvent(string name);
        public event DirectoryRemovedEvent DirectoryRemoved;

        public delegate void FileRenamedEvent(string newName, string oldName);
        public event FileRenamedEvent FileRenamed;

        public delegate void DirectoryRenamedEvent(string newName, string oldName);
        public event DirectoryRenamedEvent DirectoryRenamed;

        public bool AboveDrives { get { return _AboveDrives; } }
        private bool _AboveDrives = false;

        public string CurrentDirectory
        {
            get
            {
                return directory;
            }
        }
        public List<string> DirectoryCache;        // list of sorted directories for the current directory
        public List<string> FileCache;             // list of sorted files for the current directory

        private Queue<RenamedEventArgs> _ItemRenamedQueue = new Queue<RenamedEventArgs>();
        private Queue<string> _ItemDeletedQueue = new Queue<string>();
        private Queue<string> _FileCreatedQueue = new Queue<string>();
        private Queue<string> _DirectoryCreatedQueue = new Queue<string>();

        // switched to using a timer and queue to prevent buffer overflow of the FileSystemWatcher
        // otherwise the FileSystemWatcher will skip / miss files that are deleted causing lots of issues
        // modified TIMER class allows for the use of the timer on non-ui threads
        private TIMER _EmptyItemRenamedQueueTimer = new TIMER() { Interval = 200 };
        private TIMER _EmptyItemDeletedQueueTimer = new TIMER() { Interval = 200 };
        private TIMER _EmptyFileCreatedQueueTimer = new TIMER() { Interval = 200 };
        private TIMER _EmptyDirectoryCreatedQueueTimer = new TIMER() { Interval = 200 };

        private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private string directory;

        private Task DirectorySortThread;
        private Task FileSortThread;
        public FolderWatcher()
        {
            _EmptyFileCreatedQueueTimer.Tick += _EmptyFileCreatedQueueTimer_Tick;
            _EmptyDirectoryCreatedQueueTimer.Tick += _EmptyDirectoryCreatedQueueTimer_Tick;
            _EmptyItemDeletedQueueTimer.Tick += _EmptyItemDeletedQueueTimer_Tick;
            _EmptyItemRenamedQueueTimer.Tick += _EmptyItemRenamedQueueTimer_Tick;
            directory = "";
            CreateWatchers(Directory.GetCurrentDirectory(), false);
            FileCache = new List<string>();
            DirectoryCache = new List<string>();
            UpdateDirectory("");
        }


        public FolderWatcher(string path)
        {
            _EmptyFileCreatedQueueTimer.Tick += _EmptyFileCreatedQueueTimer_Tick;
            _EmptyDirectoryCreatedQueueTimer.Tick += _EmptyDirectoryCreatedQueueTimer_Tick;
            _EmptyItemDeletedQueueTimer.Tick += _EmptyItemDeletedQueueTimer_Tick;
            _EmptyItemRenamedQueueTimer.Tick += _EmptyItemRenamedQueueTimer_Tick;
            directory = path;

            if (!Directory.Exists(path))
            {
                CreateWatchers(Directory.GetCurrentDirectory(), false);
                FileCache = new List<string>();
                DirectoryCache = new List<string>();
                return;
            }

            CreateWatchers(path, true);
            SetFiles(path);
        }

        private void _EmptyItemRenamedQueueTimer_Tick(object sender, EventArgs e)
        {
            _EmptyItemRenamedQueueTimer.Stop();

            WaitThreadsFinished();

            RenamedEventArgs ea;
            while(_ItemRenamedQueue.Count != 0)
            {
                ea = _ItemRenamedQueue.Dequeue();

                if (FileCache.Remove(ea.OldName))
                {
                    BinaryInsertFileCache(ea.Name,  false);
                    OnFileRenamed(ea.Name, ea.OldName);
                }

                if (DirectoryCache.Remove(ea.OldName))
                {
                    BinaryInsertDirectoryCache(ea.Name,  false);
                    OnDirectoryRenamed(ea.Name, ea.OldName);
                }
            }
        }

        private void _EmptyItemDeletedQueueTimer_Tick(object sender, EventArgs e)
        {
            _EmptyItemDeletedQueueTimer.Stop();

            WaitThreadsFinished();

            string name;
            while (_ItemDeletedQueue.Count != 0)
            {
                name = _ItemDeletedQueue.Dequeue();

                if (FileCache.Remove(name))
                {
                    OnFileRemoved(name);
                }
                if (DirectoryCache.Remove(name))
                {
                    OnDirectoryRemoved(name);
                }
            }
        }

        private void _EmptyFileCreatedQueueTimer_Tick(object sender, EventArgs e)
        {
            _EmptyFileCreatedQueueTimer.Stop();
            
            WaitThreadsFinished(true);
            
            while (_FileCreatedQueue.Count != 0)
            {
                BinaryInsertFileCache(_FileCreatedQueue.Dequeue());
            }
        }

        private void _EmptyDirectoryCreatedQueueTimer_Tick(object sender, EventArgs e)
        {
            _EmptyDirectoryCreatedQueueTimer.Stop();

            WaitThreadsFinished(false);

            while (_DirectoryCreatedQueue.Count != 0)
            {
                BinaryInsertDirectoryCache(_DirectoryCreatedQueue.Dequeue());
            }
        }

        /// <summary>
        /// blocks the thread until the Sortthread is completed
        /// </summary>
        public void WaitThreadsFinished(bool isFileThread)
        {
            if (isFileThread)
            {
                if (FileSortThread == null)
                    return;

                if (!FileSortThread.IsCompleted)
                {
                    FileSortThread.Wait();
                }
            }
            else
            {
                if (DirectorySortThread == null)
                    return;
                if (!DirectorySortThread.IsCompleted)
                    DirectorySortThread.Wait();
            }
        }

        public void WaitThreadsFinished()
        {
            WaitThreadsFinished(true);
            WaitThreadsFinished(false);
        }

        private int BinarySearchIndex(List<string> arr, string name)
        {
            int L = 0;
            int R = arr.Count;
            int mid;

            while(L < R)
            {
                mid = (L + R) / 2;

                if(Helper.StringCompareNatural(arr[mid], name) <= 0)
                {
                    L = mid + 1;
                }
                else
                {
                    R = mid;
                }
            }

            return L;
        }

        private void BinaryInsertFileCache(string name, bool fireEvent = true)
        {
            if (string.IsNullOrEmpty(name))
                return;

            int index = BinarySearchIndex(FileCache, name);
            FileCache.Insert(index, name);
            if(fireEvent)
                OnFileAdded(name);
        }


        private void BinaryInsertDirectoryCache(string name, bool fireEvent = true)
        {
            if (string.IsNullOrEmpty(name))
                return;

            int index = BinarySearchIndex(DirectoryCache, name);
            DirectoryCache.Insert(index, name);
            if(fireEvent)
                OnDirectoryAdded(name);
        }


        private void ItemRenamed(object sender, RenamedEventArgs e)
        {
            _ItemRenamedQueue.Enqueue(e);
            _EmptyItemRenamedQueueTimer.Stop();
            _EmptyItemRenamedQueueTimer.Start();
        }

        private void ItemCreated(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath))
            {
                _FileCreatedQueue.Enqueue(e.Name);
                _EmptyFileCreatedQueueTimer.Stop();
                _EmptyFileCreatedQueueTimer.Start();
            }
            else if (Directory.Exists(e.FullPath))
            {
                _DirectoryCreatedQueue.Enqueue(e.Name);
                _EmptyDirectoryCreatedQueueTimer.Stop();
                _EmptyDirectoryCreatedQueueTimer.Start();
            }
        }


        private void ItemDeleted(object sender, FileSystemEventArgs e)
        {
            _ItemDeletedQueue.Enqueue(e.Name);
            _EmptyItemDeletedQueueTimer.Stop();
            _EmptyItemDeletedQueueTimer.Start();
        }

        private void SetFiles(string path)
        {
            WaitThreadsFinished();

            FileSortThread = Task.Run(() => 
            {
                FileCache.Clear();
                foreach (string i in Directory.EnumerateFiles(path).OrderByNatural(e => e))
                {
                    FileCache.Add(Path.GetFileName(i));
                }
            });

            DirectorySortThread = Task.Run(() =>
            {
                DirectoryCache.Clear();
                foreach(string i in Directory.EnumerateDirectories(path).OrderByNatural(e => e))
                {
                    DirectoryCache.Add(Path.GetFileName(i));
                }
            });
        }


        private void CreateWatchers(string path, bool enabled = true)
        {
            FileSystemWatcher w = new FileSystemWatcher();
            w.Path = path;
            w.IncludeSubdirectories = false;
            w.NotifyFilter = NotifyFilters.FileName;
            w.Created += ItemCreated;
            w.Renamed += ItemRenamed;
            w.Deleted += ItemDeleted;
            w.EnableRaisingEvents = enabled;
            watchers.Add(w);
        }

       
        private void UpdateWatchers(string path, bool enable = true)
        {
            foreach (FileSystemWatcher fsw in watchers)
            {
                fsw.Path = path;
                fsw.EnableRaisingEvents = enable;
            }
        }

        public int GetTotalCount()
        {
            WaitThreadsFinished();
            return FileCache.Count + DirectoryCache.Count;
        }

        public int GetFileCount()
        {
            WaitThreadsFinished();
            return FileCache.Count;
        }

        public int GetDirectoryCount()
        {
            WaitThreadsFinished();
            return DirectoryCache.Count();
        }

        public void UpdateDirectory(string path)
        {
            _AboveDrives = false;
            directory = path;

            if (!Directory.Exists(path))
            {
                WaitThreadsFinished();
                
                DirectoryCache.Clear();
                FileCache.Clear();
                if (!string.IsNullOrEmpty(path))
                {
                    UpdateWatchers(path, false);
                }
                else
                {
                    foreach (DriveInfo di in DriveInfo.GetDrives())
                    {
                        DirectoryCache.Add(di.Name);
                    }
                    _AboveDrives = true;
                }
                return;
            }

            UpdateWatchers(path, true);
            SetFiles(directory);
        }

        private void OnFileAdded(string name)
        {
            if (FileAdded != null)
                FileAdded.Invoke(name);
        }

        private void OnFileRemoved(string name)
        {
            if (FileRemoved != null)
                FileRemoved.Invoke(name);
        }

        private void OnDirectoryAdded(string name)
        {
            if (DirectoryAdded != null)
                DirectoryAdded.Invoke(name);
        }

        private void OnDirectoryRemoved(string name)
        {
            if (DirectoryRemoved != null)
                DirectoryRemoved.Invoke(name);
        }

        private void OnFileRenamed(string newName, string oldName)
        {
            if (FileRenamed != null)
                FileRenamed.Invoke(newName, oldName);
        }

        private void OnDirectoryRenamed(string newName, string oldName)
        {
            if (DirectoryRenamed != null)
                DirectoryRenamed.Invoke(newName, oldName);
        }

        public void Dispose()
        {
            foreach(FileSystemWatcher fsw in watchers)
            {
                fsw.Created -= ItemCreated;
                fsw.Renamed -= ItemRenamed;
                fsw.Deleted -= ItemDeleted;
                fsw.Dispose();
            }

            this._EmptyFileCreatedQueueTimer.Tick -= _EmptyFileCreatedQueueTimer_Tick;
            this._EmptyItemDeletedQueueTimer.Tick -= _EmptyItemDeletedQueueTimer_Tick;
            this._EmptyDirectoryCreatedQueueTimer.Tick -= _EmptyDirectoryCreatedQueueTimer_Tick;
            this._EmptyItemRenamedQueueTimer.Tick -= _EmptyItemRenamedQueueTimer_Tick;
            this.watchers.Clear();
            
            WaitThreadsFinished();
            
            this._EmptyFileCreatedQueueTimer?.Dispose();
            this._EmptyItemDeletedQueueTimer?.Dispose();
            this._EmptyDirectoryCreatedQueueTimer?.Dispose();
            this._EmptyItemRenamedQueueTimer?.Dispose();
            this.FileCache.Clear();
            this.FileSortThread?.Dispose();
            this.DirectoryCache.Clear();
            this.DirectorySortThread?.Dispose();
            
            GC.SuppressFinalize(this);
        }
    }
}
