using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yogaAshram.Models
{
    public class Payment
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public long MembershipId { get; set; }
        public long ClientId { get; set; }
        public long CreatorId { get; set; }
        public int Debts { get; set; }
        public DateTime CateringDate { get; set; } = DateTime.Now;
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        public virtual Employee Creator { get; set; }
        public virtual Client Client { get; set; }
        public virtual Membership Membership { get; set; }
    }
}
