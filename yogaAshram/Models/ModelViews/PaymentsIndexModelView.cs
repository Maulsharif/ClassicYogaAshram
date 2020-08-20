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
        Sickness
    }
    public class PaymentsIndexModelView
    {
        public List<Payment> Payments { get; set; }
        public string FilterByName { get; set; }
        public int MyProperty { get; set; }
        public bool SortReverse { get; set; } = false;
        public SortPaymentsBy SortSelect { get; set; }
        public int CurrentPage { get; set; }
        public bool IsNextPage { get; set; } = false;
        public int PaymentsLength { get; set; } = 20;
    }
}
