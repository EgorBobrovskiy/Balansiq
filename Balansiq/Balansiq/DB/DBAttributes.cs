using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balansiq.DB
{
    sealed class DBAttributes
    {
        public static readonly Dictionary<Type, string> Attributes = new Dictionary<Type, string>()
        {
            {typeof(Ignore), string.Empty},
            {typeof(NotNull), "NOT NULL"},
            {typeof(AutoIncrement), "AUTOINCREMENT"},
            {typeof(PrimaryKey), "PRIMARY KEY"},
            {typeof(Unique), "UNIQUE"},
            {typeof(ForeignKey), string.Empty}
        };

        public static readonly Dictionary<Type, string> TypeNames = new Dictionary<Type, string>()
        {
            {typeof(long?), "INTEGER"},
            {typeof(int?), "INTEGER"},
            {typeof(int), "INTEGER"},
            {typeof(double), "REAL"},
            {typeof(string), "TEXT"},
            {typeof(DateTime), "INTEGER"}
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class ForeignKey : Attribute { }
}
