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
                MainControl.InitComponents(this);
                
                // bind events
                this.FormClosing += OnClosing;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nБудь у меня искусственный интеллект, я бы уже исправил ошибку :(", "Упс... Возникла проблема!");
                Load += (s, e) => Close();
            }
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
