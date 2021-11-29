using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ImViewLite.Helpers;


namespace ImViewLite.Controls
{
    class LISTVIEW : ListView
    {
        public delegate void RightClickedEvent();
        public event RightClickedEvent RightClicked;


        public int NewestSelectedIndex = -1;
        public int OldestSelectedIndex = -1;
        public int SelectedItemsCount = 0;
        public ListViewItem LastSelectedItem;
        
        bool _IsLeftClick = false;
        bool _IsRightClick = false;

        public LISTVIEW()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.EnableNotifyMessage, true);
            this.AllowDrop = true;
        }
        
        public void DeselectAll()
        {
            this.SelectedIndices.Clear();
        }

        public string GetSelectedItemText2()
        {
            if (NewestSelectedIndex == -1)
                return string.Empty;

            if (Items.Count <= NewestSelectedIndex)
                return string.Empty;

            return Items[NewestSelectedIndex].SubItems[2].Text;
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);

            if (AllowDrop && e.Button == MouseButtons.Left)
            {
                string[] files = new string[this.SelectedIndices.Count];
                int c = 0;
                foreach(int i in this.SelectedIndices)
                {
                    files[c] = this.Items[i].SubItems[2].Text;
                    c++;
                }

                DoDragDrop(new DataObject(DataFormats.FileDrop, files), DragDropEffects.Move | DragDropEffects.Copy);
            }
        }
       
        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            LastSelectedItem = e.Item;
            NewestSelectedIndex = e.ItemIndex;
            OldestSelectedIndex = e.ItemIndex;
            base.OnItemSelectionChanged(e);    
        }

        
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    _IsLeftClick = false;
                    SelectedItemsCount = this.SelectedIndices.Count;

                    if (SelectedItemsCount < 1)
                        return;

                    if (this.OldestSelectedIndex == this.SelectedIndices[0])
                    {
                        this.NewestSelectedIndex = this.SelectedIndices[SelectedItemsCount - 1];
                    }
                    if (this.NewestSelectedIndex == this.SelectedIndices[0])
                    {
                        this.OldestSelectedIndex = this.SelectedIndices[SelectedItemsCount - 1];
                    }
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;
                case MouseButtons.Right:
                    _IsRightClick = false;
                    OnRightClick();
                    break;

                case MouseButtons.XButton1:
                    break;

                case MouseButtons.XButton2:
                    break;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    _IsLeftClick = true;
                    break;
                case MouseButtons.Right:
                    _IsRightClick = true;
                    break;
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            if (_IsLeftClick)
            {
                int count = this.SelectedIndices.Count;
                if (SelectedItemsCount != count)
                {
                    SelectedItemsCount = count;
                    if (count < 1)
                        return;
                    if (this.OldestSelectedIndex == this.SelectedIndices[0])
                    {
                        this.NewestSelectedIndex = this.SelectedIndices[SelectedItemsCount - 1];
                    }
                    if (this.NewestSelectedIndex == this.SelectedIndices[0])
                    {
                        this.OldestSelectedIndex = this.SelectedIndices[SelectedItemsCount - 1];
                    }
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            switch (e.KeyData)
            {
                case Keys.Up:
                case Keys.Down:
                    this.SelectedItemsCount = 1;
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;
                case Keys.Shift | Keys.Down:
                    SelectedItemsCount = this.SelectedIndices.Count;
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;

                case Keys.Shift | Keys.Up:
                    SelectedItemsCount = this.SelectedIndices.Count;
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyData)
            {
                case Keys.Up:
                case Keys.Down:
                    this.SelectedItemsCount = 1;
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;
                case Keys.Shift | Keys.Down:
                    this.SelectedIndices.Add(this.NewestSelectedIndex);
                    this.NewestSelectedIndex = (this.NewestSelectedIndex + 1).ClampMax(this.Items.Count);
                    break;

                case Keys.Shift | Keys.Up:
                    this.SelectedIndices.Add(this.NewestSelectedIndex);
                    this.NewestSelectedIndex = (this.NewestSelectedIndex - 1).ClampMin(0);
                    break;
            }
        }

        private void OnRightClick()
        {
            if (RightClicked != null)
                RightClicked();
        }
    }
}
