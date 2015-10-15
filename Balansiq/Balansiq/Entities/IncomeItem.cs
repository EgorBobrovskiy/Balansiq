using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.Entities
{
    class IncomeItem
    {
        [NotNull]
        public DateTime IDate { get; set; }
        public int IFilter { get; set; }
        [NotNull]
        public double Summary { get; set; }
        [NotNull]
        public string Description { get; set; }
    }
}
