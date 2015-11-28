using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.DB.Entities
{
    public class IncomeItem : IItem
    {
        [NotNull]
        public string Description { get; set; }
        [NotNull]
        public double Summary { get; set; }
        [ForeignKey]
        public int? IFilter { get; set; }
        [NotNull]
        public DateTime IDate { get; set; }
        [Ignore]
        public static new KeyValuePair<Balansiq.DB.DataTableType, string>? ForeignKey
        {
            get
            {
                return new KeyValuePair<DataTableType,string>(DataTableType.IncomeFilters, "Id");
            }
        }

        public IncomeItem() : this(null, DateTime.MinValue, null, 0.0, "") { }
        public IncomeItem(int? id, DateTime date, int? filter, double summary, string description) : base(id)
        {
            IDate = date;
            IFilter = filter;
            Summary = summary;
            Description = description;
        }
    }
}
