using System.ComponentModel.DataAnnotations;

namespace DemoU2FSite.Models
{    
    public class BeginLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class CompleteLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "App id")]
        public string AppId { get; set; }

        [Required]
        [Display(Name = "Key Handel")]
        public string KeyHandle { get; set; }

        [Required]
        [Display(Name = "Version")]
        public string Version { get; set; }

        [Required]
        [Display(Name = "Device Response")]
        public string DeviceResponse { get; set; }

        [Required]
        [Display(Name = "Challenge")]
        public string Challenge { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Challenge")]
        public string Challenge { get; set; }

        [Display(Name = "Version")]
        public string Version { get; set; }
        
        [Display(Name = "App ID")]
        public string AppId { get; set; }
    }

    public class CompleteRegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Challenge")]
        public string Challenge { get; set; }

        [Display(Name = "Version")]
        public string Version { get; set; }

        [Display(Name = "App ID")]
        public string AppId { get; set; }

        public string DeviceResponse { get; set; }
    }
}