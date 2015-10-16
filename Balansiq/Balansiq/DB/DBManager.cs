using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.Entities;
using System.Data.SQLite;

namespace Balansiq.DB
{
    class DBManager
    {
        private static readonly string DT_INCOME_DATA = "INCOME_DATA";
        private static readonly string DT_INCOME_FILTERS = "INCOME_FILTERS";
        private static readonly string DT_SPEND_DATA = "SPEND_DATA";
        private static readonly string DT_SPEND_FILTERS = "SPEND_FILTERS";
        private static readonly string DT_SPEND_FILTER_TYPES = "SPEND_FILTER_TYPES";

        private static readonly string Q_CREATE_TABLE_FORMAT = "CREATE TABLE '{0}' ({1});";
        private static readonly string Q_FOREIGN_KEY_FORMAT = "FOREIGN KEY('{0}') REFERENCES {1}( {2} ) ON UPDATE CASCADE ON DELETE SET NULL";
        private static readonly string Q_PARAM_DELIMITER = " , ";

        public static Dictionary<int, Filter> IncomeFilters { get; protected set; }
        public static Dictionary<Filter, List<SpendFilter>> SpendFilters { get; protected set; }
        public static Dictionary<DateTime, List<SpendItem>> spendData { get; protected set; }
        public static Dictionary<DateTime, List<IncomeItem>> incomeData { get; protected set; }

        //public static void CreateTable
    }
}
