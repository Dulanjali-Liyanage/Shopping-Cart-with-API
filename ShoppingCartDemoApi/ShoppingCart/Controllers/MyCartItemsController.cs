using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ShoppingCartDemo.Helpers;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using ShoppingCart.Models;
using System.Net.Http;

namespace ShoppingCart.Controllers
{
    
    public class MyCartItemsController : Controller
    {

        // GET: MyCartItems
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items");
            

            if (cart == null) {
                ViewBag.cart = cart;
                ViewBag.total = 0;
            }
            else
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => item.Price);
            }

            
            return View();
        }


        //displaying the details of the item
        // GET: Items/AddtoCart/5
        public IActionResult AddtoCart(int? id)
        {
            if (GlobalVariables.webApiClient.DefaultRequestHeaders.Authorization == null)
            {
                return RedirectToAction("Login", "Authenticate");
            }
            IEnumerable<Item> itemList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Items").Result;
            itemList = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;

            if (id == null)
            {
                return NotFound();
            }

            foreach (Item item in itemList)
            {
                if(item.Id == id)
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

        public IActionResult ToCart([Bind("Id,Title,Category,Price,ImageName")] Item myCartItem)
        {
            Debug.WriteLine(myCartItem);

            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(myCartItem);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Items", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items");
                /*int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Product = productModel.find(id), Quantity = 1 });
                }*/
                cart.Add(myCartItem);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Items", cart);
            }
            return RedirectToAction("Index", "Items");
        }





        //GET: MyCartItems/Delete/5

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
                    return PartialView("MyCartItemDeletePartialView", kn);
                }
            }
            return NotFound();
        }

        // POST: MyCartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items");
            Debug.WriteLine(id);
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Items", cart);
            return RedirectToAction("index");
        }


        private int isExist(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }


    }
}
