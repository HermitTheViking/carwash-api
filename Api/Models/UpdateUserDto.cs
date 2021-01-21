using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class UpdateUserDto
    {
        [Required]
        public string Name { get; set; }
    }
}
