using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Balansiq.DB
{
    public class DBConnector
    {
        private static readonly string source = "Data Source={0}; Version={1}; Primary Key={2};";
        private static readonly string DBName = "Data\\userData.sl3";
        private static readonly string DBNameTest = "Data\\test.sl3";
        private static SQLiteConnection _connection = null;
        public static SQLiteConnection Connection { get { return _connection; } }

        public static void OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection(string.Format(source, DBNameTest, "3", "true"), true);
                _connection.Open();
                DBManager.CheckTables();
            }
        }

        public static void CloseConnection()
        {
            if (_connection != null)
            {
                //TODO: save changes in database

                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
