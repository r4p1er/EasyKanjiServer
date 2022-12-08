using Microsoft.EntityFrameworkCore;

namespace EasyKanji.Server.Models
{
    public class ApplicationContext : DbContext
    {
        IWebHostEnvironment Environment { get; }
        
        public DbSet<Kanji> Kanjis { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options, IWebHostEnvironment environment) : base(options) 
        {
            Environment = environment;
            
            if (environment.IsDevelopment())
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Environment.IsDevelopment())
            {
                modelBuilder.Entity<Kanji>().HasData(
                new Kanji("人", "Человек", "ニン0ジン", "ひと1человек0ひと.なり1саняяя") { Id = 1 },
                new Kanji("然", "Так", "ゼン", "しか.し1однако, тем не менее0しか.しながら1однако, тем не менее0そ.う1так") { Id = 2 }
                );
            }
        }
    }
}
