using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImViewLite.Forms
{
    public partial class RemoveItemsFromListForm : Form
    {
        public object[] Items
        {
            get
            {
                int c = _Items.Count(_ => _ != null);
                if (c < 1)
                {
                    return null;
                }

                object[] items = new object[c];

                c = 0;
                foreach(object o in _Items)
                {
                    if (o == null)
                        continue;

                    items[c] = o;
                    c++;
                }

                return items;
            }
        }
        private object[] _Items;

        public RemoveItemsFromListForm(object[] items)
        {
            InitializeComponent();
            _Items = items;
            UpdateItems();
        }

        public void UpdateItems()
        {
            this.checkedListBox1.Controls.Clear();

            int c = 0;
            foreach(object item in _Items)
            {
                if (item == null)
                    continue;

                CheckBox cb = new CheckBox();
                cb.Dock = DockStyle.Top;
                cb.Text = item.ToString();
                cb.Tag = c;
                this.checkedListBox1.Controls.Add(cb);
                c++;
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            foreach(CheckBox cb in this.checkedListBox1.Controls)
            {
                if (cb.Checked)
                {
                    _Items[(int)cb.Tag] = null;
                }
            }
            UpdateItems();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
