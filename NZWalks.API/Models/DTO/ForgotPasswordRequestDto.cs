using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
