using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CustomerCrud.Context;
using CustomerCrud.Models;
using Microsoft.Ajax.Utilities;

namespace CustomerCrud.Controllers
{

    public class CustomerController : Controller
    {
        CustomerDbEntities dbObj = new CustomerDbEntities();

       public ActionResult Index1()
        {
            return View();
        }
        
        public ActionResult Customer(CustomerTable obj)
        {
            if (obj!=null)
            {
                return View(obj);
            }
            else
            {
                return View();
            }           
        }



        [HttpPost]
        public ActionResult AddCustomer(CustomerTable model)
        {

            if (ModelState.IsValid)
            {
                
                if (model.Id==0)
                {
                    model.CustID = GenerateCustomId();
                    model.Isactive = true;
                    dbObj.CustomerTables.Add(model);
                    dbObj.SaveChanges();
                    
                }
                else
                {
                    var existingCustomer = dbObj.CustomerTables.FirstOrDefault(x=>x.Id==model.Id);

                    if (existingCustomer != null)
                    {
                        existingCustomer.Id = model.Id;
                        existingCustomer.CustID = model.CustID;
                        existingCustomer.Fname = model.Fname;
                        existingCustomer.Lname = model.Lname;
                        existingCustomer.Age = model.Age;
                        existingCustomer.Nationality = model.Nationality;
                        existingCustomer.Country = model.Country;
                        existingCustomer.State = model.State;
                        existingCustomer.City = model.City;
                        existingCustomer.Address = model.Address;
                        existingCustomer.Isactive=model.Isactive;

                    }
                    else
                    {
                        ModelState.AddModelError("", "Customer not found.");
                        return View("CustomerList");
                    }
                }
                dbObj.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("CustomerList");
            }
            return View(model);
        }

        public ActionResult CustomerList()
        {          
            var cust = dbObj.CustomerTables.ToList();
            foreach (var customer in cust)
            {
                if (customer.Isactive == true)
                {
                    customer.Isactive = false;
                }
                else
                {
                    customer.Isactive = true;
                }
            }
            return View(cust);
        }

        [HttpGet]
        public ActionResult ActivateCustomer(int id)
        {
            var customer = dbObj.CustomerTables.FirstOrDefault(c => c.Id == id);

            if (customer != null)
            {
                customer.Isactive = true;
                dbObj.SaveChanges();
                return RedirectToAction("CustomerList");
            }

            return RedirectToAction("CustomerList");
        }

        [HttpGet]
        public ActionResult DeactivateCustomer(int id)
        {
            var customer = dbObj.CustomerTables.FirstOrDefault(c => c.Id ==id);
            
            if (customer!=null)
            {
                
                customer.Isactive=false;                
                dbObj.SaveChanges();
                return RedirectToAction("CustomerList");
            }           
            return RedirectToAction("CustomerList");
        }

        /*[HttpPost]
        public ActionResult DeactivateCustomer(CustomerTable model)
        {
            var check = dbObj.CustomerTables.FirstOrDefault(x=>x.Id==model.Id);
            
            if (check!=null)
            {
                check.Isactive = true;
                dbObj.SaveChanges();
                return RedirectToAction("CustomerList");
            }

            return RedirectToAction("CustomerList");

        }*/
        public string GenerateCustomId()
        {
            
            string prefix = "CE";
            string year = DateTime.Now.Year.ToString();

            using (var context = new CustomerDbEntities())
            {
                var latestRecord = context.CustomerTables
                    .Where(c => c.CustID.StartsWith(prefix + "/" + year + "/"))
                    .OrderByDescending(c => c.CustID)
                    .FirstOrDefault();

                int sequenceNumber = 1;

                if (latestRecord != null)
                {
                    string[] parts = latestRecord.CustID.Split('/');
                    if (parts.Length == 3)
                    {
                        if (int.TryParse(parts[2], out int latestSequenceNumber))
                        {
                            sequenceNumber = latestSequenceNumber + 1;
                        }
                    }
                }

               
                string formattedSequence = sequenceNumber.ToString("D3");

                
                string newId = $"{prefix}/{year}/{formattedSequence}";

                return newId;
            }
        }
        
        /*public ActionResult Delete(string id)
        {
            var result = dbObj.CustomerTable.FirstOrDefault(x => x.CustId == id);
            if (result != null)
            {
                dbObj.CustomerTable.Remove(result);
                dbObj.SaveChanges();
            }
            var list=dbObj.CustomerTable.ToList();
            return View("CustomerList", list);
        }

        */
    }
}