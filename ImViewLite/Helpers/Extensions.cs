using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ImViewLite.Settings;
//using ImageViewer.structs;

namespace ImViewLite.Helpers
{
    public static class Extensions
    {

        #region string Extensions

        public static string Truncate(this string str, int maxLength)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > maxLength)
            {
                return str.Substring(0, maxLength);
            }

            return str;
        }

        public static string[] OnlyValidFiles(this string[] str)
        {
            if (str == null || str.Length < 1)
                return null;
            List<string> newA = new List<string>();

            foreach(string path in str)
            {
                if (!Helper.IsValidFilePath(path) || !File.Exists(path))
                    continue;

                newA.Add(new FileInfo(path).FullName); // force absolute paths
            }
            return newA.ToArray();
        }

        #endregion

        #region Bitmap / Image Extensions

        public static Bitmap Copy(this Image image)
        {
            return ImageProcessor.DeepClone(image, PixelFormat.Format32bppArgb);
        }


        public static byte[] ToByteArray(this Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }

        #endregion

        #region byte[] Extensions

        public static string ReturnStrHash(this byte[] crypto)
        {
            StringBuilder hash = new StringBuilder();
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        #endregion

        

        public static IEnumerable<T> OrderByNatural<T>(this IEnumerable<T> items, Func<T, string> selector, StringComparer stringComparer = null)
        {
            Regex regex = InternalSettings.ReDigit;

            int maxDigits = items
                          .SelectMany(i => regex.Matches(selector(i)).Cast<Match>().Select(digitChunk => (int?)digitChunk.Value.Length))
                          .Max() ?? 0;

            return items.OrderBy(i => regex.Replace(selector(i), match => match.Value.PadLeft(maxDigits, '0')), stringComparer ?? StringComparer.CurrentCulture);
        }

        public static byte ToByte(this int input)
        {
            return (byte)input.Clamp(0, 255);
        }
    

        public static void InvokeSafe(this Control control, Action action)
        {
            if (control != null && !control.IsDisposed)
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        public static T Clamp<T>(this T input, T min, T max) where T : IComparable<T>
        {
            return MathHelper.Clamp(input, min, max);
        }

        public static T ClampMin<T>(this T input, T min) where T : IComparable<T>
        {
            return MathHelper.ClampMin(input, min);
        }

        public static T ClampMax<T>(this T input,T max) where T : IComparable<T>
        {
            return MathHelper.ClampMax(input, max);
        }

        public static T CloneSafe<T>(this T obj) where T : class, ICloneable
        {
            if (obj == null)
                return null;

            try
            {
                return obj.Clone() as T;
            }
            catch {}

            return null;
        }

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            Helper.Move(list, oldIndex, newIndex);
        }

        public static void Move<T>(this List<T> list, T item, int newIndex)
        {
            Helper.Move(list, item, newIndex);
        }
    }
}
