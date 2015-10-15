using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Balansiq.DB
{
    class DBConnector
    {
        private static readonly string source = "Data Source={0}; Version={1}; Primary Key={2};";
        private static readonly string DBName = "Data\\userData.sl3";
        private static readonly string DBNameTest = "Data\\test.sl3";
        //private static String appPath = AppDomain.CurrentDomain.BaseDirectory;
        // connections: connection, availability
        private static Dictionary<SQLiteConnection, bool> connectionPool = null;

        public static void OpenConnection(int poolSize)
        {
            if (connectionPool == null)
            {
                connectionPool = new Dictionary<SQLiteConnection, bool>(poolSize);
                int count = 0;
                for (int i = 0; i < poolSize && count < poolSize * 2; count++)
                {
                    try
                    {
                        SQLiteConnection connection = new SQLiteConnection(string.Format(source, DBNameTest, "3", "true"), true);
                        connection.Open();
                        connectionPool.Add(connection, true);
                        i++;
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                if (connectionPool.Count == 0)
                {
                    throw new DBOpenException("No connection. Tries: " + count, "Cannot open database");
                }
            }
        }

        public static void CloseConnection()
        {
            if (connectionPool != null)
            {
                //TODO: save changes in database

                foreach (var connection in connectionPool.Keys)
                {
                    connection.Close();
                    connection.Dispose();
                }
                connectionPool = null;
            }
        }

        public static SQLiteConnection Get()
        {
            if (connectionPool != null)
            {
                SQLiteConnection conn = connectionPool.Where(e => e.Value == true)
                    .Select(e => e.Key)
                    .FirstOrDefault();
                if (conn == null)
                {
                    conn = new SQLiteConnection(string.Format(source, DBNameTest, "3", "true"), true);
                    conn.Open();
                }
                connectionPool[conn] = false;
                return conn;
            }
            return null;
        }

        public static void Free(SQLiteConnection conn)
        {
            var c = conn as SQLiteConnection;
            if (c != null && connectionPool.Keys.Contains(c))
            {
                connectionPool[c] = true;
            }
        }
    }
}
