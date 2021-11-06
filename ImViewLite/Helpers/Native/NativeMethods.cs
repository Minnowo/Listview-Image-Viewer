using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ImViewLite.Native
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
    }
}
