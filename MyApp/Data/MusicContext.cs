using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Data
{
    public class MusicContext
    {
        public class MusicDbContext : DbContext
        {
            public MusicDbContext(DbContextOptions<MusicDbContext> options) : base(options) { }

            public DbSet<MusicModel> Music { get; set; }
        }
    }
}
