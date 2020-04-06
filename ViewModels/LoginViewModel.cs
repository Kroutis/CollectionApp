using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CollectionApp.ViewModels
{

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        public bool RememberMe = false;

        //public string ReturnUrl { get; set; }
        [Required]
        [DataType(DataType.Text)]

        public string LoginDate = DateTime.Now.ToString("yyyy-MM-dd");
        [Required]
        [DataType(DataType.Text)]

        public string Status = "Online";
    }
}