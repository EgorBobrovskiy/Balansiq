using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.Entities
{
    class SpendItem
    {
        [NotNull]
        public DateTime SDate { get; set; }
        public int SFilter { get; set; }
        [NotNull]
        public double Price { get; set; }
        [NotNull]
        public double Amount { get; set; }
        [NotNull]
        public String Description { get; set; }
    }
}
