using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.Entities
{
    class SpendFilter : Filter
    {
        [ForeignKey]
        public long? Type { get; set; }
        
        [Ignore]
        public static new KeyValuePair<DataTable, string>? ForeignKey
        {
            get
            {
                return new KeyValuePair<DataTable,string>(DataTable.SpendFilterTypes, "Id");
            }
        }

        public SpendFilter() : this("", null) { }
        public SpendFilter(String name, int? type) : this(null, name, type) { }
        public SpendFilter(int? id, String name, int? type) : base(id, name)
        {
            Type = type;
        }

        public override string ToString()
        {
            return String.Format("Id:{1}; Type:{0}; Name:{2}", this.Type, this.Id, this.Name);
        }
    }
}
