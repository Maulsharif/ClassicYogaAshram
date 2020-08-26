using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yogaAshram.Models.ModelViews
{
    public class MembershipExtendModelView : PaymentCreateModelView
    {
        [Remote(action: "CheckMembershipId", controller: "Validation")]
        public long MembershipId { get; set; }
        public long GroupId { get; set; }
    }
}
