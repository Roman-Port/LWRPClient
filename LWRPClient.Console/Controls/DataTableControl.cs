using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LWRPClient.Console.Controls
{
    class DataTableControl : TableLayoutPanel
    {
        private readonly Dictionary<int, Control[]> rows = new Dictionary<int, Control[]>();
        private int maxRow;

        public int RowHeight { get; set; } = 50;
        public SizeType RowSizeType { get; set; } = SizeType.Absolute;

        /// <summary>
        /// Gets controls on a row.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controls"></param>
        /// <returns></returns>
        public bool TryGetRow(int index, out Control[] controls)
        {
            return rows.TryGetValue(index, out controls);
        }

        /// <summary>
        /// Adds a row of controls to the table.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controls"></param>
        public void InsertRow(int index, params Control[] controls)
        {
            //Find all rows after this and shift them down on the layout panel
            foreach (var r in rows)
            {
                if (r.Key >= index)
                {
                    //Enumerate controls
                    foreach (var c in r.Value)
                    {
                        //Get current position
                        TableLayoutPanelCellPosition pos = GetCellPosition(c);

                        //Increment and set
                        int newRow = pos.Row + 1;
                        SetCellPosition(c, new TableLayoutPanelCellPosition
                        {
                            Column = pos.Column,
                            Row = newRow
                        });
                        maxRow = Math.Max(maxRow, newRow);
                    }
                }
            }

            //Add to dict
            rows.Add(index, controls);

            //Add controls and set their positions
            for (int x = 0; x < controls.Length; x++)
                Controls.Add(controls[x], x, index);
            maxRow = Math.Max(maxRow, index);

            //Configure row styles
            for (int i = 0; i < maxRow + 1; i++)
                ConfigureRowStyleAt(i, RowHeight, RowSizeType);
        }

        public void ClearRows()
        {
            //Delete all controls (and re-add dummy)
            Controls.Clear();

            //Delete all row styles
            RowStyles.Clear();

            //Reset parms
            rows.Clear();
            maxRow = 0;
        }

        private void ConfigureRowStyleAt(int index, int height, SizeType sizeType)
        {
            //Add if it's out of range
            if (index >= RowStyles.Count)
                RowStyles.Add(new RowStyle());

            //Set
            RowStyles[index].Height = height;
            RowStyles[index].SizeType = sizeType;
        }
    }
}
