using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.DB.Entities
{
    public class Filter : IItem
    {
        [NotNull]
        public String Name { get; set; }

        public Filter() : this(string.Empty) { }
        public Filter(String name) : this(null, name) { }
        public Filter(int? id, String name) : base(id)
        {
            Name = name;
        }
        
        [Ignore]
        public new bool IsEmpty { get { return Id == null && Name == string.Empty; } }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Id.ToString(), Name);
        }
    }
}
