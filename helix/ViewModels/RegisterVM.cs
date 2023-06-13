using System.ComponentModel.DataAnnotations;

namespace helix.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname   { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        public string Institution { get; set; }
    }
}
