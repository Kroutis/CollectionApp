using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CollectionApp.ViewModels
{
    public class ProfileControllerViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public string RegisterDate { get; set; }
        public string LoginDate { get; set; }

        public string Status { get; set; }

        public string Role { get; set; }

        public string Role_2 { get; set; }
    }
}
