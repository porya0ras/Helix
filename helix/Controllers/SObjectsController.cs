using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using helix.Data;
using helix.Models;
using helix.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace helix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SObjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SObjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SObjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SObject>>> GetSObjects()
        {
            if (_context.SObjects == null)
            {
                return NotFound();
            }
            return await _context.SObjects.ToListAsync();
        }

        // GET: api/GetDropDownDetectors
        [HttpGet("GetDropDownSObjects")]
        public async Task<ActionResult<IEnumerable<KeyValuePair<long, string>>>> GetDropDownSObjects()
        {
            if (_context.SObjects == null)
            {
                return NotFound();
            }
            return await _context.SObjects.Select(e => new KeyValuePair<long, string>(e.Id, e.Name)).ToListAsync();
        }

        // GET: api/SObjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SObject>> GetSObject(long id)
        {
            if (_context.SObjects == null)
            {
                return NotFound();
            }
            var sObject = await _context.SObjects.FindAsync(id);

            if (sObject == null)
            {
                return NotFound();
            }

            return sObject;
        }

        // PUT: api/SObjects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,OPERATOR")]
        public async Task<IActionResult> PutSObject(long id, SObject sObject)
        {
            if (id != sObject.Id)
            {
                return BadRequest();
            }

            _context.Entry(sObject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SObjectExists(id))
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

        // POST: api/SObjects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "ADMIN,OPERATOR")]
        public async Task<ActionResult<SObject>> PostSObject(SObjectVModel sObject)
        {
            if (_context.SObjects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SObjects'  is null.");
            }


            if (sObject==null)
            {
                return BadRequest("Data Is Null!");
            }
            var ExistData = await _context.SObjects.Where(e => e.Name==sObject.Name).FirstOrDefaultAsync();

            if (ExistData!=null)
                return CreatedAtAction("GetSObject", new { id = ExistData.Id }, ExistData);

            var _new = new SObject()
            {
                Id = sObject.Id,
                Name= sObject.Name,
                RA=sObject.RA,
                DEC=sObject.DEC,
            };

            _context.SObjects.Add(_new);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSObject", new { id = _new.Id }, _new);
        }

        // DELETE: api/SObjects/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,OPERATOR")]
        public async Task<IActionResult> DeleteSObject(long id)
        {
            if (_context.SObjects == null)
            {
                return NotFound();
            }
            var sObject = await _context.SObjects.FindAsync(id);
            if (sObject == null)
            {
                return NotFound();
            }

            _context.SObjects.Remove(sObject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SObjectExists(long id)
        {
            return (_context.SObjects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
