using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CollectionApp.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        [Display(Name = "Collection Name")]
        [DataType(DataType.Text)]
        public string CollectionName { get; set; }
        public string UserName { get; set; }
        
        public string CreationDate { get; set; }
        public int Likes { get; set; }
        public IFormFile Image { get; set; }
        public int ItemCount { get; set; }
    }
}
