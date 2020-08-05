using yogaAshram.Controllers;

namespace yogaAshram.Models
{
    public class Client
    {
        public long Id { get; set; }
        
        public string NameSurname { get; set; }

        public string PhoneNumber { get; set; }

        public ClientType ClientType { get; set; } = ClientType.Probe;

        public long GroupId { get; set; }
        
        public virtual Group Group { get; set; }
        
        public long  CreatorId { get; set; }
        
        public virtual Employee Creator { get; set; }
    }
}