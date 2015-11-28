using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using Balansiq.DB;
using Balansiq.DB.Entities;
using Balansiq.Windows.Controls.GridView;

namespace Balansiq.Windows.Controls
{
    public abstract class MainControl
    {
        public static DataGridViewCellStyle CellTemplate { get; protected set; }
        public static DataGridViewCellStyle CellTemplateAlt { get; protected set; }

        public static void InitComponents()
        {
            InitCellTemplates();
        }

        private static void InitCellTemplates()
        {
            CellTemplate = new DataGridViewCellStyle();
            CellTemplate.Alignment = DataGridViewContentAlignment.MiddleLeft;
            CellTemplate.WrapMode = DataGridViewTriState.True;
            CellTemplate.Font = new Font("Microsoft Sans Serif", 10);
            CellTemplate.BackColor = Color.White;
            CellTemplate.ForeColor = Color.Black;
            CellTemplate.SelectionBackColor = Color.FromArgb(192, 255, 192);
            CellTemplate.SelectionForeColor = Color.Black;

            CellTemplateAlt = new DataGridViewCellStyle(CellTemplate);
            CellTemplateAlt.BackColor = Color.FromArgb(224, 224, 224);
        }

        public static DataGridViewFilterTypeColumn GetFilterTypeColumn(SpendFilterType spendFilterType)
        {
            return spendFilterType == null ? null : new DataGridViewFilterTypeColumn(spendFilterType);
        }

        public static DataGridViewFilterTypeColumn GetFilterTypeColumn(string columnName)
        {
            DataGridViewFilterTypeColumn column = null;
            if (columnName != null && columnName != string.Empty)
            {
                // add in DB
                SpendFilterType filterType = new SpendFilterType(columnName);
                DBManager.CreateOrUpdateItem(filterType);
                DBManager.SpendFilters.Add(filterType, new List<SpendFilter>());

                column = GetFilterTypeColumn(filterType);
            }
            return column;
        }

        public static void FillSpendFiltersTable(DataGridView SFG)
        {
            SFG.ColumnAdded += (o, e) => e.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            SFG.RowsAdded += (o, e) =>
            {
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    foreach (DataGridViewFilterCell cell in SFG.Rows[i].Cells)
                    {
                        cell.ItemType = typeof(SpendFilter);
                    }
                }
            };
            foreach (var filter in DBManager.SpendFilters)
            {
                var column = new DataGridViewFilterTypeColumn(filter.Key);
                SFG.Columns.Add(column);

                int iRow = filter.Value.Count - SFG.Rows.Count + 1;
                if (iRow > 0)
                {
                    SFG.Rows.Add(iRow);
                }
                for (iRow = 0; iRow < filter.Value.Count; iRow++)
                {
                    var row = SFG.Rows[iRow];
                    var cell = row.Cells[column.Name];
                    cell.Value = filter.Value[iRow];
                }
            }
        }

        public static void FillIncomeFiltersTable(DataGridView IFG)
        {
            IFG.RowsAdded += (o, e) =>
            {
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    foreach (DataGridViewFilterCell cell in IFG.Rows[i].Cells)
                    {
                        cell.ItemType = typeof(IncomeFilter);
                    }
                }
            };
            IFG.Columns.Clear();
            IFG.Rows.Clear();

            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column.MinimumWidth = 50;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.MaxInputLength = 128;
            column.CellTemplate = new DataGridViewFilterCell();
            IFG.Columns.Add(column);

            for (int i = 0; i < DBManager.IncomeFilters.Count; i++)
            {
                IFG.Rows.Add();
                if (IFG.Rows[i].Cells.Count > 0)
                {
                    var cell = IFG.Rows[i].Cells[0].Value = DBManager.IncomeFilters[i];
                }
            }
        }
    }
}
