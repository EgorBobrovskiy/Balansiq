using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Balansiq.DB.Entities;

namespace Balansiq.Pages.Controls.GridView
{
    public class DataGridViewFilterTypeColumn : DataGridViewTextBoxColumn
    {
        public SpendFilterType FilterType { get; protected set; }
        public DataGridViewFilterTypeColumn(SpendFilterType item)
            : base()
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "SpendFilterType item must be not null!");
            }
            this.FilterType = item;
            this.HeaderText = item.Name;
            this.Name = item.Name;
            this.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.MinimumWidth = 50;
            this.Resizable = DataGridViewTriState.True;
            this.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.MaxInputLength = 128;
            this.CellTemplate = new DataGridViewFilterCell();
            if (item.Id > -1)
            {
                this.ContextMenuStrip = GetToolStrip();
            }
        }

        private ContextMenuStrip GetToolStrip()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            ToolStripButton changeColumnTSB = new ToolStripButton("Изменить столбец");
            ToolStripButton removeColumnTSB = new ToolStripButton("Удалить столбец");
            changeColumnTSB.Dock = DockStyle.Fill;
            removeColumnTSB.Dock = DockStyle.Fill;
            cms.Items.Add(changeColumnTSB);
            cms.Items.Add(removeColumnTSB);
            changeColumnTSB.Click += ChangeColumn_Click;
            removeColumnTSB.Click += RemoveColumn_Click;
            cms.AutoSize = true;
            return cms;
        }

        private void RemoveColumn_Click(object sender, EventArgs e)
        {
            string message = "Вы действительно хотите удалить выбранный столбец?\n" +
                "Действие необратимо, все фильтры будут помещены в \"Другое\".\n\n" +
                "Продолжить?";
            DialogResult res = MessageBox.Show(message, "Удаление типа расходов", MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {
                // move filters to others
                List<SpendFilter> others = DB.DBManager.SpendFilters.FirstOrDefault(k => k.Key.Id == -1).Value;
                foreach (var filter in DB.DBManager.SpendFilters[this.FilterType])
                {
                    filter.Type = -1;
                    others.Add(filter);
                    DB.DBManager.CreateOrUpdateItem(filter);
                }
                DB.DBManager.SpendFilters.Remove(this.FilterType);
                DB.DBManager.DeleteItem(this.FilterType);

                this.DataGridView.Columns.Remove(this);
            }
        }

        private void ChangeColumn_Click(object sender, EventArgs e)
        {
            string columnName = Microsoft.VisualBasic
                .Interaction.InputBox("Введите новое название:", "Изменить название фильтра", this.Name);
            if (!string.IsNullOrWhiteSpace(columnName))
            {
                this.Name = columnName;
                this.HeaderText = columnName;
                this.FilterType.Name = columnName;
                DB.DBManager.CreateOrUpdateItem(this.FilterType);
            }
        }
    }
}
