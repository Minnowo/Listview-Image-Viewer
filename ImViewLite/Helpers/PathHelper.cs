using System.IO;


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
                    Directory.Delete(path);
                    return true;
                }
            }
            catch
            {
                return false;
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
    }
}
