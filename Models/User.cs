using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CollectionApp.Models
{
    public class User : IdentityUser
    {
        public string RegisterDate { get; set; }

        public string LoginDate { get; set; }
        public string Status { get; set; }

        public string Block { get; set; }

        public string Role { get; set; }
    }
}
