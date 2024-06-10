using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiAccountMVP.Models;

namespace MultiAccountMVP;

public class MainDbContext: IdentityDbContext<Account, RoleModel, long>
{
    public MainDbContext(DbContextOptions<MainDbContext> dbContextOptions) : base(dbContextOptions)
    {
        this.Database.EnsureCreated();
    }
    public DbSet<DevAccount> DevAccounts { get; protected init; } = null!;
}