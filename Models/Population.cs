//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UCTP_GA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Population
    {
        public string PopulationId { get; set; }
        public string ScheduleId { get; set; }
        public Nullable<int> ConflictNumber { get; set; }
        public Nullable<double> FitnessNumber { get; set; }
    
        public virtual Schedule Schedule { get; set; }
    }
}
