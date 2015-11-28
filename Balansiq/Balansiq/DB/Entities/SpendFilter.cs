using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balansiq.DB.Entities
{
    public class SpendFilter : Filter
    {
        [ForeignKey]
        public long? Type { get; set; }
        
        [Ignore]
        public static new KeyValuePair<DataTableType, string>? ForeignKey
        {
            get
            {
                return new KeyValuePair<DataTableType,string>(DataTableType.SpendFilterTypes, "Id");
            }
        }

        public SpendFilter() : this("", null) { }
        public SpendFilter(String name, long? type) : this(null, name, type) { }
        public SpendFilter(int? id, String name, long? type) : base(id, name)
        {
            Type = type;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
