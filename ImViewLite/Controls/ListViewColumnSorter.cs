using System.Collections;
using System.Windows.Forms;
using System;

namespace ImViewLite.Controls
{

    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    /*public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;

        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor. Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface. It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItemEx listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItemEx)x;
            listviewY = (ListViewItemEx)y;

            if (listviewX.SortKey == -1 && listviewY.SortKey == -1)
            {
                compareResult = ObjectCompare.Compare(
                    listviewX.SubItems[ColumnToSort].Text, 
                    listviewY.SubItems[ColumnToSort].Text);
                
                if (OrderOfSort == SortOrder.Ascending)
                    return compareResult;
                else if (OrderOfSort == SortOrder.Descending)
                    return -compareResult;
                else
                    return 0;
            }
            else
            {
                compareResult = ObjectCompare.Compare(
                    listviewY.SortKeyStr + listviewY.SubItems[ColumnToSort].Text, 
                    listviewX.SortKeyStr + listviewX.SubItems[ColumnToSort].Text);


                if (listviewX.SortKey > listviewY.SortKey)
                    return OrderOfSort == SortOrder.Ascending ? compareResult : -compareResult;
                else if (listviewX.SortKey < listviewY.SortKey)
                    return OrderOfSort == SortOrder.Ascending ? -compareResult : compareResult;
                else
                    return OrderOfSort == SortOrder.Ascending ? -compareResult : compareResult;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

    }*/
}
