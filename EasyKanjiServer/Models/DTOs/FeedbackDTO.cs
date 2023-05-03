using System.ComponentModel.DataAnnotations;

namespace EasyKanjiServer.Models.DTOs
{
    public class FeedbackDTO
    {
        [Required]
        public string Body { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}
