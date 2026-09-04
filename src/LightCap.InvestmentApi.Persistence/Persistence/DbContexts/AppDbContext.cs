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
    public DbSet<LinkedBankAccount> LinkedBankAccounts { get; set; }
    public DbSet<Wallets> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    private static string Normalize(string s) => s.Replace(" ", "").Replace("-", "");
}
