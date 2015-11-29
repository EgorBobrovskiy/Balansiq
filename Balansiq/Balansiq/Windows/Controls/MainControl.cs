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
        private static MainWindow MainWindow;

        public static void InitComponents(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            InitCellTemplates();
            ApplyCellTemplates();
            MainWindow.datePicker.Value = DateTime.Now.Date;
            FillSpendFiltersTable();
            FillIncomeFiltersTable();
            FillSpendItemsTable();

            CalculateMoneyLeft(null, null);
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

        private static void ApplyCellTemplates()
        {
            MainWindow.spendFiltersGrid.RowsDefaultCellStyle = MainControl.CellTemplate;
            MainWindow.spendFiltersGrid.AlternatingRowsDefaultCellStyle = MainControl.CellTemplateAlt;

            MainWindow.incomeFiltersGrid.RowsDefaultCellStyle = MainControl.CellTemplate;
            MainWindow.incomeFiltersGrid.AlternatingRowsDefaultCellStyle = MainControl.CellTemplateAlt;

            MainWindow.spendGrid.RowsDefaultCellStyle = MainControl.CellTemplate;
            MainWindow.spendGrid.AlternatingRowsDefaultCellStyle = MainControl.CellTemplateAlt;

            MainWindow.incomeGrid.RowsDefaultCellStyle = MainControl.CellTemplate;
            MainWindow.incomeGrid.AlternatingRowsDefaultCellStyle = MainControl.CellTemplateAlt;
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

        public static void FillSpendFiltersTable()
        {
            DataGridView SFG = MainWindow.spendFiltersGrid;
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
            UpdateSpendFiltersTable(SFG);
        }

        public static void UpdateSpendFiltersTable(DataGridView SFG)
        {
            SFG.Rows.Clear();
            SFG.Columns.Clear();
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

        public static void FillIncomeFiltersTable()
        {
            DataGridView IFG = MainWindow.incomeFiltersGrid;
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
            UpdateIncomeFiltersTable(IFG);
        }

        public static void UpdateIncomeFiltersTable(DataGridView IFG)
        {
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

        public static void FillSpendItemsTable()
        {
            DataGridView SIT = MainWindow.spendGrid;
            DateTimePicker dateTimePicker = MainWindow.datePicker;

            dateTimePicker.ValueChanged += (o, e) => 
            {
                UpdateSpendItemsTable(SIT, dateTimePicker.Value);
            };
            SIT.RowsAdded += (o, e) =>
            {
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    var row = SIT.Rows[i] as DataGridViewSpendRow;
                    if (row != null)
                    {
                        row.DatePicker = dateTimePicker;
                        row.FillCellsWithValues();
                        row.TotalSpendChanged += CalculateMoneyLeft;
                    }
                }
            };

            var rowTemplate = new DataGridViewSpendRow();
            SIT.RowTemplate = rowTemplate;
            UpdateSpendItemsTable(SIT, dateTimePicker.Value);

            SIT.CellValueChanged += (o, e) =>
            {
                var table = o as DataGridView;
                if (table != null)
                {
                    var row = table.Rows[e.RowIndex] as DataGridViewSpendRow;
                    if (row != null && !row.RowIsEditing)
                        row.UpdateCell(e.ColumnIndex);
                }
            };
            SIT.UserDeletingRow += (o, e) =>
            {
                var row = e.Row as DataGridViewSpendRow;
                if (row != null && DB.DBManager.SpendData[row.Item.SDate.Date].Remove(row.Item))
                {
                    DB.DBManager.DeleteItem(row.Item);
                }
            };
        }

        public static void UpdateSpendItemsTable(DataGridView SIT, DateTime date)
        {
            SIT.Rows.Clear();
            List<SpendItem> spendItems;
            if (DBManager.SpendData.ContainsKey(date.Date))
                spendItems = DBManager.SpendData[date.Date];
            else
            {
                spendItems = new List<SpendItem>();
                DBManager.SpendData.Add(date.Date, spendItems);
            }

            foreach(SpendItem item in spendItems)
            {
                var row = SIT.Rows[SIT.Rows.Add()] as DataGridViewSpendRow;
                row.Item = item;
            }
        }

        private static void CalculateMoneyLeft(object sender, EventArgs eventArgs)
        {
            double income = DB.DBManager.IncomeData.Sum(date => date.Value.Sum(item => item.Summary));
            double spend = DB.DBManager.SpendData.Sum(date => date.Value.Sum(item => item.Price * item.Amount));
            MainWindow.moneyLeftLabel.Text = (income - spend).ToString("C2");
        }
    }

    public delegate void MoneyChangedEventHandler(object source, EventArgs eventArgs);
}
