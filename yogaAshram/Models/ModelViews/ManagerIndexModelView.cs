namespace yogaAshram.Models.ModelViews
{
    public class ManagerIndexModelView
    {
        public bool IsModalInvalid { get; set; } = false;
        public Employee Employee { get; set; }
        public bool IsEditInvalid { get; set; } = false;
        public EditModelView EditModel { get; set; } = new EditModelView();
        public ChangePasswordModelView Model { get; set; } = new ChangePasswordModelView();
    }
}