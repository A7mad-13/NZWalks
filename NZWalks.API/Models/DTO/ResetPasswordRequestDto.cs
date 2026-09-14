using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
