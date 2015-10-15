using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.Entities
{
    class Filter
    {
        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Id { get; set; }

        [NotNull]
        public String Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Id.ToString(), Name);
        }
    }
}
