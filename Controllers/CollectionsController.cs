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
                collections = sortOrder switch
                {
                    CollectionSort.IdDesc => collections.OrderByDescending(s => s.Id),
                    CollectionSort.NameAsc=>collections.OrderBy(s=>s.CollectionName),
                    CollectionSort.NameDesc=>collections.OrderByDescending(s=>s.CollectionName),
                    _=>collections.OrderBy(s=>s.Id),
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
                if (model.CollectionName != null)
                {
                    Collection collection = new Collection
                    {
                        UserName = model.UserName,
                        CollectionName = model.CollectionName,
                        CreationDate = model.CreationDate,
                        Likes = model.Likes,
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
                if (model.CollectionName != null)
                {
                    collection.CollectionName = model.CollectionName;
                    db.Collections.Update(collection);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Collections", new { userName = collection.UserName });
                }
                else
                {
                    model.Image = collection.Image;
                    return View(model);
                }
            }
        }


        [HttpPost]
        public async Task<ActionResult> Functions(List<int> Id, string Delete)
        {
            Collection collection;
            Collection collection_2=await db.Collections.FindAsync(Id[0]);
            User currentuser = null;
            if (User.Identity.Name!=null)
            {
                currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            string username = collection_2.UserName;
            User user = await _userManager.FindByNameAsync(username);
            if (!string.IsNullOrEmpty(Delete) && currentuser!=null)
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
            return RedirectToAction("Index", "Collections", new{userName=username});
        }
        

        public async Task<IActionResult> Collection (int Id)
        {
            Collection collection = await db.Collections.FindAsync(Id);
            if (collection!=null)
            {
                CollectionViewModel model = new CollectionViewModel
                {
                    CollectionId = collection.Id,
                    Items = await db.Items.ToListAsync(),
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
                if (model.ItemName != null)
                {
                    Item item = new Item
                    {
                        UserName = model.UserName,
                        CollectionId = model.CollectionId,
                        ItemName = model.ItemName,
                        Likes = model.Likes,
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


                if (model.ItemName != null)
                {
                    item.ItemName = model.ItemName;
                    db.Items.Update(item);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Collection", "Collections", new { Id = item.CollectionId });
                }
                else
                {
                    model.Image = item.Image;
                    return View(model);
                }
            }
        }



        public async Task<ActionResult> FunctionsItem(List<int> Id, string Delete)
        {
            Item item;
            User currentuser = null;
            User user;
            if (User.Identity.Name != null)
            {
                currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            Item item_2 = await db.Items.FindAsync(Id[0]);
            int Id_2 = item_2.CollectionId;
            Collection collection = await db.Collections.FindAsync(Id_2);
            string username = item_2.UserName;
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
            return RedirectToAction("Collection", "Collections", new { Id = Id_2 });
        }
        public async Task<IActionResult> Item (int Id)
        {

            Item item = await db.Items.FindAsync(Id);
            if (item != null)
            {
                ItemViewModel model = new ItemViewModel
                {
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
                ItemComment comment = new ItemComment
                {
                 Text = model.Comment.Text,
                 ItemId = Id,
                 UserName = User.Identity.Name,
                };
                 db.ItemComments.Add(comment);
                await db.SaveChangesAsync();
                return RedirectToAction("Item", "Collections", new { Id = Id});
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
