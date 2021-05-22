using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ShoppingCart.Models;
using System.Net.Http;

namespace ShoppingCart.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        public ItemsController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }
        // GET: Items
        [AllowAnonymous]
        public IActionResult Index(string itemCategory, string searchString)
        {
            IEnumerable<Item> itemList, itemListDup;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Items").Result;
            itemList = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;
            itemListDup = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;

            //accepts distinct category names
            HashSet<string> categoryQuery = new HashSet<string>();

            List<Item> items = new List<Item>();
            List<Item> newitems = new List<Item>();

            HashSet<int> keepIndex = new HashSet<int>();

            foreach (Item item in itemList)
            {
                categoryQuery.Add(item.Category);
                items.Add(item);
            }

            int i = 0;
            int x = 0;
            foreach(var item in items)
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    x = 1;
                    if (item.Title == searchString)
                    {
                        keepIndex.Add(i);
                    }
                }

                if (!string.IsNullOrEmpty(itemCategory))
                {
                    x = 1;
                    if (item.Category == itemCategory)
                    {
                        keepIndex.Add(i);
                    }
                }
                i++;
            }

            foreach(int j in keepIndex)
            {
                newitems.Add(items[j]);
            }

            var itemCategoryVM = new ItemCategoryViewModel();
            if (x == 1)
            {
                itemCategoryVM = new ItemCategoryViewModel
                {
                    Category = new SelectList(categoryQuery.ToList()),
                    Items = newitems
                };
            }
            else
            {
                itemCategoryVM = new ItemCategoryViewModel
                {
                    Category = new SelectList(categoryQuery.ToList()),
                    Items = items
                };
            }
            
            ViewBag.category = new List<String>(categoryQuery.ToList());

            if (LoggedUser.UserRole == "Admin")
            {
                return View("AdminProductList", itemCategoryVM);
            }

            return View(itemCategoryVM);
        }



        // GET: Items/Details/5
        public IActionResult Details(int? id)
        {
            IEnumerable<Item> itemList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Items").Result;
            itemList = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;

            if (id == null)
            {
                return NotFound();
            }

            foreach (Item item in itemList)
            {
                if (item.Id == id)
                {
                    Item kn = new Item
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Category = item.Category,
                        Price = item.Price,
                        ImageName = item.ImageName
                    };
                    return View(kn);
                }
            }

            return NotFound();
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public IActionResult Create([Bind("Id,Title,Category,Price,Image")] ItemViewModel itemView)
        {
            
            Debug.WriteLine(itemView.Image);
            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + itemView.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    itemView.Image.CopyTo(fileStream);
                }

                Item item = new Item
                {
                    Id = itemView.Id,
                    Title = itemView.Title,
                    Category = itemView.Category,
                    Price = itemView.Price,
                    ImageName = uniqueFileName
                };

                HttpResponseMessage response = GlobalVariables.webApiClient.PostAsJsonAsync("Items", item).Result;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Items/Edit/5
        public IActionResult Edit(int? id)
        {
            IEnumerable<Item> itemList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Items").Result;
            itemList = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;

            if (id == null)
            {
                return NotFound();
            }

            foreach (Item item in itemList)
            {
                if (item.Id == id)
                {
                    ItemViewModel itemView = new ItemViewModel
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Category = item.Category,
                        Price = item.Price,
                    };
                    return View(itemView);
                }
            }

            return NotFound();
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,Title,Category,Price,Image")] ItemViewModel itemView)
        {
            if (id != itemView.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + itemView.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    itemView.Image.CopyTo(fileStream);
                }

                Item item = new Item
                {
                    Id = itemView.Id,
                    Title = itemView.Title,
                    Category = itemView.Category,
                    Price = itemView.Price,
                    ImageName = uniqueFileName
                };

               
               HttpResponseMessage response = GlobalVariables.webApiClient.PutAsJsonAsync("Items/"+id.ToString(), item).Result;
              
               return RedirectToAction(nameof(Index));
            }
            return View(itemView);
        }


        // GET: Items/Delete/5
        public IActionResult Delete(int? id)
        {
            IEnumerable<Item> itemList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Items").Result;
            itemList = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;

            if (id == null)
            {
                return NotFound();
            }

            foreach (Item item in itemList)
            {
                if (item.Id == id)
                {
                    Item kn = new Item
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Category = item.Category,
                        Price = item.Price,
                        ImageName = item.ImageName
                    };
                    return PartialView("DeletePartialView", item);
                }
            }

            return NotFound();
           
        }

        [HttpPost, ActionName("Delete")]
        // POST: Items/Delete/5
        public IActionResult DeleteConfirmed(int id)
        {
            
            HttpResponseMessage response = GlobalVariables.webApiClient.DeleteAsync("Items/" + id.ToString()).Result;
            return RedirectToAction(nameof(Index));
        }

    }
}
