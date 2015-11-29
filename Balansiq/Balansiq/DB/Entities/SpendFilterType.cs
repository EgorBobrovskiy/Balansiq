using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balansiq.DB.Entities
{
    public class SpendFilterType : Filter
    {
        public SpendFilterType() : base() { }
        public SpendFilterType(string name) : this(null, name) { }
        public SpendFilterType(int? id, String name) : base(id, name) { }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
