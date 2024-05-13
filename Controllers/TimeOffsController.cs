using FakeDNA.Data;
using FakeDNA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;


namespace FakeDNA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class TimeOffsController(FakeDNAContext context, UserManager<FakeDNAUser> userManager) : ControllerBase
    {
        // GET: api/TimeOffs
        [HttpGet(Name = "GetMyLeaves")]
        public async Task<ActionResult<IEnumerable<TimeOff>>> GetLeaves()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Create an account with POST /api/Account/Create");
            }
            await context.Entry(user).Collection(u => u.TimeOffs).LoadAsync();
            return user.TimeOffs.ToList();
        }

        // GET: api/TimeOffs/FutureLeaves
        [HttpGet("FutureLeaves", Name = "GetMyFutureLeaves")]
        public async Task<ActionResult<IEnumerable<TimeOff>>> GetFutureLeaves(int numberOfDays = 0)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Create an account with POST /api/Account/Create");
            }

            var endSearch = numberOfDays == 0 ? DateTime.MaxValue : DateTime.Now.AddDays(numberOfDays);
            return await context.TimeOff.Where(timeOff => timeOff.Requester == user && timeOff.StartDate >= DateTime.Now && timeOff.StartDate < endSearch).ToListAsync();
        }


        // GET: api/TimeOffs/5
        [HttpGet("{id}", Name = "GetLeave")]
        public async Task<ActionResult<TimeOff>> GetTimeOff(Guid id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Create an account with POST /api/Account/Create");
            }

            var timeOff = await context.TimeOff.FindAsync(id);

            if (timeOff == null || timeOff.Requester != user)
            {
                return NotFound();
            }

            return timeOff;
        }

        // POST: api/TimeOffs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpDelete("{id}", Name = "CancelLeave")]
        public async Task<IActionResult> RequestTimeOffCancellation(Guid id, string cancellationMessage)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Create an account with POST /api/Account/Create");
            }
            var timeOff = await context.TimeOff.FindAsync(id);
            if (timeOff == null || timeOff.Requester != user)
            {
                return NotFound();
            }

            timeOff.Status = TimeOffStatus.RequestCancellation;

            context.Entry(timeOff).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeOffExists(id))
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


        // POST: api/TimeOffs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(Name = "RequestNewLeave")]
        public async Task<ActionResult<TimeOff>> RequestLeave(TimeOffRequest timeOffRequest)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Create an account with POST /api/Account/Create");
            }

            var timeOff = new TimeOff
            {
                Id = Guid.NewGuid(),
                Reason = timeOffRequest.Reason,
                StartDate = timeOffRequest.StartDate,
                StartDayPeriod = timeOffRequest.StartPeriod,
                EndDate = timeOffRequest.EndDate,
                EndDayPeriod = timeOffRequest.EndPeriod,
                Status = TimeOffStatus.Submitted,
                Requester = user
            };

            if(timeOff.Reason == TimeOffReason.PaidLeave && timeOff.Duration > user.PaidLeaveRemaining)
            {
                return BadRequest($"Not enough paid leave days remaining. Only {user.PaidLeaveRemaining} out of {user.PaidLeaveTotal} left");
            }
            if (timeOff.Reason == TimeOffReason.HolidayCompensation && timeOff.Duration > user.CompensationLeaveRemaining)
            {
                return BadRequest($"Not enough public holiday compensation days remaining. Only {user.CompensationLeaveRemaining} out of {user.CompensationLeaveTotal} left");
            }

            context.TimeOff.Add(timeOff);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(RequestLeave), timeOff);
        }

        private bool TimeOffExists(Guid id)
        {
            return context.TimeOff.Any(e => e.Id == id);
        }
    }
}
