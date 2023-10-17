using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspClass.Db.SqlServer.Models;
using Microsoft.EntityFrameworkCore;

namespace NorthwindAppRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly NorthwindContext _db;

        public IndexModel(ILogger<IndexModel> logger, NorthwindContext context)
        {
            _logger = logger;
            _db = context;
        }

        public List<Product> Products { get; set; }

        public void OnGet()
        {
            Products = _db.Products.ToList();
        }
    }
}