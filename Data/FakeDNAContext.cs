using FakeDNA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FakeDNA.Data;

public class FakeDNAContext : IdentityDbContext<FakeDNAUser>
{
    public FakeDNAContext(DbContextOptions<FakeDNAContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<FakeDNA.Models.TimeOff> TimeOff { get; set; } = default!;
}
