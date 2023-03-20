using Microsoft.EntityFrameworkCore;

namespace EasyKanjiServer.Models
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Kanji> Kanjis { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options) 
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Username = "admin", PasswordHash = "admin" });
            modelBuilder.Entity<Kanji>().HasData(new Kanji { Id = 1, Writing = "人", OnReadings = "ニン,ジン", KunReadings = "ひと", Meaning = "Человек" });
        }
    }
}
