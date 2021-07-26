﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using myproject.Models;
using PagedList;
namespace myproject.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders

       
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var orders = from s in db.Orders
                         select s;
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(s =>
                s.Name.ToUpper().Contains(searchString.ToUpper())
                ||
                s.PhoneNumber.ToString().Contains(searchString)
                ||
                s.Email.ToUpper().Contains(searchString.ToUpper())
                ||
                s.Date.ToString().Contains(searchString)
                ||
                s.Province.ToUpper().Contains(searchString.ToUpper())
                ||
                s.District.ToUpper().Contains(searchString.ToUpper())
                ||
                s.Ward.ToUpper().Contains(searchString.ToUpper()));
            }
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            return View(orders.OrderBy(m => m.Checked).ThenBy(n => n.Date).ToPagedList(pageNumber, pageSize));
        }
        // GET: Orders/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            List<OrderDetail> orderdetail = db.OrderDetails.Where(m => m.Order.OrderID == order.OrderID).ToList();
            List<Product> products = new List<Product>();
            var productfetch = from s in db.Products
                         select s;
            foreach ( var item in orderdetail)
            {
                foreach (var product in productfetch)
                    if (item.ProductID == product.productID)
                        products.Add(product);
            }
            Session["OrderList"] = orderdetail;
            Session["Products"] = products;
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,UserID,Name,Email,PhoneNumber,Province,District,Ward,Address,TotalPrice,Date,Checked")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,UserID,Name,Email,PhoneNumber,Province,District,Ward,Address,TotalPrice,Date,Checked")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Order order = db.Orders.Find(id);
            foreach (var item in db.OrderDetails.Where(s => s.Order.OrderID == id))
            {
                db.OrderDetails.Remove(item);
            }
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CheckOrderd(string id)
        {
            Order order = db.Orders.Find(id);
            order.Checked = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
