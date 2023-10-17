using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspClass.Db.SqlServer.Models;

namespace NorthwindAppMvc.ViewComponents
{
    public class ProductsGalleryViewComponent : ViewComponent
    {
        private readonly NorthwindContext _db;

        public ProductsGalleryViewComponent(NorthwindContext context)
        {
            _db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int number = 0)
        {
            var products = await GetProductsAsync(number);
            return View("Default", products);
        }

        private Task<List<Product>> GetProductsAsync(int number = 0)
        {
            Task<List<Product>> products;

            if (number == 0)
            {
                products = _db.Products.ToListAsync();
            }
            else
            {
                products = (from p in _db.Products 
                            orderby p.UnitPrice descending 
                            select p).Take(number).ToListAsync();
            }
            return products;
        }
    }
}
