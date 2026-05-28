using LightCap.InvestmentApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace  LightCap.InvestmentApi.Infrastructure.Persistence.DbContexts;

//UNCOMMENT LATER WHEN WE HAVE ENTITIES TO ADD
public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    //, IAppDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserLogin> UserLogins { get; set; }
    public DbSet<Otp> Otps { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    private static string Normalize(string s) => s.Replace(" ", "").Replace("-", "");
}
