using System.Collections.Generic;

namespace yogaAshram.Models
{
    public class Branch
    {
        public long Id { get; set; } 
        public string Name { get; set; }
        public string Address { get; set; }
        public string Info { get; set; }
        public long? MarketerId { get; set; }
        public virtual Employee Marketer { get; set; }
        public long? SellerId { get; set; }
        public virtual Employee Seller { get; set; }
    }
}