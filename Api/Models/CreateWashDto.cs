using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class CreateWashDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public bool StartNow { get; set; }
    }
}
