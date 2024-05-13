using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FakeDNA.Models;
using FakeDNA.Data;

[Authorize]
public class AdminController : ControllerBase
{
    private readonly UserManager<FakeDNAUser> _userManager;
    private readonly  FakeDNAContext _context;

    public AdminController(UserManager<FakeDNAUser> userManager, FakeDNAContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // Get all submitted requests that are pending approval
    [HttpGet("pending-requests")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var requests = await _context.TimeOff
                                     .Where(x => x.Status == TimeOffStatus.Submitted)
                                     .ToListAsync();
        return Ok(requests);
    }

    // Approve a specific time off request
    [HttpPost("approve/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveRequest(Guid id)
    {
        var request = await _context.TimeOff.FindAsync(id);
        if (request == null)
        {
            return NotFound();
        }
        request.Status = TimeOffStatus.Approved;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Reject a specific time off request
    [HttpPost("reject/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectRequest(Guid id)
    {
        var request = await _context.TimeOff.FindAsync(id);
        if (request == null)
        {
            return NotFound();
        }
        request.Status = TimeOffStatus.Rejected;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
