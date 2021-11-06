using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImViewLite.Controls
{

    class ListViewItemEx : ListViewItem
    {

        public ListViewItemEx() : base()
        {
            
        }

        public ListViewItemEx(string text) : base(text)
        {

        }

        public ListViewItemEx(string[] items) : base(items)
        {

        }
    }
}
