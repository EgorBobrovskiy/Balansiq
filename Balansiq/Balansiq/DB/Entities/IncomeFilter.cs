using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balansiq.DB.Entities
{
    public class IncomeFilter : Filter
    {
        public IncomeFilter() : base() { }
        public IncomeFilter(string name) : base(name) { }
        public IncomeFilter(int? id, String name) : base(id, name) { }
    }
}
