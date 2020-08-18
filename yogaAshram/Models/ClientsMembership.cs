using System;
using System.Collections.Generic;

namespace yogaAshram.Models
{
    public class ClientsMembership
    {
        public long Id { get; set; }
        public DateTime DateOfPurchase { get; set; }
        
        public long ClientId { get; set; }
        public Client Client { get; set; }
        
        public long MembershipId { get; set; }
        public Membership Membership { get; set; }
    }
}