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
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Item name must have a minimum of 3 or a maximum of 50 characters", MinimumLength = 3)]
        [Display(Name = "Item name")]
        public string ItemName { get; set; }
        public int Likes { get; set; }
        public IFormFile Image { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(500, ErrorMessage = "Description must have a minimum of 10 or a maximum of 500 characters", MinimumLength = 10)]
        [Display(Name = "Description")]
        public string Text { get; set; }
    }
}
