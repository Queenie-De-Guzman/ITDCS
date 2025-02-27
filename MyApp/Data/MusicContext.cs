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
            public DbSet<PlaylistModel> Playlists { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<MusicModel>()
                    .HasOne(m => m.Playlist)
                    .WithMany(p => p.Music)
                    .HasForeignKey(m => m.PlaylistId)
                    .OnDelete(DeleteBehavior.SetNull);  // Ensures Music isn't deleted when Playlist is removed
            }
        }
    }
}
