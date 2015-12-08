using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB.Entities;
using System.Data.SQLite;

namespace Balansiq.DB
{
    public enum DataTableType
    {
        IncomeData,
        IncomeFilters,
        SpendData,
        SpendFilters,
        SpendFilterTypes
    }

    public class DBManager
    {
        private static readonly Dictionary<DataTableType, Tuple<string, Type>> Tables = new Dictionary<DataTableType, Tuple<string, Type>>()
        {
           {DataTableType.IncomeData, new Tuple<string, Type>("INCOME_DATA", typeof(IncomeItem))},
           {DataTableType.IncomeFilters, new Tuple<string, Type>("INCOME_FILTERS", typeof(IncomeFilter))},
           {DataTableType.SpendData, new Tuple<string, Type>("SPEND_DATA", typeof(SpendItem))},
           {DataTableType.SpendFilters, new Tuple<string, Type>("SPEND_FILTERS", typeof(SpendFilter))},
           {DataTableType.SpendFilterTypes, new Tuple<string, Type>("SPEND_FILTER_TYPES", typeof(SpendFilterType))}
        };

        private static readonly string Q_GET_TABLE_INFO_FORMAT = "SELECT * FROM sqlite_master WHERE type='table' AND name='{0}';";
        private static readonly string Q_GET_LAST_INSERT_ROWID = "SELECT last_insert_rowid();";
        private static readonly string Q_CREATE_TABLE_FORMAT = "CREATE TABLE '{0}' ({1});";
        private static readonly string Q_FOREIGN_KEY_FORMAT = "FOREIGN KEY('{0}') REFERENCES {1}( {2} ) ON UPDATE CASCADE ON DELETE SET NULL";
        private static readonly string Q_INSERT_FORMAT = "INSERT INTO {0} ({1}) VALUES ({2});";
        private static readonly string Q_UPDATE_FORMAT = "UPDATE {0} SET {1} WHERE {2};";
        private static readonly string Q_DELETE_FORMAT = "DELETE FROM {0} WHERE {1};";
        private static readonly string Q_SELECT_ALL_FORMAT = "SELECT * FROM {0};";

        private static string ForeignKey = string.Empty;

        public static List<IncomeFilter> IncomeFilters { get; protected set; }
        public static Dictionary<SpendFilterType, List<SpendFilter>> SpendFilters { get; protected set; }
        public static Dictionary<DateTime, List<IncomeItem>> IncomeData { get; protected set; }
        public static Dictionary<DateTime, List<SpendItem>> SpendData { get; protected set; }

        public static void CheckTables()
        {
            foreach (var te in Tables)
            {
                if (!TableExists(te.Key))
                    ExecuteNonQuery(CreateTableQuery(te.Key, te.Value.Item2));
            }
        }

        private static bool TableExists(DataTableType table)
        {
            return ExecuteReader(string.Format(Q_GET_TABLE_INFO_FORMAT, Tables[table].Item1)).HasRows;
        }

        private static int ExecuteNonQuery(string sql)
        {
            var conn = DBConnector.Connection;
            var com = conn.CreateCommand();
            com.CommandText = sql;
            int rows = com.ExecuteNonQuery();
            conn = null;
            return rows;
        }

        private static SQLiteDataReader ExecuteReader(string sql)
        {
            var conn = DBConnector.Connection;
            var com = conn.CreateCommand();
            com.CommandText = sql;
            SQLiteDataReader reader = com.ExecuteReader();
            conn = null;
            return reader;
        }

        private static object ExecuteScalar(string sql)
        {
            var conn = DBConnector.Connection;
            var com = conn.CreateCommand();
            com.CommandText = sql;
            object obj = com.ExecuteScalar();
            conn = null;
            return obj;
        }

        public static string CreateTableQuery(DataTableType table, Type item)
        {
            ForeignKey = string.Empty;
            var createParams = new StringBuilder();
            foreach(var field in GetItemFields(item))
            {
                createParams.AppendFormat("\n{0},", GetItemFieldString(field.Key, field.Value));
            }
            if (ForeignKey == string.Empty)
            {
                createParams.Remove(createParams.Length - 1, 1);
            }
            else
            {
                var fk = (KeyValuePair<DataTableType, string>?)item.GetProperty("ForeignKey").GetValue(null, null);
                if (fk != null)
                {
                    createParams.AppendFormat('\n' + Q_FOREIGN_KEY_FORMAT, ForeignKey, Tables[fk.Value.Key].Item1, fk.Value.Value);
                }
            }

            return string.Format(Q_CREATE_TABLE_FORMAT, Tables[table].Item1, createParams.ToString());
        }

        private static Dictionary<string, List<Type>> GetItemFields(Type item)
        {
            var ret = new Dictionary<string, List<Type>>();
            var props = new List<System.Reflection.PropertyInfo>(item.GetProperties().Reverse());

            foreach(var p in props)
            {
                var atts = p.CustomAttributes;
                if (atts.FirstOrDefault(b => b.AttributeType == typeof(Ignore)) == null)
                {
                    if (atts.FirstOrDefault(b => b.AttributeType == typeof(ForeignKey)) != null && ForeignKey == string.Empty)
                    {
                        ForeignKey = p.Name;
                    }
                    ret.Add(p.Name, atts
                        .Select(el => el.AttributeType)
                        .Where(el => DBAttributes.Attributes.Select(e => e.Key).Contains(el)).ToList());
                    ret[p.Name].Insert(0, p.PropertyType);
                }
            }
            return ret;
        }

        private static string GetItemFieldString(string fieldName, List<Type> fieldAttributes)
        {
            var builder = new StringBuilder('\'' + fieldName + "\'");

            foreach(var fa in fieldAttributes)
            {
                if (DBAttributes.Attributes.ContainsKey(fa))
                {
                    builder.AppendFormat(" {0}", DBAttributes.Attributes[fa]);
                }
                else if (DBAttributes.TypeNames.ContainsKey(fa))
                {
                    builder.AppendFormat(" {0}", DBAttributes.TypeNames[fa]);
                }
            }

            return builder.ToString();
        }

        public static void CreateOrUpdateItem(IItem item)
        {
            if (item != null)
            {
                var itemValues = item.GetType().GetProperties().Reverse()
                    .Where(e => e.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(Ignore)) == null)
                    .ToDictionary(k => k.Name, v => v.GetValue(item, null));

                if (item.Id == null)
                {
                    // Create
                    StringBuilder columns = new StringBuilder();
                    StringBuilder values = new StringBuilder();

                    foreach (var iv in itemValues.Where(iv => iv.Key.ToLower() != "id"))
                    {
                        DateTime? date = iv.Value as DateTime?;
                        string valstr = (date == null)
                            ? iv.Value != null 
                                ? iv.Value.ToString() 
                                : string.Empty
                            : date.Value.ToString("yyyy-MM-dd");
                        columns.Append(iv.Key + ',');
                        values.Append(String.Format("'{0}',", valstr));
                    }
                    columns.Remove(columns.Length - 1, 1);
                    values.Remove(values.Length - 1, 1);

                    var table = Tables.Values.FirstOrDefault(t => t.Item2 == item.GetType()).Item1;
                    String sql = String.Format(Q_INSERT_FORMAT, table, columns, values);
                    ExecuteNonQuery(sql);

                    object i = ExecuteScalar(Q_GET_LAST_INSERT_ROWID);

                    if (i != null)
                    {
                        item.Id = (long)i;
                    }
                }
                else
                {
                    // Update
                    StringBuilder values = new StringBuilder();
                    String id = String.Format("Id='{0}'", itemValues.FirstOrDefault(iv => iv.Key.ToLower() == "id").Value.ToString());
                    var table = Tables.Values.FirstOrDefault(t => t.Item2 == item.GetType()).Item1;

                    foreach (var iv in itemValues.Where(iv => iv.Key.ToLower() != "id"))
                    {
                        DateTime? date = iv.Value as DateTime?;
                        string valstr = (date == null)
                            ? (iv.Value != null) 
                                ? string.Format("'{0}'", iv.Value.ToString()) 
                                : "NULL"
                            : string.Format("'{0}'", date.Value.ToString("yyyy-MM-dd"));
                        values.Append(String.Format("{0}={1},", iv.Key, valstr));
                    }
                    values.Remove(values.Length - 1, 1);

                    String sql = String.Format(Q_UPDATE_FORMAT, table, values, id);
                    ExecuteNonQuery(sql);
                }
            }
        }

        public static void GetAllItems()
        {
            // fill income filters
            IncomeFilters = GetItems<IncomeFilter>();
            
            // fill spend filters
            var spendFilterTypes = GetItems<SpendFilterType>();
            var spendFilters = GetItems<SpendFilter>();
            SpendFilters = new Dictionary<SpendFilterType, List<SpendFilter>>();
            foreach (var sft in spendFilterTypes)
            {
                if (!SpendFilters.Keys.Contains(sft))
                {
                    var filters = spendFilters.Where(f => f.Type == sft.Id).ToList();
                    SpendFilters.Add(sft, filters);
                    spendFilters = spendFilters.Except(filters).ToList();
                }
            }
            if (spendFilters.Count > 0)
            {
                SpendFilterType other = new SpendFilterType(-1, "Другое");
                SpendFilters.Add(other, spendFilters);
                foreach(var sf in spendFilters)
                {
                    sf.Type = -1;
                }
            }

            // fill income items
            var incomeItems = GetItems<IncomeItem>();
            IncomeData = new Dictionary<DateTime,List<IncomeItem>>();
            foreach (var ii in incomeItems)
            {
                var key = IncomeData.Keys.FirstOrDefault(k => k.Date == ii.IDate.Date);
                if (key == DateTime.MinValue)
                {
                    key = ii.IDate.Date;
                    IncomeData.Add(key, new List<IncomeItem>());
                }
                IncomeData[key].Add(ii);
            }

            // fill spend items
            var spendItems = GetItems<SpendItem>();
            SpendData = new Dictionary<DateTime, List<SpendItem>>();
            foreach (var si in spendItems)
            {
                var key = SpendData.Keys.FirstOrDefault(k => k.Date == si.SDate.Date);
                if (key == DateTime.MinValue)
                {
                    key = si.SDate.Date;
                    SpendData.Add(key, new List<SpendItem>());
                }
                SpendData[key].Add(si);
            }
        }
        
        private static List<T> GetItems<T>() where T: IItem
        {
            List<T> ret = new List<T>();
            string table = Tables.Values.FirstOrDefault(t => t.Item2 == typeof(T)).Item1;
            string sql = String.Format(Q_SELECT_ALL_FORMAT, table);
            var properties = typeof(T).GetProperties()
                .Where(e => e.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(Ignore)) == null)
                .ToList();

            SQLiteDataReader reader = ExecuteReader(sql);
            while(reader.Read())
            {
                T item = Activator.CreateInstance<T>();
                foreach(var p in properties)
                {
                    object value = reader[p.Name];
                    if (value == System.DBNull.Value) value = null;
                    if (p.Name == "SDate" || p.Name == "IDate")
                    {
                        value = DateTime.Parse((string)value);
                    }
                    p.SetValue(item, value);
                }
                ret.Add(item);
            }

            return ret;
        }

        public static void DeleteItem(IItem item)
        {
            if (item != null && item.Id != null)
            {
                string table = Tables.Values.FirstOrDefault(t => t.Item2 == item.GetType()).Item1;
                string sql = String.Format(Q_DELETE_FORMAT, table, "Id=" + item.Id.ToString());
                ExecuteNonQuery(sql);
            }
        }

        public static SpendFilter GetSpendFilter(long id)
        {
            foreach (var filters in SpendFilters.Values)
            {
                var sf = filters.Find(f => f.Id.HasValue && f.Id.Value == id);
                if (sf != null)
                {
                    return sf;
                }
            }
            return null;
        }

        public static Dictionary<string, double> GetSpendValuesForPeriod(List<SpendFilter> selection, DateTime from, DateTime to)
        {
            Dictionary<string, double> ret = new Dictionary<string, double>();

            foreach (var filter in selection)
            {
                double value = SpendData
                    .Where(kvp => kvp.Key >= from.Date && kvp.Key <= to.Date)
                    .Select(kvp => kvp.Value
                        .Where(item => item.SFilter == filter.Id)
                        .Select(item => item.Price * item.Amount)
                        .Sum())
                    .Sum();
                ret.Add(filter.Name, value);
            }

            return ret;
        }
    }
}
