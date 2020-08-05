using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yogaAshram.Models.ModelViews
{
    public class ChiefIndexModelView
    {
        public bool IsModalInvalid { get; set; } = false;
        public Employee Employee { get; set; }
        public bool IsEditInvalid { get; set; } = false;
        public ChiefEditModelView EditModel { get; set; } = new ChiefEditModelView();
        public ChangePasswordModelView Model { get; set; } = new ChangePasswordModelView();
        public CheckPasswordModelView CheckPassword { get; set; } = new CheckPasswordModelView();
    }
}
