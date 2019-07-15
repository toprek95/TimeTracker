using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/users")]
    public class UsersController : Controller
    {
        private readonly TimeTrackerDbContext _dbContext;
        private readonly ILogger<UsersController> _logger;
        public UsersController(TimeTrackerDbContext dbContext, ILogger<UsersController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetById(long id)
        {
            _logger.LogInformation($"Getting user by id: {id}");
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning($"User with id: {id} not found");
                return NotFound(); //Ako ne nadje korisnika vrati 404
            }

            return UserModel.FromUser(user);
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<UserModel>>> GetPage(int page = 1, int size = 5)
        {
            _logger.LogInformation($"Getting a page {page} of users with page size {size}");

            var users = await _dbContext.Users
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(); //Prikazujemo stranice korisnika, po njih {size}
            var totalUsers = await _dbContext.Users.CountAsync();

            return new PagedList<UserModel>
            {
                Items = users.Select(UserModel.FromUser), //Da pretvorimo Usere iz liste u UserModel
                Page = page,
                PageSize = size,
                TotalCount = totalUsers
            };
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogInformation($"Deleting user with id: {id}");

            var user = await _dbContext.Users.FindAsync(id);

            if(user == null)
            {
                _logger.LogWarning($"No user found with id {id}");
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(); //Tek sada izmijeni sve u bazi, do sada je samo u kodu

            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<UserModel>> Create( UserInputModel model)
        {
            _logger.LogInformation($"Creating a new user with name: {model.Name}");

            var user = new Domain.User();

            model.MapTo(user);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            //Od novog korisnika user kojeg je unio korisnik kreiramo novi view koji cemo vratiti da se prikaze
            var resultModel = UserModel.FromUser(user);

            return CreatedAtAction(nameof(GetById), "users", new { id = user.Id}, resultModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserModel>> Update (long id, UserInputModel model)
        {
            _logger.LogInformation($"Updateing a user with id: {id}");

            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning($"No user found with id {id}");
                return NotFound();
            }

            model.MapTo(user);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return UserModel.FromUser(user);

        }

    }
}