﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Domain;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/time-entries/")]
    public class TimeEntriesController : Controller
    {
        private readonly TimeTrackerDbContext _dbContext;
        private readonly ILogger<TimeEntriesController> _logger;
        public TimeEntriesController(TimeTrackerDbContext dbContext, ILogger<TimeEntriesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TimeEntryModel>> GetById (long id)
        {
            _logger.LogInformation($"Getting a time entry with id {id}");
            var timeEntry = await _dbContext.TimeEntries
                .Include(x => x.Project)
                .Include(x => x.Project.Client)
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Id == id);

            //Ukljucimo sve da mozemo dobiti ClientName npr u TimeEntry
            
            if(timeEntry == null)
            {
                return NotFound();
            }

            return TimeEntryModel.FromTimeEntry(timeEntry); //Konvertujemo u TimeEntry i vratimo
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<TimeEntryModel>>> GetPage(int page = 1, int size = 5)
        {
            _logger.LogInformation($"Getting a page {page} of time entries with page size {size}");

            var timeEntries = await _dbContext.TimeEntries
                .Include(x => x.Project)
                .Include(x => x.Project.Client)
                .Include(x => x.User)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var totalCount = await _dbContext.TimeEntries.CountAsync();
            return new PagedList<TimeEntryModel>
            {
                Items = timeEntries.Select(TimeEntryModel.FromTimeEntry),
                Page = page,
                PageSize = size
            };
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogDebug($"Deleting time entries with id {id}");

            var timeEntry = await _dbContext.TimeEntries.FindAsync(id);

            if (timeEntry == null)
            {
                return NotFound();
            }

            _dbContext.TimeEntries.Remove(timeEntry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<TimeEntryModel>> Create(TimeEntryInputModel model)
        {
            _logger.LogDebug(
                $"Creating a new time entry for user {model.UserId}, project {model.ProjectId} and date {model.EntryDate}");

            var user = await _dbContext.Users.FindAsync(model.UserId);
            var project = await _dbContext.Projects
                .Include(x => x.Client) // Necessary for mapping to TimeEntryModel later
                .SingleOrDefaultAsync(x => x.Id == model.ProjectId);

            if (user == null || project == null)
            {
                return NotFound();
            }

            var timeEntry = new TimeEntry
            {
                User = user,
                Project = project,
                HourRate = user.HourRate
            };

            model.MapTo(timeEntry);

            await _dbContext.TimeEntries.AddAsync(timeEntry);
            await _dbContext.SaveChangesAsync();

            var resultModel = TimeEntryModel.FromTimeEntry(timeEntry);

            return CreatedAtAction(nameof(GetById), "TimeEntries", new { id = timeEntry.Id }, resultModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TimeEntryModel>> Update(long id, TimeEntryInputModel model)
        {
            _logger.LogDebug($"Updating time entry with id {id}");

            var timeEntry = await _dbContext.TimeEntries
                .Include(x => x.User)
                .Include(x => x.Project)
                .Include(x => x.Project.Client)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (timeEntry == null)
            {
                return NotFound();
            }

            model.MapTo(timeEntry);

            _dbContext.TimeEntries.Update(timeEntry);
            await _dbContext.SaveChangesAsync();

            return TimeEntryModel.FromTimeEntry(timeEntry);
        }


    }
}