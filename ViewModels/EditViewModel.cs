using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CollectionApp.ViewModels
{
    public class EditViewModel
    {
        public int Id { get; set; }
        [Required]
        public string CollectionName { get; set; }

        public byte[] Image { get; set; }
        public IFormFile NewImage { get; set; }
    }
}
