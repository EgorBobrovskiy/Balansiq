using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.Entities
{
    class SpendItem : IItem
    {
        [NotNull]
        public String Description { get; set; }
        [NotNull]
        public double Amount { get; set; }
        [NotNull]
        public double Price { get; set; }
        [ForeignKey]
        public int? SFilter { get; set; }
        [NotNull]
        public DateTime SDate { get; set; }
        [Ignore]
        public static new KeyValuePair<Balansiq.DB.DataTable, string>? ForeignKey
        {
            get
            {
                return new KeyValuePair<DataTable,string>(DataTable.SpendFilters, "Id");
            }
        }

        public SpendItem() : this(null, DateTime.MinValue, null, 0.0, 0.0, "") { }
        public SpendItem(int? id, DateTime date, int? filter, double price, double amount, String description) : base(id)
        {
            SDate = date;
            SFilter = filter;
            Price = price;
            Amount = amount;
            Description = description;
        }
    }
}
