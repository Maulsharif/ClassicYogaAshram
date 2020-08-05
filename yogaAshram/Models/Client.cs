namespace yogaAshram.Models
{
    public class Client
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string ClientType { get; set; }

        public long GroupId { get; set; }
        
        public virtual Group Group { get; set; }
    }
}