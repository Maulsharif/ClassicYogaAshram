using System.Collections.Generic;

namespace yogaAshram.Models.ModelViews
{
    public class AdminIndexModel
    {
        public List<Group> Groups { get; set; }
        public bool IsModalInvalid { get; set; } = false;
        public Employee Employee { get; set; }
        public bool IsEditInvalid { get; set; } = false;
        public EmployeeEditModelView ManagerEditModel { get; set; } = new EmployeeEditModelView();
        public ChangePasswordModelView Model { get; set; } = new ChangePasswordModelView();
    }
}