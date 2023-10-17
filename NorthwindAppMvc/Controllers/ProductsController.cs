using AspClass.Db.SqlServer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NorthwindAppMvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly NorthwindContext _db;

        // Inject the DBContext (NorthwindContext like defined in Program.cs) on Constructor creation
        public ProductsController(NorthwindContext context)
        {
            // consume the injected object to use it in this class
            _db = context;            
        }

        [HttpGet()]
        public IActionResult Index()
        {
            ViewBag.Time = DateTime.Now.ToLongTimeString();
            //var products = _db.Products.ToList();
            //return View(nameof(Index), products);
            return View(nameof(Index));
        }

        [HttpGet()]
        public IActionResult Details(int productId)
        {
            var product = _db.Products.Where(p => p.ProductId == productId).FirstOrDefault();
            if (product != null)
            {
                return View(product);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet()]
        public IActionResult Create() 
        {
            var suppliers = (from s in _db.Suppliers
                            select new SelectListItem()
                            {
                                Value = s.SupplierId.ToString(),
                                Text = s.CompanyName
                            }).ToList();
            ViewBag.SupplierId = suppliers;

            var categories = (from c in _db.Categories
                             select new SelectListItem()
                             {
                                 Value = c.CategoryId.ToString(),
                                 Text = c.CategoryName
                             }).ToList();
            ViewBag.CategoryId = categories;

            return View();
        }

        [HttpPost()]
        public IActionResult Create(Product product) 
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            _db.Products.Attach(product);
            _db.SaveChanges();
            
            return RedirectToAction(nameof(Details),new { productId = product.ProductId });
        }

        [HttpPost()]
        public IActionResult Delete(int productId) 
        { 
            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
