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

    public enum PaymentsDates
    {
        AllTime,
        LastMonth,
        LastWeek,
        LastDay
    }
    public class PaymentsIndexModelView
    {
        public List<Payment> Payments { get; set; }
        public string FilterByName { get; set; }
        public bool SortReverse { get; set; } = false;
        public SortPaymentsBy SortSelect { get; set; }
        public PaymentsDates ByDate { get; set; }
        public int CurrentPage { get; set; } = 1;
        public bool IsNextPage { get; set; } = false;
        public int PaymentsLength { get; set; } = 20;
    }
}
