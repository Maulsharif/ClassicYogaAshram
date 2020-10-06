using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yogaAshram.Models.ModelViews
{
    public class CoachDetailsModelView
    {
        public Employee Coach { get; set; }
        public PaymentSelector[] Payments { get; set; }
        public DateTime From { get; set; } = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime To { get; set; } = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.Day + 1);
    }
}
