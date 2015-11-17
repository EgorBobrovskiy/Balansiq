using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.Entities
{
    class Filter : IItem
    {
        [NotNull]
        public String Name { get; set; }

        public Filter() : this("") { }
        public Filter(String name) : this(null, name) { }
        public Filter(int? id, String name) : base(id)
        {
            Name = name;
        }


        public override string ToString()
        {
            return string.Format("{0}: {1}", Id.ToString(), Name);
        }
    }
}
