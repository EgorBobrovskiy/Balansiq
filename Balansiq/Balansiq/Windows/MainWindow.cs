using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Balansiq.DB;
using Balansiq.DB.Entities;
using Balansiq.Windows.Controls;

namespace Balansiq
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                DBConnector.OpenConnection();
                DBManager.GetAllItems();
                MainControl.InitComponents();

                // default values
                this.datePicker.Value = DateTime.Now;
                ApplyCellFormats();
                FillTablesWithValues();

                // bind events
                this.FormClosing += OnClosing;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nБудь у меня искусственный интеллект, я бы уже исправил ошибку :(", "Упс... Возникла проблема!");
                Load += (s, e) => Close();
            }
        }

        private void ApplyCellFormats()
        {
            this.spendFiltersGrid.RowsDefaultCellStyle = MainControl.CellTemplate;
            this.spendFiltersGrid.AlternatingRowsDefaultCellStyle = MainControl.CellTemplateAlt;

            this.incomeFiltersGrid.RowsDefaultCellStyle = MainControl.CellTemplate;
            this.incomeFiltersGrid.AlternatingRowsDefaultCellStyle = MainControl.CellTemplateAlt;

            this.spendGrid.RowsDefaultCellStyle = MainControl.CellTemplate;
            this.spendGrid.AlternatingRowsDefaultCellStyle = MainControl.CellTemplateAlt;

            this.incomeGrid.RowsDefaultCellStyle = MainControl.CellTemplate;
            this.incomeGrid.AlternatingRowsDefaultCellStyle = MainControl.CellTemplateAlt;
        }

        private void FillTablesWithValues()
        {
            MainControl.FillSpendFiltersTable(this.spendFiltersGrid);
            MainControl.FillIncomeFiltersTable(this.incomeFiltersGrid);
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DBConnector.CloseConnection();
        }

        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filterName = Microsoft.VisualBasic
                .Interaction.InputBox("Введите название нового типа фильтров:", "Добавить столбец", "Новый фильтр");
            if (!string.IsNullOrWhiteSpace(filterName))
            {
                var dColumn = MainControl.GetFilterTypeColumn(filterName);
                this.spendFiltersGrid.Columns.Add(dColumn);
            }
        }
    }
}
