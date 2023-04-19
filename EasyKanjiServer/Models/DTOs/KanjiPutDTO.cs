using System.ComponentModel.DataAnnotations;

namespace EasyKanjiServer.Models.DTOs
{
    public class KanjiPutDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Writing can't be unspecified.")]
        public string Writing { get; set; } = string.Empty;
        public string OnReadings { get; set; } = string.Empty;
        public string KunReadings { get; set; } = string.Empty;
        [Required(ErrorMessage = "Meaning can't be unspecified.")]
        public string Meaning { get; set; } = string.Empty;
    }
}
