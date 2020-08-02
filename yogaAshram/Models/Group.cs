namespace yogaAshram.Models
{
    public class Group
    {
        public long Id { get; set; }
        public string  Name { get; set; }
        public int IdBranch { get; set; }
        public virtual Branch Branch { get; set; }
        public string  CoachName { get; set; }
        public int MaxCapacity { get; set; } = 16;
        public int MinCapacity { get; set; } = 10;
    }
}