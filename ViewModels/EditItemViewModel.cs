using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CollectionApp.ViewModels
{
    public class EditItemViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Item name")]
        [StringLength(50, ErrorMessage = "Collection name must have a minimum of 5 or a maximum of 50 characters", MinimumLength = 5)]
        [DataType(DataType.Text)]
        public string ItemName { get; set; }

        public byte[] Image { get; set; }
        public IFormFile NewImage { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(500, ErrorMessage = "Description must have a minimum of 10 or a maximum of 500 characters", MinimumLength = 10)]
        [Display(Name = "Description")]
        public string Text { get; set; }
    }
}
