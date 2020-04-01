using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionApp.Models;

namespace CollectionApp.ViewModels
{
    public class CollectionViewModel
    {
        public int CollectionId { get; set; }
        public List<Item> Items { get; set; }
    }
}
