using System.ComponentModel.DataAnnotations;

namespace FaceLogin.Models
{
    public class LoginViewModel 
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Not a valid email")]
        public string LEmail { get; set; }
 
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8,ErrorMessage = "Min length is 8.")]
        public string LPassword { get; set; }

    }
}