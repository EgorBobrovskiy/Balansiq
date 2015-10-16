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

namespace Balansiq
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                DBConnector.OpenConnection(3);
                this.FormClosing += OnClosing;
            }
            catch (DBOpenException ex)
            {
                MessageBox.Show(ex.Message, ex.title);
                Load += (s, e) => Close();
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DBConnector.CloseConnection();
        }
    }
}
