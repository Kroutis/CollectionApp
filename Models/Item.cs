using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CollectionApp.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        [Required]
        public string ItemName { get; set; }
        public int Likes { get; set; }
        public byte[] Image { get; set; }
        public string UserName { get; set; }
        [Required]

        public string Text { get; set; }
    }
}
