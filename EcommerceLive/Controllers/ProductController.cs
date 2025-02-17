using EcommerceLive.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceLive.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> products = new List<Product>()
        {
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Pc",
                Description = "A powerful pc",
                Category = "Electronics",
                Price = 1000
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Tv",
                Description = "A beautiful tv",
                Category = "Electronics",
                Price = 700
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Book",
                Description = "A nice book",
                Category = "Literature",
                Price = 20
            }
        };

        public IActionResult Index()
        {
            var productsList = new ProductsViewModel()
            {
                Products = products
            };

            return View(productsList);
        }
    }
}
