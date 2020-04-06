using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionApp.Models;

namespace CollectionApp.ViewModels
{
    public class UsersViewModel
    {
        public List<User> Users { get; set; }
        public string Role { get; set; }
    }
}
