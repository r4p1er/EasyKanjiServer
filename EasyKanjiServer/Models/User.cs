namespace EasyKanjiServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set;} = string.Empty;
        public List<Kanji> Kanjis { get; set; } = new();
        public List<Report> Reports { get; set; } = new();
    }
}
