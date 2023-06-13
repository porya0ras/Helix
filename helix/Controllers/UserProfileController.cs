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
using static Duende.IdentityServer.IdentityServerConstants;
using Microsoft.AspNetCore.Identity;

namespace helix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserProfileController> _loger;
        private readonly UserManager<User> _userManager;



        public UserProfileController(
            ApplicationDbContext context,
        ILogger<UserProfileController> loger,
        UserManager<User> userManager)
        {
            _context = context;
            _loger=loger;
            _userManager = userManager;
        }
        /// <summary>
        /// دریافت رسد های خود کاربر
        /// </summary>
        [HttpGet("GetUserObservationSubmissions")]
        public async Task<IEnumerable<ObservationSubmission>> GetUserObservationSubmissions()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            return await _context.ObservationSubmissions.Where(e => e._User.Id == userId).ToListAsync();
        }
        [HttpGet("GetUser")]
        public async Task<ActionResult<dynamic>> GetUser()
        {
            var id = _userManager.GetUserId(HttpContext.User);
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.Select(e=>new {e.Id,e.UserName,e.Email, e.Type,e.FirstName,e.LastName,e.Institution,e.PhoneNumber,e.EmailConfirmed,e.PhoneNumberConfirmed }).Where(e=>e.Id==id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut]

        public async Task<IActionResult> PutUser(User user)
        {
            var id = _userManager.GetUserId(HttpContext.User);

            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

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

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}