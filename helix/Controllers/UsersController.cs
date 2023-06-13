using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using helix.Data;
using helix.Models;
using Microsoft.AspNetCore.Authorization;
using helix.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace helix.Controllers
{

    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _UserManager;
        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _UserManager=userManager;
        }

        // GET: api/Users

        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.Select(e => new { e.Id, e.Email, e.FirstName, e.LastName, e.UserName }).ToListAsync();
        }

        // GET: api/Users/5

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, UserVM user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            var dbuser = _context.Users.Where(e => e.Id==id).FirstOrDefault();

            if(dbuser == null)
                return NotFound();

            //todo check change username
            dbuser.UserName=user.Username;
            dbuser.FirstName=user.Firstname;
            dbuser.LastName=user.Lastname;
            dbuser.Institution=user.Institution;
            dbuser.Type=user.Type;


            await _UserManager.ChangePasswordAsync(dbuser, user.CurrentPassword, user.Password);

           _context.Entry(dbuser).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: api/GetDropDownDetectors
        [HttpGet("api/Users/GetDropDownTelescopes")]
        public async Task<ActionResult<IEnumerable<KeyValuePair<string, string>>>> GetDropDownTelescopes()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.Select(e => new KeyValuePair<string, string>(e.Id, e.FirstName+" "+e.LastName)).ToListAsync();
        }
    }
}
