using Microsoft.EntityFrameworkCore;

namespace EasyKanjiServer.Models
{
    public class DBContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<User> Users { get; set; }
        public DbSet<Kanji> Kanjis { get; set; }

        public DBContext(DbContextOptions<DBContext> options, IConfiguration configuration) : base(options) 
        {
            _configuration = configuration;
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword(_configuration["AuthOptions:AdminPassword"] + _configuration["AuthOptions:PEPPER"]), Role = "Admin" });
            modelBuilder.Entity<Kanji>().HasData(new Kanji { Id = 1, Writing = "人", OnReadings = "ニン,ジン", KunReadings = "ひと", Meaning = "Человек" });
        }
    }
}
