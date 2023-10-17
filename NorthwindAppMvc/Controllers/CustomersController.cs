using AspClass.Db.SqlServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NorthwindAppMvc.Controllers
{
    public class CustomersController : Controller
    {
        private readonly NorthwindContext _db;

        public CustomersController(NorthwindContext context)
        {
            _db = context;   
        }

        // GET: CustomersController
        [HttpGet]
        public ActionResult Index()
        {
            var customers = _db.Customers.ToList();
            return View();
        }

        // GET: CustomersController/Details/5
        [HttpGet]
        public ActionResult Details(string id)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.CustomerId == id);
            return View(customer);
        }

        // GET: CustomersController/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
                try
                {
                    _db.Customers.Attach(customer);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(customer);
                }
            return View(customer);
        }

        // GET: CustomersController/Edit/5
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer != null)
            {
                return View(id, customer);
            }
            return NotFound();
        }

        // POST: CustomersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Customer customer)
        {
            if (customer.CustomerId != id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(customer);
                    _db.SaveChanges();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Best practice: Custom conflict management and merging rules
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), id);
            }
            return View(customer);
        }

        private bool CustomerExists(string id)
        {
            return (_db.Customers?.Any(c => c.CustomerId == id)).GetValueOrDefault();
        }

        // GET: CustomersController/Delete/5
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer != null)
            {
                return View();
            }
            return NotFound();
        }

        // POST: CustomersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null) 
            {
                return NotFound();
            }

            try
            {
                _db.Remove(customer);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
