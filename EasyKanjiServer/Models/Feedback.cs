namespace EasyKanjiServer.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Body { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int? UserId { get; set; } 
        public User? User { get; set; }
    }
}
