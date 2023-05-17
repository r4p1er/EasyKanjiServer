namespace EasyKanjiServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set;} = string.Empty;
        public string RefreshToken { get; set;} = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public List<Kanji> Kanjis { get; set; } = new();
        public List<Feedback> Feedback { get; set; } = new();
    }
}
