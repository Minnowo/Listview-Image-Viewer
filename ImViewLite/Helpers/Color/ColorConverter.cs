using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    public class _ColorConverter : ColorConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
