using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace yogaAshram.Models
{
    public enum AttendanceState
    {
         [Description("+")]
        attended=1,
        [Description("н")]
      
        notattended=2,
        [Description("з")]
        frozen=3,
        [Description("н/п")]
        notcheked=4,
        [Description("от")]
        cancel =5
    }
    public class Attendance
    {
        public long Id { get; set; }
        public long  ClientId  { get; set; }
        public virtual Client  Client { get; set; }
        public long  GroupId  { get; set; }
        public virtual Group  Group { get; set; }
        public long? MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
        public long? ClientsMembershipId { get; set; }
        public virtual  ClientsMembership ClientsMembership{ get; set; }
        public AttendanceState AttendanceState { get; set; } = Models.AttendanceState.notcheked;
        public bool IsChecked { get; set; } = false;
        public bool IsNotActive { get; set; }
        public DateTime Date { get; set; }
        public long? AttendanceCountId { get; set; }
        public virtual AttendanceCount AttendanceCount { get; set; }
    }
}