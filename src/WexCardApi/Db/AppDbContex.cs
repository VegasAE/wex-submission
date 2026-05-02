using Microsoft.EntityFrameworkCore;
using WexCardApi.Models;

namespace WexCardApi.DB;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Card> Cards { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}
