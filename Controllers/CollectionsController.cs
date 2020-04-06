using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CollectionApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CollectionApp.ViewModels;
using System.IO;

namespace CollectionApp.Controllers
{
    
    public class CollectionsController : Controller
    {
        private readonly CollectionContext db;
        private readonly UserManager<User> _userManager;
        public CollectionsController(UserManager<User> userManager, CollectionContext context)
        {
            _userManager = userManager;
            db = context;
        }
        public async Task<IActionResult> Index(string userName, CollectionSort sortOrder= CollectionSort.IdAsc)
        {
            //View(await db.Collections.ToListAsync());
            User user = null;
            if (userName != null)
            {
                user = await _userManager.FindByNameAsync(userName);
            }
            string role = null;
            if (User.Identity.Name != null)
            {
                User user_2 = await _userManager.FindByNameAsync(User.Identity.Name);
                role = user_2.Role;
            }
            if (user != null)
            {
                IQueryable<Collection> collections = db.Collections.Include(x=>x);
                ViewData["IdSort"] = sortOrder == CollectionSort.IdAsc ? CollectionSort.IdDesc : CollectionSort.IdAsc;
                ViewData["NameSort"] = sortOrder == CollectionSort.NameAsc ? CollectionSort.NameDesc : CollectionSort.NameAsc;
                ViewData["ItemSort"] = sortOrder == CollectionSort.ItemCountAsc ? CollectionSort.ItemCountDesc : CollectionSort.ItemCountAsc;
                ViewData["TextSort"] = sortOrder == CollectionSort.TextAsc ? CollectionSort.TextDesc : CollectionSort.TextAsc;
                collections = sortOrder switch
                {
                    CollectionSort.IdDesc => collections.OrderByDescending(s => s.Id),
                    CollectionSort.NameAsc=>collections.OrderBy(s=>s.CollectionName),
                    CollectionSort.NameDesc=>collections.OrderByDescending(s=>s.CollectionName),
                    CollectionSort.ItemCountAsc => collections.OrderBy(s => s.ItemCount),
                    CollectionSort.ItemCountDesc => collections.OrderByDescending(s => s.ItemCount),
                    CollectionSort.TextAsc => collections.OrderBy(s => s.Text),
                    CollectionSort.TextDesc => collections.OrderByDescending(s => s.Text),
                    _ =>collections.OrderBy(s=>s.Id),
                };
                CollectionsControllerViewModel model = new CollectionsControllerViewModel
                {
                    UserName = user.UserName,
                    Role_2 = role,
                    Collections = await collections.AsNoTracking().ToListAsync(),
                };
                return View(model);
            }
            return NotFound();
        }
        public async Task<IActionResult> Create(string username)
        {
            User user=null;
            
            User currentuser = null;
            if (User.Identity.Name!=null)
            {
                currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (username != null)
            {
                user = await _userManager.FindByNameAsync(username);
            }
            if (user != null && currentuser!=null && user.Role=="user")
            {
                if (currentuser.UserName == username || currentuser.Role == "admin" || currentuser.Role == "moderator")
                {
                    CreateViewModel model = new CreateViewModel
                    {
                        UserName = username,
                        CollectionName = null,
                        CreationDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        Likes = 0,
                        ItemCount = 0,
                    };
                    return View(model);
                }
            }


            else if (user != null && currentuser != null)
            {
               if (currentuser.UserName==username || currentuser.Role=="admin")
                {
                    CreateViewModel model = new CreateViewModel
                    {
                        UserName = username,
                        CollectionName = null,
                        CreationDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        Likes = 0,
                        ItemCount = 0,
                    };
                    return View(model);
                }
            }


            return NotFound();
            //return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Collection collection = new Collection
                    {
                        UserName = model.UserName,
                        CollectionName = model.CollectionName,
                        CreationDate = model.CreationDate,
                        Likes = model.Likes,
                        Text=model.Text,
                    };
                    if (model.Image != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(model.Image.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)model.Image.Length);
                        }
                        collection.Image = imageData;
                    }
                    db.Collections.Add(collection);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", "Collections", new { userName = model.UserName });
                }
                else
                {
                    return View(model);
                }
            }

        }
        public async Task<IActionResult> Edit(int Id)
        {
            if (Id != null)
            {
                Collection collection = await db.Collections.FindAsync(Id);
                if (collection != null)
                {
                    User user = await _userManager.FindByNameAsync(collection.UserName);
                    User currentuser = null;
                    if (User.Identity.Name!=null)
                    {
                        currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
                    }
                    if (currentuser != null && user != null && user.Role == "user")
                    {
                        if (currentuser.UserName == user.UserName || currentuser.Role == "admin" || currentuser.Role == "moderator")
                        {
                            EditViewModel model = new EditViewModel
                            {
                                Id = collection.Id,
                                CollectionName = collection.CollectionName,
                                Image = collection.Image,
                                NewImage = null,
                                Text=collection.Text,
                            };
                            return View(model);
                        }
                    }
                    else if (currentuser != null && user != null)
                    {
                        if (currentuser.UserName == user.UserName || currentuser.Role == "admin")
                        {
                            EditViewModel model = new EditViewModel
                            {
                                Id = collection.Id,
                                CollectionName = collection.CollectionName,
                                Image = collection.Image,
                                NewImage = null,
                                Text=collection.Text,
                            };
                            return View(model);
                        }
                    }
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                
                    Collection collection = await db.Collections.FindAsync(model.Id);
                if (model.NewImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(model.NewImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.NewImage.Length);
                    }
                    collection.Image = imageData;
                    db.Collections.Update(collection);
                    await db.SaveChangesAsync();
                }
                if (ModelState.IsValid)
                {
                    collection.CollectionName = model.CollectionName;
                    collection.Text = model.Text;
                    db.Collections.Update(collection);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Collection", "Collections", new { Id = collection.Id });
                }
                else
                {
                    model.Image = collection.Image;
                    return View(model);
                }
            }
        }


        [HttpPost]
        public async Task<ActionResult> Functions(List<int> Id, string Delete, string username)
        {
            if (Id != null)
            {
                Collection collection;
                
                User currentuser = null;
                if (User.Identity.Name != null)
                {
                    currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
                }
                
                User user = await _userManager.FindByNameAsync(username);
                if (!string.IsNullOrEmpty(Delete) && currentuser != null)
                {
                    if (user.Role == "user")
                    {
                        if (currentuser.UserName == username || currentuser.Role == "admin" || currentuser.Role == "moderator")
                        {
                            for (int i = 0; i < Id.Count; i++)
                            {
                                collection = await db.Collections.FindAsync(Id[i]);
                                if (collection != null)
                                {
                                    db.Collections.Remove(collection);
                                    await db.SaveChangesAsync();
                                }

                            }
                        }
                    }

                    else if (currentuser.UserName == username || currentuser.Role == "admin")
                    {
                        for (int i = 0; i < Id.Count; i++)
                        {
                            collection = await db.Collections.FindAsync(Id[i]);
                            if (collection != null)
                            {
                                db.Collections.Remove(collection);
                                await db.SaveChangesAsync();
                            }

                        }
                    }


                }
               
            }
            return RedirectToAction("Index", "Collections", new { userName = username });
        }
        

        public async Task<IActionResult> Collection (int Id, ItemSort sortOrder = ItemSort.IdAsc)
        {
            User user_2;
            string role = null;
            string username = null;
            Collection collection = await db.Collections.FindAsync(Id);
            username = collection.UserName;
            if (User.Identity.Name!=null)
            {
                user_2 = await _userManager.FindByNameAsync(User.Identity.Name);
                role = user_2.Role;
            }
            if (collection!=null)
            {

                IQueryable<Item> items = db.Items.Include(x => x);
                ViewData["IdSort"] = sortOrder == ItemSort.IdAsc ? ItemSort.IdDesc : ItemSort.IdAsc;
                ViewData["NameSort"] = sortOrder == ItemSort.NameAsc ? ItemSort.NameDesc : ItemSort.NameAsc;
                ViewData["TextSort"] = sortOrder == ItemSort.TextAsc ? ItemSort.TextDesc : ItemSort.TextAsc;

                items = sortOrder switch
                {
                    ItemSort.IdDesc => items.OrderByDescending(s => s.Id),
                    ItemSort.NameAsc => items.OrderBy(s => s.ItemName),
                    ItemSort.NameDesc => items.OrderByDescending(s => s.ItemName),
                    ItemSort.TextAsc => items.OrderBy(s => s.Text),
                    ItemSort.TextDesc => items.OrderByDescending(s => s.Text),
                    _ => items.OrderBy(s => s.Id),
                };

                CollectionViewModel model = new CollectionViewModel
                {
                    collection=collection,
                    CollectionId = collection.Id,
                    Items = await items.AsNoTracking().ToListAsync(),
                    Role_2 =role,
                    UserName=username,
                };
                return View(model);
            }
            return NotFound();
        }

        public async Task<IActionResult> AddItem (int Id)
        {
            int maxitems = 1000;
            if (Id!=null)
            {
                User currentuser = null;
                User user = null;
                if (User.Identity.Name!=null)
                {
                    currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
                }
                Collection collection = await db.Collections.FindAsync(Id);
                if (collection != null)
                {
                    user = await _userManager.FindByNameAsync(collection.UserName);
                }
                if (collection != null && currentuser != null && user.Role=="user" && collection.ItemCount<maxitems)
                {
                    if (currentuser.UserName == collection.UserName || currentuser.Role == "admin" || currentuser.Role == "moderator")
                    {
                        AddItemViewModel model = new AddItemViewModel
                        {
                            CollectionId = collection.Id,
                            UserName = collection.UserName,
                            ItemName = null,
                            Likes = 0,
                        };
                        return View(model);
                    }
                }

                else if (collection != null && currentuser != null && collection.ItemCount < maxitems)
                {
                    if (currentuser.UserName == collection.UserName || currentuser.Role == "admin")
                    {
                        AddItemViewModel model = new AddItemViewModel
                        {
                            CollectionId = collection.Id,
                            UserName = collection.UserName,
                            ItemName = null,
                            Likes = 0,
                        };
                        return View(model);
                    }
                }

            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddItem (AddItemViewModel model)
        {
            if (User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Item item = new Item
                    {
                        UserName = model.UserName,
                        CollectionId = model.CollectionId,
                        ItemName = model.ItemName,
                        Likes = model.Likes,
                        Text=model.Text,
                    };
                    if (model.Image != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(model.Image.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)model.Image.Length);
                        }
                        item.Image = imageData;
                    }
                    Collection collection = await db.Collections.FindAsync(item.CollectionId);
                    collection.ItemCount++;
                    db.Items.Add(item);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Collection", "Collections", new { Id = model.CollectionId });
                }
                else
                {
                    return View(model);
                }
            }
        }


        public async Task<IActionResult> EditItem (int Id)
        {
            if (Id != null)
            {
                Item item = await db.Items.FindAsync(Id);
                User currentuser = null;
                User user = null;
                if (User.Identity.Name!=null)
                {
                    currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
                }
                if (item!=null)
                {
                    user = await _userManager.FindByNameAsync(item.UserName);
                }
                if (item != null && currentuser!=null && user.Role=="user")
                {
                    if (currentuser.UserName == item.UserName || currentuser.Role == "admin" || currentuser.Role == "moderator")
                    {
                        EditItemViewModel model = new EditItemViewModel
                        {
                            Id = item.Id,
                            ItemName = item.ItemName,
                            Image = item.Image,
                            NewImage = null,
                            Text=item.Text,
                        };
                        return View(model);
                    }
                }

                else if (item != null && currentuser != null)
                {
                    if (currentuser.UserName == item.UserName || currentuser.Role == "admin")
                    {
                        EditItemViewModel model = new EditItemViewModel
                        {
                            Id = item.Id,
                            ItemName = item.ItemName,
                            Image = item.Image,
                            NewImage = null,
                            Text=item.Text,
                        };
                        return View(model);
                    }
                }
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> EditItem (EditItemViewModel model)
        {
            if (User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Item item = await db.Items.FindAsync(model.Id);

                if (model.NewImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(model.NewImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.NewImage.Length);
                    }
                    item.Image = imageData;
                    db.Items.Update(item);
                    await db.SaveChangesAsync();
                }


                if (ModelState.IsValid)
                {
                    item.ItemName = model.ItemName;
                    item.Text = model.Text;
                    db.Items.Update(item);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Item", "Collections", new { Id = item.Id });
                }
                else
                {
                    model.Image = item.Image;
                    return View(model);
                }
            }
        }



        public async Task<ActionResult> FunctionsItem(List<int> Id, string Delete, int id_0)
        {
            if (Id != null)
            {
                Item item;
                User currentuser = null;
                User user;
                if (User.Identity.Name != null)
                {
                    currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
                }

                Collection collection = await db.Collections.FindAsync(id_0);
                string username = collection.UserName;
                user = await _userManager.FindByNameAsync(username);
                if (!string.IsNullOrEmpty(Delete) && currentuser != null)
                {
                    if (user.Role == "user")
                    {
                        if (currentuser.UserName == username || currentuser.Role == "admin" || currentuser.Role == "moderator")
                        {
                            for (int i = 0; i < Id.Count; i++)
                            {
                                item = await db.Items.FindAsync(Id[i]);
                                if (item != null)
                                {
                                    db.Items.Remove(item);
                                    collection.ItemCount--;
                                    await db.SaveChangesAsync();
                                }

                            }
                        }
                    }

                    else if (currentuser.UserName == username || currentuser.Role == "admin")
                    {
                        for (int i = 0; i < Id.Count; i++)
                        {
                            item = await db.Items.FindAsync(Id[i]);
                            if (item != null)
                            {
                                db.Items.Remove(item);
                                collection.ItemCount--;
                                await db.SaveChangesAsync();
                            }

                        }
                    }


                }
            }
            return RedirectToAction("Collection", "Collections", new { Id = id_0 });
        }
        public async Task<IActionResult> Item (int Id)
        {
            User currentuser = null;
            string role = null;
            if (User.Identity.Name!=null)
            {
                currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
                role = currentuser.Role;
            }
            Item item = await db.Items.FindAsync(Id);
            if (item != null)
            {
                ItemViewModel model = new ItemViewModel
                {
                    Role_2=role,
                    UserName = item.UserName,
                    Item=item,
                    ItemComments = await db.ItemComments.ToListAsync(),
                };
                return View(model);
            }
            return NotFound();
        }

        public async Task<IActionResult> Comment(ItemViewModel model, string CommentButton, int Id)
        {
           
            if (!string.IsNullOrEmpty(CommentButton) && User.Identity.Name != null)
            {
                if (ModelState.IsValid)
                {
                    ItemComment comment = new ItemComment
                    {
                        Text = model.Text,
                        ItemId = Id,
                        UserName = User.Identity.Name,
                    };
                    db.ItemComments.Add(comment);
                    await db.SaveChangesAsync();
                }
                    return RedirectToAction("Item", "Collections", new { Id = Id });
               
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
