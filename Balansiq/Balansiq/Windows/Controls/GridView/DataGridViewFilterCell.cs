using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Balansiq.DB.Entities;

namespace Balansiq.Windows.Controls.GridView
{
    public class DataGridViewFilterCell : DataGridViewTextBoxCell
    {
        private Filter _item = null;
        private Type _itemType = null;
        public Filter Item
        {
            get { return _item; }
        }
        public Type ItemType
        {
            get
            {
                return _itemType;
            }
            set
            {
                if (value == typeof(IncomeFilter) || value == typeof(SpendFilter))
                {
                    _itemType = value;
                }
            }
        }
        public bool IsEmpty { get { return !(this._item is SpendFilter); } }
        public DataGridViewFilterCell()
            : base()
        {
            this.ValueType = typeof(string);
        }
        protected override bool SetValue(int rowIndex, object value)
        {
            bool ret = false;
            string str = value as string;
            if (str != null)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    // delete item
                    string message = "Вы действительно хотите удалить фильтр?\n" +
                        "Все связанные с ним данные будут недоступны при анализе.\n\n" +
                        "Продожить?";
                    DialogResult res = MessageBox.Show(message, "Удаление фильтра", MessageBoxButtons.OKCancel);
                    if (res == DialogResult.OK)
                    {
                        DB.DBManager.DeleteItem(this._item);
                        this._item = null;
                    }
                    ret = true;
                }
                else if (this._item == null)
                {
                    // create new item
                    if (_itemType == typeof(IncomeFilter))
                    {
                        this._item = new IncomeFilter(str);
                    }
                    else if (_itemType == typeof(SpendFilter))
                    {
                        var col = this.OwningColumn as DataGridViewFilterTypeColumn;
                        this._item = new SpendFilter(str, col != null ? col.FilterType.Id : null);
                    }
                    DB.DBManager.CreateOrUpdateItem(this._item);
                    ret = true;
                }
                else
                {
                    // update existing
                    this._item.Name = str;
                    DB.DBManager.CreateOrUpdateItem(this._item);
                    ret = true;
                }
            }
            else if (value is Filter)
            {
                this._item = (Filter)value;
                ret = true;
            }
            if (ret) RaiseCellValueChanged(new DataGridViewCellEventArgs(this.ColumnIndex, this.RowIndex));
            return ret;
        }
        protected override object GetValue(int rowIndex)
        {
            return this._item != null ? this._item.Name : null;
        }
    }
}
