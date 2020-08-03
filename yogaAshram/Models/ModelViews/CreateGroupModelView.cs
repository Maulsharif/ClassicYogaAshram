﻿using System.Collections.Generic;

namespace yogaAshram.Models.ModelViews
{
    public class CreateGroupModelView
    {
        public long Id { get; set; }
        public string  Name { get; set; }
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public string  CoachName { get; set; }
        
        public int MaxCapacity { get; set; } = 16;
        
        public int MinCapacity { get; set; } = 10;
        
        public long  CreatorId { get; set; }
        
        public virtual Employee Creator { get; set; }
    }
}