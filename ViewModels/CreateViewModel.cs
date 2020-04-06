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
        [Display(Name = "Collection name")]
        [StringLength(50, ErrorMessage = "Collection name must have a minimum of 5 or a maximum of 50 characters", MinimumLength = 5)]
        [DataType(DataType.Text)]
        public string CollectionName { get; set; }
        public string UserName { get; set; }
        
        public string CreationDate { get; set; }
        public int Likes { get; set; }
        public IFormFile Image { get; set; }
        public int ItemCount { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(500, ErrorMessage = "Description must have a minimum of 10 or a maximum of 500 characters", MinimumLength = 10)]
        [Display(Name = "Description")]
        public string Text { get; set; }
    }
}
