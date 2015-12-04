using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Balansiq.DB.Entities;

namespace Balansiq.Windows.Controls.GridView
{
    public class DataGridViewIncomeRow : DataGridViewRow
    {
        private IncomeItem _item = null;
        private DateTime _date = DateTime.MinValue;
        public IncomeItem Item
        { 
            get { return _item; }
            set { _item = value; FillCellsWithValues(); }
        }
        public DateTime Date
        {
            get { return _date; }
            set 
            { 
                if (this._item != null)
                {
                    this._item.IDate = value.Date;
                }
                _date = value; 
            }
        }
        public bool RowIsEditing { get; protected set; }
        public event MoneyChangedEventHandler IncomeSummaryChanged;

        public DataGridViewIncomeRow()
            : base()
        {
            IncomeItem item = new IncomeItem();
            if (this.Date != DateTime.MinValue)
            {
                item.IDate = this.Date.Date;
            }
            this._item = item;
            RowIsEditing = false;
        }

        public void FillCellsWithValues()
        {
            if (this.Cells.Count == 4)
            {
                RowIsEditing = true;

                // fill date
                this.Cells[0].Value = this.Item.IDate.ToShortDateString();

                // fill combobox
                var filterCell = this.Cells[1] as DataGridViewComboBoxCell;
                if (filterCell != null)
                {
                    filterCell.Items.Clear();
                    filterCell.Items.AddRange(DB.DBManager.IncomeFilters.ToArray());
                    filterCell.ValueMember = "Id";
                    filterCell.DisplayMember = "Name";

                    if (this._item != null && this._item.IFilter.HasValue)
                    {
                        filterCell.Value = this._item.IFilter.Value;
                    }
                }

                if (this._item != null)
                {
                    // fill summary
                    this.Cells[2].Value = this._item.Summary;

                    // fill description
                    this.Cells[3].Value = this._item.Description;
                }

                RowIsEditing = false;
            }
        }

        public void UpdateCell(int columnIndex)
        {
            if (this._item == null)
            {
                this._item = new IncomeItem(this._date);
            }
            switch (columnIndex)
            {
                case 1:
                    this._item.IFilter = this.Cells[1].Value as long?;
                    break;

                case 2:
                    this._item.Summary = ParseNumericCell(this.Cells[2] as DataGridViewTextBoxCell);
                    this.Cells[2].Value = this._item.Summary;
                    if (IncomeSummaryChanged != null) IncomeSummaryChanged(this, new EventArgs());
                    break;

                case 3:
                    string description = this.Cells[3].Value as string;
                    this._item.Description = description != null ? description : string.Empty;
                    break;
            }
            if (!this.Item.IsEmpty)
            {
                if (!this.Item.Id.HasValue)
                {
                    if (!DB.DBManager.IncomeData.ContainsKey(this.Date.Date))
                    {
                        DB.DBManager.IncomeData.Add(this.Date.Date, new List<IncomeItem>());
                    }
                    DB.DBManager.IncomeData[this.Date.Date].Add(this.Item);
                }
                DB.DBManager.CreateOrUpdateItem(this.Item);
            }
        }

        private double ParseNumericCell(DataGridViewTextBoxCell cell)
        {
            double itemValue = .0;
            if (cell != null)
            {
                string strVal = cell.Value as string;
                double? dblVal = cell.Value as double?;
                if (strVal != null)
                {
                    double value;
                    if (double.TryParse(strVal, System.Globalization.NumberStyles.Currency, null, out value))
                    {
                        itemValue = Math.Abs(value);
                    }
                }
                else if (dblVal.HasValue)
                {
                    itemValue = dblVal.Value;
                }
            }
            return itemValue;
        }
    }
}
