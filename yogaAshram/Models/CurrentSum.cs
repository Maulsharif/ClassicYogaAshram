using System.ComponentModel.DataAnnotations.Schema;

namespace yogaAshram.Models
{
    public class CurrentSum
    {
        public long Id { get; set; }
        public int CashSum { get; set; }
        
        public int CreditSum { get; set; } 
        public long ?BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        public int Total => CashSum+ CreditSum;
    }
   
}