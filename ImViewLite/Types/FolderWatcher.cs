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

        public string CurrentDirectory
        {
            get
            {
                return directory;
            }
        }
        public List<string> DirectoryCache;        // list of sorted directories for the current directory
        public List<string> FileCache;             // list of sorted files for the current directory

        private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private string directory;

        private Task DirectorySortThread;
        private Task FileSortThread;

        public FolderWatcher()
        {
            directory = "";
            CreateWatchers(Directory.GetCurrentDirectory(), false);
            FileCache = new List<string>();
            DirectoryCache = new List<string>();
        }

        public FolderWatcher(string path)
        {
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

        private int GetMaxFileLength()
        {
            WaitThreadsFinished(true);

            return FileCache.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length;
        }

        private int GetMaxDirectoryLength()
        {
            WaitThreadsFinished(false);

            return DirectoryCache.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length;
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
                    FileSortThread.Wait();
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


        private void InsertProperPositionFile(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            FileSortThread = Task.Run(() => {
                int i;
                for (i = 0; i < FileCache.Count; i++)
                {
                    if (string.IsNullOrEmpty(FileCache[i]))
                        continue;

                    if (Helper.StringCompareNatural(FileCache[i], name) >= 0)
                    {
                        break;
                    }
                }
                FileCache.Insert(i, name);
                OnFileAdded(name);
            });
        }

        private void InsertProperPositionDirectory(string name)
        {
            DirectorySortThread = Task.Run(() => {
                int i;
                for (i = 0; i < DirectoryCache.Count; i++)
                {
                    if (Helper.StringCompareNatural(DirectoryCache[i], name) >= 0)
                    {
                        break;
                    }
                }
                DirectoryCache.Insert(i, name);
                OnDirectoryAdded(name);
            });
        }

        private void ItemRenamed(object sender, RenamedEventArgs e)
        {
            WaitThreadsFinished();
            int fileRenamed = 0;
            int directoryRenamed = 0;

            if (FileCache.Remove(e.OldName))
            {
                OnFileRemoved(e.Name);
                fileRenamed++;
            }

            if (DirectoryCache.Remove(e.OldName))
            {
                OnDirectoryRemoved(e.Name);
                directoryRenamed++;
            }

            if (File.Exists(e.Name))
            {
                InsertProperPositionFile(e.Name);
                fileRenamed++;
            }
            if (Directory.Exists(e.Name))
            {
                InsertProperPositionDirectory(e.Name);
                directoryRenamed++;
            }

            if (fileRenamed == 2)
            {
                OnFileRenamed(e.Name, e.OldName);
            }

            if(directoryRenamed == 2)
            {
                OnDirectoryRenamed(e.Name, e.OldName);
            }
        }

        private void FolderChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.ChangeType);
            switch (e.ChangeType)
            {
                // no need to remove from list we will check if the file exists when fetching
                case WatcherChangeTypes.Deleted:
                    WaitThreadsFinished();

                    if (FileCache.Remove(e.Name))
                    {
                        OnFileRemoved(e.Name);
                    }

                    if (DirectoryCache.Remove(e.Name))
                    {
                        OnDirectoryRemoved(e.Name);
                    }
                    
                    break;

                // using a timer because i don't want to waste cpu resorting the files if lots of files 
                // are being created / copied
                case WatcherChangeTypes.Created:
                    if (File.Exists(e.Name))
                    {
                        InsertProperPositionFile(e.Name);
                    }
                    else if (Directory.Exists(e.FullPath))
                    {
                        InsertProperPositionDirectory(e.Name);
                    }
                    break;
            }
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
                // FileCache = Directory.EnumerateFiles(path).OrderByNatural(e => e).ToList();
            });

            DirectorySortThread = Task.Run(() =>
            {
                DirectoryCache.Clear();
                foreach(string i in Directory.EnumerateDirectories(path).OrderByNatural(e => e))
                {
                    DirectoryCache.Add(Path.GetFileName(i));
                }
                //DirectoryCache = Directory.EnumerateDirectories(path).OrderByNatural(e => e).ToList();
            });
        }


        private void CreateWatchers(string path, bool enabled = true)
        {
            FileSystemWatcher w = new FileSystemWatcher();
            w.Path = path;
            w.IncludeSubdirectories = false;
            w.Created += FolderChanged;
            w.Renamed += ItemRenamed;
            w.Deleted += FolderChanged;
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
            directory = path;

            if (!Directory.Exists(path))
            {
                WaitThreadsFinished();
                UpdateWatchers(path, false);
                DirectoryCache.Clear();
                FileCache.Clear();
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
                fsw.Created -= FolderChanged;
                fsw.Renamed -= ItemRenamed;
                fsw.Deleted -= FolderChanged;
                fsw.Dispose();
            }

            this.watchers.Clear();
            
            WaitThreadsFinished();
            
            this.FileCache.Clear();
            this.FileSortThread?.Dispose();
            this.DirectoryCache.Clear();
            this.DirectorySortThread?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
