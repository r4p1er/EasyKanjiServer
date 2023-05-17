using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EasyKanjiServer.Models
{
    public class DBContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<User> Users { get; set; }
        public DbSet<Kanji> Kanjis { get; set; }
        public DbSet<Feedback> Feedback { get; set; }

        public DBContext(DbContextOptions<DBContext> options, IConfiguration configuration) : base(options) 
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword(_configuration["AuthOptions:AdminPassword"] + _configuration["AuthOptions:PEPPER"]), Role = "Admin" });
            var kanjis = new List<Kanji>();

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "kanjis.json")))
            {
                using (var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "kanjis.json"), System.Text.Encoding.Unicode))
                {
                    using (var json = JsonDocument.Parse(reader.ReadToEnd()))
                    {
                        int i = 1;

                        foreach (var item in json.RootElement.EnumerateArray())
                        {
                            kanjis.Add(new Kanji { Id = i++, Writing = item.GetProperty("writing").GetString() ?? "", KunReadings = string.Join(',', item.GetProperty("kunreadings").EnumerateArray()), OnReadings = string.Join(',', item.GetProperty("onreadings").EnumerateArray()), Meaning = item.GetProperty("meaning").GetString() ?? "" });
                        }
                    }
                }
            }

            if (kanjis.Count > 0)
            {
                modelBuilder.Entity<Kanji>().HasData(kanjis);
            }
            else
            {
                modelBuilder.Entity<Kanji>().HasData(new Kanji { Id = 1, Writing = "人", OnReadings = "ニン,ジン", KunReadings = "ひと", Meaning = "Человек" });
            }
        }
    }
}
