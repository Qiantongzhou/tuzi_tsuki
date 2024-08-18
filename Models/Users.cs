using System.ComponentModel.DataAnnotations;

namespace tuzi_tsuki.Models
{
    public class Users
    {
        [Display(Name = "Full name")]
        public required string FullName { get; set; }
        [Display(Name = "Full name")]
        public required string Password { get; set; }
        [Display(Name = "EmailAddress")]
        public required string EmailAddress { get; set; }
    }
}
