using Microsoft.EntityFrameworkCore;

using ITDC.Models;

namespace ITDC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //public DbSet<ListenModel> Listen { get; set; }
    }
}
