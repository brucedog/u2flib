using System.ComponentModel.DataAnnotations;

namespace DemoU2FSite.Models
{    
    public class BeginLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class CompleteLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Token response")]
        public string TokenResponse { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Token")]
        public string TokenResponse { get; set; }

        [Display(Name = "Challenge")]
        public string Challenge { get; set; }

        [Display(Name = "Version")]
        public string Version { get; set; }
        
        [Display(Name = "App ID")]
        public string AppId { get; set; }
    }
}