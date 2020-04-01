using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionApp.Models;

namespace CollectionApp.ViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }
        public ItemComment Comment { get; set; }
        public List<ItemComment> ItemComments { get; set; }
    }
}
