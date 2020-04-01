using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionApp.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [Required]
        public string CollectionName { get; set; }
        public string CreationDate { get; set; }
        public int Likes { get; set; }
        public byte[] Image { get; set; }
        public int ItemCount { get; set; }


    }
}
