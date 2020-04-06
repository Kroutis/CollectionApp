using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CollectionApp.ViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(200, ErrorMessage = "Comment must have a minimum of 2 or a maximum of 200 characters", MinimumLength = 2)]
        [Display(Name = "Comment")]
        public string Text { get; set; }
        public ItemComment Comment { get; set; }
        public List<ItemComment> ItemComments { get; set; }
        public string UserName { get; set; }
        public string Role_2 { get; set; }
    }
}
