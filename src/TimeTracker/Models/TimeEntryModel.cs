using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Domain;

namespace TimeTracker.Models
{
    public class TimeEntryModel
    {
        private TimeEntryModel()
        {
        }
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public DateTime EntryDate { get; set; }
        public int Hours { get; set; }
        public string Description { get; set; }
        public decimal HourRate { get; set; }
        public decimal Total => Hours * HourRate;

        public static TimeEntryModel FromTimeEntry(TimeEntry timeEntry)
        {
            return new TimeEntryModel
            {
                Id = timeEntry.Id,
                UserName = timeEntry.User.Name,
                ProjectId = timeEntry.Project.Id,
                ProjectName = timeEntry.Project.Client.Name,
                ClientName = timeEntry.Project.Client.Name,
                UserId = timeEntry.User.Id,
                EntryDate = timeEntry.EntryDate,
                Hours = timeEntry.Hours,
                HourRate = timeEntry.HourRate,
                Description = timeEntry.Description
            };
        }
    }
}
