using System.ComponentModel.DataAnnotations;

namespace EasyKanjiServer.Models.DTOs
{
    public class UserPutDTO
    {
        public int Id { get; set; }
        [MinLength(2, ErrorMessage = "Username length can't be less than 2.")]
        [MaxLength(16, ErrorMessage = "Username length can't be more than 16.")]
        public string Username { get; set; } = string.Empty;
        [MinLength(6, ErrorMessage = "Password length can't be less than 6.")]
        [MaxLength(24, ErrorMessage = "Password length can't be more than 24.")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Role can't be unspecified.")]
        public string Role { get; set; } = string.Empty;
        public List<Kanji> Kanjis { get; set; } = new();
    }
}
