using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CollectionApp.Models
{
    public class CollectionContext: DbContext
    {
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemComment> ItemComments { get; set; }
        public CollectionContext(DbContextOptions<CollectionContext> options)
            :base(options)
        {
            Database.EnsureCreated();
        }
    }
}
