using FakeDNA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace FakeDNA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class AccountController(UserManager<FakeDNAUser> userManager) : ControllerBase
    {
        [HttpPost("Create", Name = "CreateMyAccount")]
        public async Task<ActionResult<FakeDNAUser>> CreateAccount()
        {
            var user = await  userManager.GetUserAsync(User);
            if (user != null)
            {
                return BadRequest("You already have an account.");
            }

            var truc = await userManager.CreateAsync(new FakeDNAUser
            {
                CompensationLeaveTotal = 2,
                PaidLeaveTotal = 26,
                Id = User.GetNameIdentifierId(),
                Email = User.GetDisplayName(),
                UserName = User.GetDisplayName()
            });

            if(truc.Succeeded)
            {
                return await userManager.GetUserAsync(User);
            } else
            {
                return BadRequest(truc.Errors);
            }
        }

        [HttpGet("Counters", Name = "GetMyLeaveCounters")]
        public async Task<ActionResult<List<LeaveCounter>>> GetLeaveCounters()
        {
            var user = await userManager.Users.Include(u => u.TimeOffs).SingleOrDefaultAsync(u => u.Id == User.GetNameIdentifierId());
            if (user == null)
            {
                return BadRequest("Create an account with POST /api/Account/Create");
            }

            List<LeaveCounter> counters = [];
            counters.Add(new LeaveCounter { Reason = TimeOffReason.PaidLeave, Remaining = user.PaidLeaveRemaining, Total = user.PaidLeaveTotal, Amount = user.PaidLeaveAmount });
            counters.Add(new LeaveCounter { Reason = TimeOffReason.SickLeave, Remaining = default, Total = default, Amount = user.SickLeaveAmount });
            counters.Add(new LeaveCounter { Reason = TimeOffReason.HolidayCompensation, Remaining = user.CompensationLeaveRemaining, Total = user.CompensationLeaveTotal, Amount = user.CompensationLeaveAmount });
            return counters;
        }
    }
}
