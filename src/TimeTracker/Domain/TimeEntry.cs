using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Domain
{
    public class TimeEntry
    {
        public long Id { get; set; }

        [Required]
        public Project Project { get; set; }

        [Required]
        public User User { get; set; }

        public DateTime EntryDate { get; set; }

        [Range(1, 24)]
        public int Hours { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal HourRate { get; set; }
    }
}
