using CollectionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionApp.ViewModels
{
    public class HomeControllerViewModel
    {
        public List<Item> Items { get; set; }
        public List<Collection> Collections { get; set; }
    }
}
