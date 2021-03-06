﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balansiq.DB;

namespace Balansiq.DB.Entities
{
    public class SpendItem : IItem
    {
        [NotNull]
        public String Description { get; set; }
        [NotNull]
        public double Amount { get; set; }
        [NotNull]
        public double Price { get; set; }
        [ForeignKey]
        public long? SFilter { get; set; }
        [NotNull]
        public DateTime SDate { get; set; }
        [Ignore]
        new public bool IsEmpty
        {
            get
            {
                return Id == null && Description == string.Empty && Amount == .0 && Price == .0 && SFilter == null;
            }
        }
        [Ignore]
        public static new KeyValuePair<DataTableType, string>? ForeignKey
        {
            get
            {
                return new KeyValuePair<DataTableType,string>(DataTableType.SpendFilters, "Id");
            }
        }

        public SpendItem() : this(DateTime.MinValue) { }
        public SpendItem(DateTime date) : this(null, date, null, 0.0, 0.0, string.Empty) { }
        public SpendItem(int? id, DateTime date, int? filter, double price, double amount, String description) : base(id)
        {
            SDate = date;
            SFilter = filter;
            Price = price;
            Amount = amount;
            Description = description;
        }
    }
}
