using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Balansiq.DB.Entities;

namespace Balansiq.Pages.Controls.GridView
{
    public class DataGridViewSpendRow : DataGridViewRow
    {
        private SpendItem _item = null;
        private DateTime _date = DateTime.MinValue;
        public SpendItem Item
        {
            get { return _item; }
            set
            {
                _item = value;
                FillCellsWithValues();
            }
        }
        public DateTime Date 
        {
            get { return this._date; }
            set
            {
                if (this._item != null)
                {
                    this._item.SDate = value.Date;
                }
                this._date = value;
            }
        }
        public bool RowIsEditing { get; protected set; }
        public event MoneyChangedEventHandler TotalSpendChanged;

        public DataGridViewSpendRow()
            : base()
        {
            SpendItem item = new SpendItem();
            if (this.Date != DateTime.MinValue)
                item.SDate = this.Date.Date;
            this._item = item;
            RowIsEditing = false;
        }

        public void FillCellsWithValues()
        {
            if (this.Cells.Count == 7)
            {
                RowIsEditing = true;
                // fill date
                this.Cells[0].Value = this.Item.SDate.ToShortDateString();

                // fill comboboxes
                var filterTypeCell = this.Cells[1] as DataGridViewComboBoxCell;
                if (filterTypeCell != null)
                {
                    filterTypeCell.Items.Clear();
                    filterTypeCell.Items.AddRange(DB.DBManager.SpendFilters.Keys.ToArray());
                    filterTypeCell.ValueMember = "Id";
                    filterTypeCell.DisplayMember = "Name";
                
                    var filterCell = this.Cells[2] as DataGridViewComboBoxCell;
                    if (filterCell != null)
                    {
                        filterCell.ValueMember = "Id";
                        filterCell.DisplayMember = "Name";
                    }

                    if (this.Item != null && this.Item.SFilter.HasValue)
                    {
                        var spendFilter = DB.DBManager.GetSpendFilter(this.Item.SFilter.Value);
                        if (spendFilter != null && spendFilter.Type.HasValue)
                        {
                            filterTypeCell.Value = spendFilter.Type.Value;
                            UpdateFilterCell();
                            filterCell.Value = spendFilter.Id.Value;
                        }
                    }
                }

                if (this.Item != null)
                {
                    // fill description
                    this.Cells[3].Value = this.Item.Description;

                    // fill price
                    this.Cells[4].Value = this.Item.Price;

                    // fill amount
                    this.Cells[5].Value = this.Item.Amount;

                    // fill total
                    this.Cells[6].Value = this.Item.Amount * this.Item.Price;
                }
                RowIsEditing = false;
            }
        }

        public void UpdateCell(int columnIndex)
        {
            if (this._item == null)
            {
                this.Item = new SpendItem(this.Date.Date);
            }
            switch (columnIndex)
            {
                case 1:
                    UpdateFilterCell();
                    break;
                case 2:
                    this._item.SFilter = this.Cells[2].Value as long?;
                    break;
                case 3:
                    if (this.Cells[3].Value is string)
                        this.Item.Description = (string)this.Cells[3].Value;
                    break;
                case 4:
                    UpdatePriceCell();
                    break;
                case 5:
                    UpdateAmountCell();
                    break;
                case 6:
                    UpdateTotalCell();
                    break;
            }
            if (!this.Item.IsEmpty)
            {
                if (this.Item.Id == null)
                {
                    if (!DB.DBManager.SpendData.ContainsKey(this.Item.SDate.Date))
                    {
                        DB.DBManager.SpendData.Add(this.Item.SDate.Date, new List<SpendItem>());
                    }
                    DB.DBManager.SpendData[this.Item.SDate.Date].Add(this.Item);
                }
                DB.DBManager.CreateOrUpdateItem(this.Item);
            }
        }

        private void UpdateFilterCell()
        {
            var filterTypeCell = this.Cells[1] as DataGridViewComboBoxCell;
            var filterCell = this.Cells[2] as DataGridViewComboBoxCell;
            if (filterTypeCell != null && filterCell != null && filterTypeCell.Value is long)
            {
                filterCell.Value = null;
                filterCell.Items.Clear();
                filterCell.Items.AddRange(DB.DBManager.SpendFilters.Where(kvp => kvp.Key.Id == (long)filterTypeCell.Value).First().Value.ToArray());
            }
        }

        private void UpdatePriceCell()
        {
            double price,
                amount;
            ParseNumericCell(this.Cells[4] as DataGridViewTextBoxCell, out price);
            this.Item.Price = price;
            this.Cells[4].Value = price;
            if (this.Item.Amount > 0)
            {
                SetTotal();
            }
            else if (GetPrice(price, out amount, 3))
            {
                this.Item.Amount = amount;
                this.Cells[5].Value = amount;
            }
        }

        private void UpdateAmountCell()
        {
            double price, 
                amount;
            ParseNumericCell(this.Cells[5] as DataGridViewTextBoxCell, out amount);
            this.Item.Amount = amount;
            this.Cells[5].Value = amount;
            if (this.Item.Price > 0)
            {
                SetTotal();
            }
            else if (GetPrice(amount, out price, 2))
            {
                this.Item.Price = price;
                this.Cells[4].Value = price;
            }
        }

        private void UpdateTotalCell()
        {
            double price,
                amount,
                total;
            ParseNumericCell(this.Cells[6] as DataGridViewTextBoxCell, out total);
            this.Cells[6].Value = total;
            if (this.Item.Amount > 0)
            {
                if (GetPrice(this.Item.Amount, out price, 2))
                {
                    this.Item.Price = price;
                    this.Cells[4].Value = price;
                }
            }
            else if (this.Item.Price > 0)
            {
                if (GetPrice(this.Item.Price, out amount, 3))
                {
                    this.Item.Amount = amount;
                    this.Cells[5].Value = amount;
                }
            }
        }

        private void ParseNumericCell(DataGridViewTextBoxCell cell, out double itemValue)
        {
            itemValue = .0;
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
        }

        private void SetTotal()
        {
            double total = Math.Abs(this.Item.Price * this.Item.Amount);
            this.Cells[6].Value = total;
            if (TotalSpendChanged != null)
            {
                TotalSpendChanged(this, new EventArgs());
            }
        }

        private bool GetPrice(double amount, out double price, int round)
        {
            bool ret = false;
            price = .0;
            double? total = this.Cells[6].Value as double?;
            if (total.HasValue && amount > 0 && round > 0)
            {
                price = Math.Round(total.Value / amount, round);
                ret = true;
            }
            return ret;
        }
    }
}
