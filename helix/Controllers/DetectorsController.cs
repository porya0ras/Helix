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
    public class DetectorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DetectorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Detectors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Detector>>> GetDetectors()
        {
          if (_context.Detectors == null)
          {
              return NotFound();
          }
            return await _context.Detectors.ToListAsync();
        }

        // GET: api/GetDropDownDetectors
        [HttpGet("GetDropDownDetectors")]
        public async Task<ActionResult<IEnumerable<KeyValuePair<long,string>>>> GetDropDownDetectors()
        {
            if (_context.Detectors == null)
            {
                return NotFound();
            }
            return await _context.Detectors.Select(e=>new KeyValuePair<long, string>(e.Id,e.Name)).ToListAsync();
        }

        // GET: api/Detectors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Detector>> GetDetector(long id)
        {
          if (_context.Detectors == null)
          {
              return NotFound();
          }
            var detector = await _context.Detectors.FindAsync(id);

            if (detector == null)
            {
                return NotFound();
            }

            return detector;
        }



        // PUT: api/Detectors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,OPERATORS")]
        public async Task<IActionResult> PutDetector(long id, Detector detector)
        {
            if (id != detector.Id)
            {
                return BadRequest();
            }

            _context.Entry(detector).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetectorExists(id))
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

        // POST: api/Detectors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "ADMIN,OPERATORS")]
        public async Task<ActionResult<Detector>> PostDetector(Detector detector)
        {
          if (_context.Detectors == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Detectors'  is null.");
          }
            _context.Detectors.Add(detector);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetector", new { id = detector.Id }, detector);
        }

        // DELETE: api/Detectors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,OPERATORS")]
        public async Task<IActionResult> DeleteDetector(long id)
        {
            if (_context.Detectors == null)
            {
                return NotFound();
            }
            var detector = await _context.Detectors.FindAsync(id);
            if (detector == null)
            {
                return NotFound();
            }

            _context.Detectors.Remove(detector);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetectorExists(long id)
        {
            return (_context.Detectors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
