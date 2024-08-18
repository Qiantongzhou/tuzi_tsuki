using System.ComponentModel.DataAnnotations;

namespace tuzi_tsuki.Models
{
    public class LoginVM
    {
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "需要用户名")]
        [DataType(DataType.Text)]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "需要密码")]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
