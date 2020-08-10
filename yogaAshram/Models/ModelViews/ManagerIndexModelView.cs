namespace yogaAshram.Models.ModelViews
{
    public class ManagerIndexModelView
    {
        public bool IsModalInvalid { get; set; } = false;
        public Employee Employee { get; set; }
        public bool IsEditInvalid { get; set; } = false;
        public ManagerEditModelView ManagerEditModel { get; set; } = new ManagerEditModelView();
        public ChangePasswordModelView Model { get; set; } = new ChangePasswordModelView();
    }
}