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
        public long? IFilter { get; set; }
        [NotNull]
        public DateTime IDate { get; set; }
        [Ignore]
        public new bool IsEmpty
        {
            get { return this.Description == string.Empty && this.Summary == 0.0 && !this.IFilter.HasValue; }
        }
        [Ignore]
        public static new KeyValuePair<Balansiq.DB.DataTableType, string>? ForeignKey
        {
            get
            {
                return new KeyValuePair<DataTableType,string>(DataTableType.IncomeFilters, "Id");
            }
        }

        public IncomeItem() : this(DateTime.MinValue) { }
        public IncomeItem(DateTime date) : this(null, date.Date, null, 0.0, string.Empty) { }
        public IncomeItem(long? id, DateTime date, long? filter, double summary, string description) : base(id)
        {
            IDate = date;
            IFilter = filter;
            Summary = summary;
            Description = description;
        }
    }
}
