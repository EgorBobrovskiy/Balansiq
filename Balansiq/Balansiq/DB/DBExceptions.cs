using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balansiq.DB
{
    public class DBExceptions : Exception
    {
        public String Title { get; set; }

        public DBExceptions()
            : this(String.Empty)
        { }

        public DBExceptions(String message)
            : this(message, String.Empty)
        { }

        public DBExceptions(String message, String title)
            : base(message)
        {
            this.Title = title;
        }
    }

    class DBOpenException : DBExceptions
    {
        public DBOpenException() : this("Cannot set connection with database.") { }

        public DBOpenException(String exStr) : this(exStr, String.Empty) { }

        public DBOpenException(String msg, String title) : base(msg, title) { }
    }
}
