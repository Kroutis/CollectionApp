using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;



namespace CollectionApp.ViewModels
{
    public class AddItemViewModel
    {
       public int CollectionId { get; set; }

        [Required]
        public string ItemName { get; set; }
        public int Likes { get; set; }
        public IFormFile Image { get; set; }
        public string UserName { get; set; }
    }
}
