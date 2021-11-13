using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ImViewLite.Settings;

namespace ImViewLite.Helpers
{
    public static class PathHelper
    {
        public const string InvalidPathCharacters = "/\\:*?\"<>|";

        public static void CreateDirectory(string directoryPath)
        {
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch{}
            }
        }

        public static void CreateDirectoryFromFilePath(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                CreateDirectory(directoryPath);
            }
        }

        public static bool DeleteFileOrPath(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public static bool CopyDirectory(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return false;

            try
            {
                int len = from.Length;

                if (!to.EndsWith("\\"))
                    to += "\\";

                if (!from.EndsWith("\\"))
                    len += 1;

                CreateDirectory(to);
                foreach (string dirPath in Directory.GetDirectories(from, "*", SearchOption.AllDirectories))
                {
                    string newDir = to + dirPath.Substring(len);
                    Directory.CreateDirectory(newDir);
                }

                foreach (string filePath in Directory.GetFiles(from, "*.*", SearchOption.AllDirectories))
                {
                    string newPath = to + filePath.Substring(len);
                    File.Copy(filePath, newPath, true);
                }
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static bool CopyFile(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return false;
            try
            {
                File.Copy(from, to);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static bool MoveDirectory(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return false;

            try
            {
                Directory.Move(from, to);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static bool MoveFile(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return false;
            try
            {
                File.Move(from, to);
                return true;
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="DirectoryInfo"/> for the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>true if the <see cref="DirectoryInfo"/> does not throw an error, else false</returns>
        public static bool IsValidDirectoryPath(string path)
        {
            try
            {
                new DirectoryInfo(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidDirectoryPath(string path, out DirectoryInfo info)
        {
            try
            {
                info = new DirectoryInfo(path);
                return true;
            }
            catch
            {
                info = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to create a <see cref="FileInfo"/> for the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>true if the <see cref="FileInfo"/> does not throw an error, else false</returns>
        public static bool IsValidFilePath(string path)
        {
            try
            {
                new FileInfo(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidFilePath(string path, out FileInfo info)
        {
            try
            {
                info = new FileInfo(path);
                return true;
            }
            catch
            {
                info = null;
                return false;
            }
        }

        public static bool OpenWithDefaultProgram(string path)
        {
            if (!File.Exists(path))
                return false;

            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
            return true;
        }

        /// <summary>
        /// Opens explorer at the given file or directory.
        /// </summary>
        /// <param name="path">The path to open.</param>
        /// <returns></returns>
        public static bool OpenExplorerAtLocation(string path)
        {
            if (File.Exists(path))
            {
                Process fileopener = new Process();
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = string.Format("/select,\"{0}\"", path);
                fileopener.Start();
                return true;
            }
            else if (Directory.Exists(path))
            {
                Process fileopener = new Process();
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = path;
                fileopener.Start();
                return true;
            }
            return false;
        }

        public static async Task<string> SelectFolderDialogAsync(string title, string initialDirectory = "")
        {
            return await Task.Run(() => {
                using (FolderSelectDialog fsd = new FolderSelectDialog())
                {
                    fsd.Title = title;

                    if (!string.IsNullOrEmpty(initialDirectory))
                    {
                        fsd.InitialDirectory = initialDirectory;
                    }

                    if (fsd.ShowDialog())
                    {
                        return fsd.FileName;
                    }
                }
                return string.Empty;
            });
        }

        public static string SelectFolderDialog(string initialDirectory = "", string title=InternalSettings.Folder_Select_Dialog_Title)
        {
            using (FolderSelectDialog fsd = new FolderSelectDialog())
            {
                fsd.Title = title;
                
                if (!string.IsNullOrEmpty(initialDirectory))
                {
                    fsd.InitialDirectory = initialDirectory;
                }

                if (fsd.ShowDialog())
                {
                    return fsd.FileName;
                }
            }
            return string.Empty;
        }
    }
}
