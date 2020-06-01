﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Loop.Database;
using Loop.Entities.Concrete;
using Loop.Entities.Intermediate;
using Loop.Services;
using Microsoft.AspNet.Identity;

namespace Loop.Web.Controllers
{
    public class OrderProductsController : Controller
    {
        private readonly UnitOfWork db = new UnitOfWork(new ApplicationDbContext());

        //this is the index page of our cart
        //
        public ActionResult Cart()
        {
            var cart = CreateOrGetCart();
            return View(cart);
        }

        private Cart CreateOrGetCart()
        {
            var cart = Session["Cart"] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                SaveCart(cart);
            }
            return cart;
        }

        //Firstly we will get product from products/index on add click we get product -> redirection to Cart View

        public ActionResult Add(int productId,decimal price,int quantity)
        {
            var product = db.Products.GetAll().ToList().FirstOrDefault(x => x.ProductId == productId);
            var cart = CreateOrGetCart();
            var existingItem = cart.OrderProducts.Where(x => x.ProductId == productId).Any();

            //If added product that exists
            if (existingItem)
            {
                //getting previous product -> creating new OrderPruct and add them to a temp list
                //So i can calculate quantities and prices
                //after using linq i replace current OrderProduct with the new one

                var previousProduct = cart.OrderProducts.Where(x => x.ProductId == productId).First();
                var currentProduct = new OrderProduct()
                {
                    Order = null,
                    Price = price,
                    Quantity = quantity,
                    ProductId = productId,
                    Product = db.Products.GetById(productId)
                };

                var tempList = new List<OrderProduct>();
                tempList.Add(previousProduct);
                tempList.Add(currentProduct);

                var avgPrice = tempList.Average(x=>x.Price);
                var currentQuantity = tempList.Sum(x => x.Quantity);

                var query = cart.OrderProducts.Where(x => currentProduct.ProductId == previousProduct.ProductId)
                                              .Select(g => new OrderProduct()
                                              {
                                                  Order = null,
                                                  Price = avgPrice,
                                                  Quantity = currentQuantity,
                                                  ProductId = productId,
                                                  Product = db.Products.GetById(productId),
                                              }).FirstOrDefault();

                tempList.Remove(previousProduct);
                cart.OrderProducts.Remove(previousProduct);
                cart.OrderProducts.Add(query);

            }
            else
            {

                cart.OrderProducts.Add(new OrderProduct()
                {
                    //We dont need to create an order -> we will create order at checkout proccess
                    Order = null,
                    ProductId = productId,
                    Product = db.Products.GetById(productId),
                    Price = price,
                    Quantity = quantity
                });
            }



            SaveCart(cart);

            return RedirectToAction("Cart", "OrderProducts");
        }


        // orderId : 1293912
        // productId : 123
        // Quantity : 5
        // price : 50

        //If user is authenticated -> Store user to Order and match the relationship then store it db
        // Goal is from Order to get AppUser using User.Identity -> Order-> AppUser and store it 

        // orderId : 1293912
        // productId : 124
        // Quantity : 2
        // price : 50

        //TODO: Add ClearCart btn
        private void ClearCart()
        {
            var cart = new Cart();
            SaveCart(cart);
        }

        private void SaveCart(Cart cart)
        {
            Session["Cart"] = cart;
        }

        public ActionResult Delete(int ProductId)
        {
            //Get Product from db
            var product = db.Products.GetAll().FirstOrDefault(x => x.ProductId == ProductId);

            var cart = CreateOrGetCart();
            //Searching for the product in the list of OrderProducts and receiving the OrderProduct Object
            var existingItem = cart.OrderProducts.Where(x => x.ProductId == ProductId).FirstOrDefault();

            if (existingItem != null)
            {
                //Finally removing it from OrderProducts List
                cart.OrderProducts.Remove(existingItem);
            }

            SaveCart(cart);

            return RedirectToAction("Cart", "OrderProducts");
        }
        public ActionResult Checkout()
        {
            var cart = CreateOrGetCart();

            if (cart.OrderProducts.Any())
            {
                var user = db.Users.GetUserById(User.Identity.GetUserId());
                // Create an Order object to store info about the shopping cart
                var order = new Order()
                {
                    OrderDate = DateTime.UtcNow,
                    ApplicationUser = user,
                    ApplicationUserId = User.Identity.GetUserId(),
                    OrderProducts = cart.OrderProducts.ToList(),
                };

                db.Orders.Insert(order);
                db.Save();

            }

            return RedirectToAction("Cart");
        }
    }
}
