﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using myproject.Models;
using myproject.DAL;

namespace myproject.Controllers
{
    public class CartController : Controller
    {
        private Random random = new Random();
        private ProductContext db = new ProductContext();
        public List<Cart> setCart()
        {
            List<Cart> cartlist = Session["Cart"] as List<Cart>;
            if (cartlist == null)
            {
                cartlist = new List<Cart>();
                Session["Cart"] = cartlist;
            }
            return cartlist;
        }
        // GET: Cart
        public ActionResult Index()
        {
            List<Cart> cartlist = setCart();
            return View(cartlist);
        }
        [HttpPost]
        public ActionResult AddtoCart(int id, int amount)
        {
            List<Cart> cartlist = setCart();
            Cart product = cartlist.Find(n => n.productID == id);

            if (product == null)
            {
                product = new Cart(id, amount);
                cartlist.Add(product);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                product.amount += amount;
                return RedirectToAction("Index", "Home");
            }
        }
        
        public ActionResult Info()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SubmitOrder(Order order)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string orderID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
            long totalprice = 0;
            List<Cart> cartlist = setCart();
            Order addorder = new Order();
            addorder.Name = order.Name;
            addorder.Email = order.Email;
            addorder.PhoneNumber = order.PhoneNumber;
            addorder.Address = order.Address;
            addorder.OrderID = orderID;

            foreach(var items in cartlist) 
            {
                totalprice += items.amount * items.price;

            }
            addorder.TotalPrice = totalprice;
            db.Orders.Add(addorder);
            foreach (var item in cartlist)
            {
                OrderDetail orderdetails = new OrderDetail();
                orderdetails.Order = addorder;
                orderdetails.ProductID = item.productID;
                orderdetails.amount = item.amount;
                db.OrderDetails.Add(orderdetails);
            }
            db.SaveChanges();
            cartlist.Clear();
            return RedirectToAction("", "");
        }
    }
}
