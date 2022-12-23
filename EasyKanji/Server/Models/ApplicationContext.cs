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
                new Kanji("人", "Человек", "ニン0ジン", "ひと", "ひと1человек; люди; личность, характер; другой, другие; кто [-нибудь] , кто-то0ひと.となり1натура, характер Чаще 為人0ひと.らしい1достойный [звания человека]") { Id = 1 },
                new Kanji("一", "Один", "イチ0イツ", "ひと.つ", "いち1один0ひと.つ1~[no] один; ~[no] один и тот же; ~[ni] во-первых, прежде всего; разок; немножко0いつ1одно [целое]0いつ.に1всецело, целиком; частично") { Id = 2 }
                );

            }
        }
    }
}
