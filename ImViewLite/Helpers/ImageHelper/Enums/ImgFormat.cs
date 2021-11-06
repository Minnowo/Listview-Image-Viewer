using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    public enum ImgFormat
    {
        png,
        jpg,
        tif,
        bmp,
        gif,
        wrm,
        webp,

        // can only read ico images
        [Browsable(false)] 
        ico,
        [Browsable(false)]
        nil = -1
    }
}
