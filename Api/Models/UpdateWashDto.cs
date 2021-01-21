using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class UpdateWashDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool StartNow { get; set; }

        [Required]
        public bool Abort { get; set; }
    }
}
