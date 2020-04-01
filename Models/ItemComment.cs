using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionApp.Models
{
    public class ItemComment
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
    }
}
