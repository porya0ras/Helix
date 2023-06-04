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
using System.Security.Claims;
using helix.Services.interfaces;

namespace helix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObservationSubmissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ObservationSubmissionsController> _Loger;
        private readonly UserManager<User> _userManager;
        private readonly IBufferedFileUploadService _fileUploadService;

        public ObservationSubmissionsController(ApplicationDbContext context,
            ILogger<ObservationSubmissionsController> loger,
            UserManager<User> userManager,
            IBufferedFileUploadService fileUploadService)
        {
            _context = context;
            _Loger=loger;
            _userManager=userManager;
            _fileUploadService=fileUploadService;
        }

        // GET: api/ObservationSubmissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetObservationSubmissions()
        {
            if (_context.ObservationSubmissions == null)
            {
                return NotFound();
            }
            return await _context.ObservationSubmissions.Select(e => new
            {
                e.Id,
                e.Status,
                e.DateTime,
                DetectorName = e._Detector.Name,
                FilterName = e._Filter.Name,
                TelescopeName = e._Telescope.Name,
                e._SObject,
                e._User.UserName,
                e.Name,
                e.Type,
                e.SDateTime

            }).ToListAsync();
        }

        // GET: api/ObservationSubmissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<dynamic>> GetObservationSubmission(Guid id)
        {
            if (_context.ObservationSubmissions == null)
            {
                return NotFound();
            }
            var observationSubmission = await _context.ObservationSubmissions.Select(e => new
            {
                e.Id,
                e.Status,
                e.DateTime,
                DetectorName = e._Detector.Name,
                FrameName = e._Filter.Name,
                TelescopeName = e._Telescope.Name,
                e._SObject,
                e._User.UserName,
                e.SDateTime

            }).FirstAsync(e => e.Id==id);

            if (observationSubmission == null)
            {
                return NotFound();
            }

            return observationSubmission;
        }

        // PUT: api/ObservationSubmissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,OPERATOR")]
        public async Task<IActionResult> PutObservationSubmission(Guid id, ObservationSubmissionViewModel observationSubmission)
        {
            if (id != observationSubmission.Id)
            {
                return BadRequest();
            }

            var _detector = _context.Detectors.Where(e => e.Id==observationSubmission.DetectorId).First();
            var _telescope = _context.Telescopes.Where(e => e.Id==observationSubmission.TelescopeId).First();
            var _sObject = _context.SObjects.Where(e => e.Id==observationSubmission.SObjectId).First();
            var _frame = _context.Filters.Where(e => e.Id==observationSubmission.FilterId).First();

            var obs = await _context.FindAsync<ObservationSubmission>(id);

            if (obs == null)
            {
                return NotFound();
            }

            obs._SObject=_sObject;
            obs._Filter=_frame;
            obs._Detector=_detector;
            obs._Telescope=_telescope;
            obs.Status=observationSubmission.Status.ToString();
            obs.Name=observationSubmission.Name;
            obs.Type=observationSubmission.Type;
            obs.DateTime=observationSubmission.DateTime;
            obs.SDateTime=observationSubmission.SDateTime;

            _context.Entry(obs).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObservationSubmissionExists(id))
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

        // POST: api/ObservationSubmissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "ADMIN,OPERATOR")]
        public async Task<ActionResult<ObservationSubmission>> PostObservationSubmission(ObservationSubmissionViewModel observationSubmission)
        {
            try
            {
                if (_context.ObservationSubmissions == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.ObservationSubmissions'  is null.");
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var _user = _context.Users.Where(e => e.Id==userId).First();
                var _detector = _context.Detectors.Where(e => e.Id==observationSubmission.DetectorId).First();
                var _telescope = _context.Telescopes.Where(e => e.Id==observationSubmission.TelescopeId).First();
                var _sObject = _context.SObjects.Where(e => e.Id==observationSubmission.SObjectId).First();
                var _frame = _context.Filters.Where(e => e.Id==observationSubmission.FilterId).First();

                var _new = new ObservationSubmission()
                {
                    Id=Guid.NewGuid(),
                    DateTime = observationSubmission.DateTime,
                    SDateTime=observationSubmission.SDateTime,
                    _Detector= _detector,
                    Status="False",
                    _Filter= _frame,
                    _SObject= _sObject,
                    _User=_user,
                    _Telescope= _telescope,
                    Name=observationSubmission.Name,
                    Type=observationSubmission.Type,
                };


                _context.ObservationSubmissions.Add(_new);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetObservationSubmission", new { id = _new.Id },new { _new.Id,_new.Name,_new.Type,_new.DateTime,_new.Status,_new._Filter,_new._SObject,_new._Telescope,_new._Detector });
            }
            catch (Exception ex)
            {
                _Loger.LogError(ex, "Error in SeedData.");
                return BadRequest("Error in Add Data!");
            }
        }

        // DELETE: api/ObservationSubmissions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,OPERATOR")]
        public async Task<IActionResult> DeleteObservationSubmission(Guid id)
        {
            if (_context.ObservationSubmissions == null)
            {
                return NotFound();
            }
            var observationSubmission = await _context.ObservationSubmissions.FindAsync(id);
            if (observationSubmission == null)
            {
                return NotFound();
            }

            _context.ObservationSubmissions.Remove(observationSubmission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search([FromForm] string? FilterName,
            [FromForm] string? TelescopeName,
            [FromForm] string? SObjectName,
            [FromForm] string? DetectorName,
            [FromForm] DateTime? DateOf,
            [FromForm] DateTime? DateTo,
            [FromForm] string? User,
            [FromForm] string? Name,
            [FromForm] string? FrameType,
            [FromForm] int? SortByTelescope,
            [FromForm] int? SortByFrame,
            [FromForm] int? SortSObject,
            [FromForm] int? SortDetector,
            [FromForm] int? SortDate,
            [FromForm] int? radius,
             [FromForm] string? ra,
              [FromForm] string? dec)

        {
            if (_context.ObservationSubmissions == null)
            {
                return NotFound();
            }

            var result = _context.ObservationSubmissions.Where(e => e.Status=="True");


            //filtering
            if (!string.IsNullOrEmpty(FilterName))
            {
                result=result.Where(e => e._Filter.Name==FilterName);
            }
            if (!string.IsNullOrEmpty(TelescopeName))
            {
                result=result.Where(e => e._Telescope.Name==TelescopeName);
            }
            if (!string.IsNullOrEmpty(SObjectName))
            {
                result=result.Where(e => e._SObject.Name==SObjectName);
            }
            if (!string.IsNullOrEmpty(DetectorName))
            {
                result=result.Where(e => e._Detector.Name==DetectorName);
            }
            if (!string.IsNullOrEmpty(Name))
            {
                result=result.Where(e => e.Name==Name);
            }
            if (!string.IsNullOrEmpty(FrameType))
            {
                result=result.Where(e => e.Type==FrameType);
            }

            if (DateOf!=null && DateOf!=DateTime.MinValue)
            {
                result=result.Where(e => e.DateTime>=DateOf || e.SDateTime>=DateOf);
            }

            if (DateTo!=null && DateTo!=DateTime.MinValue)
            {
                result=result.Where(e => e.DateTime<=DateTo || e.SDateTime<=DateTo);
            }

            if (!string.IsNullOrEmpty(User))
            {
                result=result.Where(e => e._User.Id==User);
            }

            if(!string.IsNullOrEmpty(ra) && !string.IsNullOrEmpty(dec))
            {
                if (radius.HasValue && radius>=0)
                {
                    var RAs = ra.Split(' ');
                    var DECs = dec.Split(' ');
                    var _RA = (Convert.ToInt32(RAs[0])*15f*60f)+(Convert.ToInt32(RAs[1])*15f)+(Convert.ToInt32(RAs[2])*15f/60f);
                    var _DEC = (Convert.ToInt32(DECs[0])*60f)+Convert.ToInt32(DECs[1])+(Convert.ToInt32(DECs[2])/60f);

                    _RA=_RA-radius??0;

                    result =result.Where(e =>
                    (_RA-radius??0)<=(e._SObject.RA0*15f*60f)+(e._SObject.RA1*15f)+(e._SObject.RA2*15f/60f) &&
                    (_RA+radius??0)>=(e._SObject.RA0*15f*60f)+(e._SObject.RA1*15f)+(e._SObject.RA2*15f/60f) &&
                    (_DEC-radius??0)<=(e._SObject.DEC0*60f)+e._SObject.DEC1+(e._SObject.DEC2/60f) &&
                    (_DEC+radius??0)>=(e._SObject.DEC0*60f)+e._SObject.DEC1+(e._SObject.DEC2/60f)
                    );
                }
                else
                {
                    result = result.Where(e =>e._SObject.RA==ra && e._SObject.DEC==dec);
                }

            }




            //sorting
            if (SortByTelescope!=0)
            {
                if (SortByTelescope==1)
                {
                    result=result.OrderBy(e => e._Telescope.Name);
                }
                else if (SortByTelescope==-1)
                {
                    result=result.OrderByDescending(e => e._Telescope.Name);
                }
            }
            if (SortByFrame!=0)
            {
                if (SortByFrame==1)
                {
                    result=result.OrderBy(e => e._Filter.Name);
                }
                else if (SortByFrame==-1)
                {
                    result=result.OrderByDescending(e => e._Filter.Name);
                }
            }
            if (SortSObject!=0)
            {
                if (SortSObject==1)
                {
                    result=result.OrderBy(e => e._SObject.Name);
                }
                else if (SortSObject==-1)
                {
                    result=result.OrderByDescending(e => e._SObject.Name);
                }
            }
            if (SortDetector!=0)
            {
                if (SortDetector==1)
                {
                    result=result.OrderBy(e => e._Detector.Name);
                }
                else if (SortDetector==-1)
                {
                    result=result.OrderByDescending(e => e._Detector.Name);
                }
            }
            if (SortDate!=0)
            {
                if (SortDate==1)
                {
                    result=result.OrderBy(e => e.DateTime);
                }
                else if (SortDate==-1)
                {
                    result=result.OrderByDescending(e => e.DateTime);
                }
            }



            if (!result.Any())
            {
                return Ok(new object[0]);
            }

            int pagesize = 50;
            int pageNumber = 1;
            if (!string.IsNullOrEmpty(Request.Form["page"]))
            {
                pageNumber=Convert.ToInt32(Request.Form["page"].ToString());
            }


            var searchRes = await result.Select(e => new
            {
                e.Id,
                e.Name,
                e.Type,
                e.Status,
                e.DateTime,
                DetectorName = e._Detector.Name,
                FilterName = e._Filter.Name,
                TelescopeName = e._Telescope.Name,
                e._SObject,
                e._User.UserName,
                e.SDateTime

            })
            .OrderBy(e => e.DateTime)
            .Skip((pageNumber-1)*pagesize)
            .Take(pagesize)
            .ToListAsync();

            return Ok(searchRes);
        }

        private bool ObservationSubmissionExists(Guid id)
        {
            return (_context.ObservationSubmissions?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize]
        [HttpPost("FileUpload")]
        public async Task<IActionResult> FileUpload(string Id, List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            //todo: check size

            var result = await _fileUploadService.UploadFile(Id, files.ToArray());

            return Ok(new { count = files.Count, result, size });
        }

        [Authorize]
        [HttpPost("FileUploadAndAutoInsert")]
        public async Task<IActionResult> FileUploadAndAutoInsert(IFormFile[] files)
        {
            _Loger.LogInformation("Start!");

            var result = await _fileUploadService.UploadFileFits(files);

            foreach (var row in result)
            {
                if (row.Value!=null)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var _user = _context.Users.Where(e => e.Id==userId).First();
                    row.Value._User=_user;

                    _context.ObservationSubmissions.Add(row.Value);
                }
            }

            _context.SaveChanges();

            return Ok(result.Where(e=>e.Value!=null).Select(e => new KeyValuePair<int, object>(e.Key, new { e.Value.Id, e.Value.Name, e.Value.Status, e.Value.DateTime, e.Value.Type })));
        }

        [HttpPost("FileDelete")]
        public async Task<IActionResult> FileDelete(string Id, string fileName)
        {

            var result = await _fileUploadService.DeleteFile(Id, fileName);

            return Ok(result);
        }

        [HttpGet("GetFiles")]
        public async Task<IActionResult> GetFiles(string Id)
        {

            var result = await _fileUploadService.GetFiles(Id);

            return Ok(result);
        }

        [HttpGet("GetFITSAsImage")]
        public async Task<IActionResult> GetFITSAsImage(string Id, string Name)
        {
            var result = await _fileUploadService.GetFITSAsImage(Id, Name);
            return Ok(result);
        }


    }
}
