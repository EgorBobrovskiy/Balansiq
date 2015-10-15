using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balansiq.DB
{
    class DBAttributes
    {
        public static readonly List<Type> attributes = new List<Type>()
        {
            typeof(Ignore),
            typeof(NotNull),
            typeof(AutoIncrement),
            typeof(PrimaryKey),
            typeof(Unique)
        };
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class NotNull : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class AutoIncrement : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class PrimaryKey : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class Unique : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class Ignore : Attribute { }
}
