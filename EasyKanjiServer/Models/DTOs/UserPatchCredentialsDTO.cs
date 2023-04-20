using System.ComponentModel.DataAnnotations;

namespace EasyKanjiServer.Models.DTOs
{
    public class UserPatchCredentialsDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
