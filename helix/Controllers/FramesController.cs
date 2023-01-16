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

namespace helix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FiltersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Frames
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Filter>>> GetFrames()
        {
            if (_context.Filters == null)
            {
                return NotFound();
            }
            return await _context.Filters.ToListAsync();
        }
        // GET: api/GetDropDownDetectors
        [HttpGet("GetDropDownDetectors")]
        public async Task<ActionResult<IEnumerable<KeyValuePair<long, string>>>> GetDropDownFrames()
        {
            if (_context.Filters == null)
            {
                return NotFound();
            }
            return await _context.Filters.Select(e => new KeyValuePair<long, string>(e.Id, e.Name)).ToListAsync();
        }

        // GET: api/Frames/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Filter>> GetFrame(long id)
        {
            if (_context.Filters == null)
            {
                return NotFound();
            }
            var frame = await _context.Filters.FindAsync(id);

            if (frame == null)
            {
                return NotFound();
            }

            return frame;
        }

        // PUT: api/Frames/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,OPERATORS")]
        public async Task<IActionResult> PutFrame(long id, Filter frame)
        {
            if (id != frame.Id)
            {
                return BadRequest();
            }

            _context.Entry(frame).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FrameExists(id))
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

        // POST: api/Frames
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "ADMIN,OPERATORS")]
        public async Task<ActionResult<Filter>> PostFrame(Filter frame)
        {
            if (_context.Filters == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Frames'  is null.");
            }
            _context.Filters.Add(frame);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFrame", new { id = frame.Id }, frame);
        }

        // DELETE: api/Frames/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,OPERATORS")]
        public async Task<IActionResult> DeleteFrame(long id)
        {
            if (_context.Filters == null)
            {
                return NotFound();
            }
            var frame = await _context.Filters.FindAsync(id);
            if (frame == null)
            {
                return NotFound();
            }

            _context.Filters.Remove(frame);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FrameExists(long id)
        {
            return (_context.Filters?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
