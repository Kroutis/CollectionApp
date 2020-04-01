using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CollectionApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "UserName")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }
        [Required]
        [DataType(DataType.Text)]

        public string RegisterDate = DateTime.Now.ToString("yyyy-MM-dd");
        [Required]
        [DataType(DataType.Text)]

        public string LoginDate = DateTime.Now.ToString("yyyy-MM-dd");
        [Required]
        [DataType(DataType.Text)]

        public string Status = "Online";
        [Required]
        [DataType(DataType.Text)]
        public string Block = "false";
        [Required]
        [DataType(DataType.Text)]
        public string Role = null;
    }
}
