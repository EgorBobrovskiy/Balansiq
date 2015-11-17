using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.Entities
{
    abstract class IItem
    {
        [Ignore]
        private long? _id = null;

        [PrimaryKey, Unique, NotNull]
        public long? Id
        {
            get { return _id; }
            set { if (_id == null) _id = value; }
        }

        [Ignore]
        public static KeyValuePair<Balansiq.DB.DataTable, string>? ForeignKey { get { return null; } }

        public IItem(int? id)
        {
            Id = id;
        }

        public IItem() : this(null) { }
    }
}
