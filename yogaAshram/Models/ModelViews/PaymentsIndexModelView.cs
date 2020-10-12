using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yogaAshram.Models.ModelViews
{
    public enum SortPaymentsBy
    {
        None,
        Memberships,
        Price,
        Group,
        Comment,
        Sickness,
        Debtors,
        Debts
    }

    public class PaymentsIndexModelView
    {
        public List<Payment> Payments { get; set; }
        public Employee[] Coaches { get; set; }
        public long? CoachId { get; set; }
        public string FilterByName { get; set; }
        public bool SortReverse { get; set; } = false;
        public SortPaymentsBy SortSelect { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public long? SicknessId { get; set; }
        public int CurrentPage { get; set; } = 1;
        public bool IsNextPage { get; set; } = false;
        public int PaymentsLength { get; set; } = 20;
        public int CashSum { get; set; } = 0;
        public int CardSum { get; set; }
        public Branch Branch { get; set; }
        public void SetAmount()
        {
            foreach (var item in Payments)
            {
                CashSum += item.CashSum;
                CardSum += item.CardSum;
            }
        }
    }
}
