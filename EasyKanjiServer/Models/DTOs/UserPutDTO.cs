namespace EasyKanjiServer.Models.DTOs
{
    public class UserPutDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<Kanji> Kanjis { get; set; } = new();
    }
}
