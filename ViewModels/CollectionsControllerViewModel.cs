using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionApp.Models;

namespace CollectionApp.ViewModels
{
    public class CollectionsControllerViewModel
    {
        public string UserName { get; set; }
        public string Role_2 { get; set; }
        public List<Collection> Collections { get; set; }
    }
}
