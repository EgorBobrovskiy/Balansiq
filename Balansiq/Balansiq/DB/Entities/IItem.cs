using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.DB.Entities
{
    public abstract class IItem
    {
        [Ignore]
        private long? _id = null;

        [Ignore]
        public bool IsEmpty { get { return this.Id == null; } }

        [PrimaryKey, Unique, NotNull]
        public long? Id
        {
            get { return _id; }
            set { if (_id == null) _id = value; }
        }

        [Ignore]
        public static KeyValuePair<Balansiq.DB.DataTableType, string>? ForeignKey { get { return null; } }

        public IItem(int? id)
        {
            Id = id;
        }

        public IItem() : this(null) { }
    }
}
