using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
                if (!File.Exists("./Models/kanjis.json"))
                {
                    Console.WriteLine(Directory.GetCurrentDirectory());
                    throw new Exception();
                }
                else
                {
                    var jsonFile = new StreamReader("./Models/kanjis.json");
                    string json = jsonFile.ReadToEnd();
                    jsonFile.Close();
                    var document = JsonDocument.Parse(json);
                    Kanji[] kanjis = new Kanji[document.RootElement.GetArrayLength()];
                    for (int i = 0; i < document.RootElement.GetArrayLength(); ++i)
                    {
                        string writing = document.RootElement[i].GetProperty("writing").GetString()!;
                        string meaning = document.RootElement[i].GetProperty("meaning").GetString()!;
                        string onReadings = "";
                        foreach (var on in document.RootElement[i].GetProperty("onreadings").EnumerateArray())
                        {
                            onReadings += on.ToString() + "0";
                        }
                        onReadings = onReadings.Substring(0, onReadings.Length - 1);
                        string kunReadings = "";
                        foreach (var kun in document.RootElement[i].GetProperty("kunreadings").EnumerateArray())
                        {
                            kunReadings += kun.ToString() + "0";
                        }
                        kunReadings = kunReadings.Substring(0, kunReadings.Length - 1);
                        string words = "";
                        if (document.RootElement[i].GetProperty("words").GetArrayLength() != 0)
                        {
                            foreach (var dict in document.RootElement[i].GetProperty("words").EnumerateArray())
                            {
                                foreach (var p in dict.EnumerateObject())
                                {
                                    words += p.Name + "1" + p.Value + "0";
                                }
                            }
                            words = words.Substring(0, words.Length - 1);
                        }
                        var kanji = new Kanji(writing, meaning, onReadings, kunReadings, words) { Id = i + 1 };
                        kanjis[i] = kanji;
                    }
                    modelBuilder.Entity<Kanji>().HasData(kanjis);
                }
            }
        }
    }
}
