using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Balansiq.DB;
using Balansiq.Entities;

namespace Balansiq
{
    public partial class MainWindow : Form
    {
        DataGridViewCell cellTemplate = null;
        DataGridViewCell cellTemplateAlt = null;
        // todo: create cell template, add use it for grid
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                DBConnector.OpenConnection();
                DBManager.GetAllItems();
                this.datePicker.Value = DateTime.Now;
                this.FormClosing += OnClosing;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nБудь у меня искусственный интеллект, я бы уже исправил ошибку :(", "Упс... Возникла проблема!");
                Load += (s, e) => Close();
            }
        }

        private void InitCellTemplates()
        {

        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DBConnector.CloseConnection();
        }

        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filterName = Interaction.InputBox("Введите название нового типа фильтров:", "Добавить столбец", "Новый фильтр");
            if (filterName != string.Empty)
            {
                SpendFilterType filterType = new SpendFilterType(filterName);
                DBManager.CreateOrUpdateItem(filterType);
                DBManager.SpendFilters.Add(filterType, new List<SpendFilter>());
                DataGridViewColumn column = new DataGridViewColumn();
            }
        }
    }
}
