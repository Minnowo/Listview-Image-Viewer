using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImViewLite.Helpers;


namespace ImViewLite.Controls
{
    class LISTVIEW : ListView
    {
        public ListViewItem LastSelectedItem;
        public int NewestSelectedIndex = -1;
        public int OldestSelectedIndex = -1;
        public SelectedIndexCollection SelectedIndexes;
        public LISTVIEW()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.EnableNotifyMessage, true);
            SelectedIndexes = this.SelectedIndices;
        }
        
        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            LastSelectedItem = e.Item;
            NewestSelectedIndex = e.ItemIndex;
            OldestSelectedIndex = e.ItemIndex;

            base.OnItemSelectionChanged(e);    
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    this.SelectedIndexes = this.SelectedIndices;
                    int count = this.SelectedIndices.Count;

                    if (count < 1)
                        return;
                    
                    if(this.OldestSelectedIndex == this.SelectedIndices[0])
                    {
                        this.NewestSelectedIndex = this.SelectedIndices[count - 1];
                    }
                    if (this.NewestSelectedIndex == this.SelectedIndices[0])
                    {
                        this.OldestSelectedIndex = this.SelectedIndices[count - 1];
                    }
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;
            }
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (this.SelectedIndexes.Count < 1)
                return;

            switch (e.KeyData)
            {
                case Keys.Shift | Keys.Down:
                    this.SelectedIndexes.Add(this.NewestSelectedIndex);
                    this.NewestSelectedIndex = (this.NewestSelectedIndex + 1).ClampMax(this.Items.Count);
                    break;

                case Keys.Shift | Keys.Up:
                    this.SelectedIndexes.Add(this.NewestSelectedIndex);
                    this.NewestSelectedIndex = (this.NewestSelectedIndex - 1).ClampMin(0);
                    break;
            }
        }
    }
}
