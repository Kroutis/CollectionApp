using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionApp.Models;

namespace CollectionApp.ViewModels
{
    public class CollectionViewModel
    {
        public Collection collection { get; set; }
        public string UserName { get; set; }
        public int CollectionId { get; set; }
        public string Role_2 { get; set; }
        public List<Item> Items { get; set; }
    }
}
