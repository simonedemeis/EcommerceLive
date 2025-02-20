using EcommerceLive.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceLive.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ProductController : Controller
    {
        private static List<Category> categories = new List<Category>()
        {
            new Category()
            {
                Id = Guid.Parse("a39a5aeb-ddbb-4827-b84d-f5f4ee1ee815"),
                Name = "Electronics"
            },
            new Category()
            {
                Id = Guid.Parse("4361ffed-6842-401f-accb-75065981d5d9"),
                Name = "Gardening"
            },
            new Category()
            {
                Id = Guid.Parse("e394ac27-43db-4f7c-b6fa-78e70a67a354"),
                Name = "Books"
            }
        };

        private static List<Product> products = new List<Product>()
        {
            new Product()
            {
                Id = Guid.Parse("07a9299d-29db-4a84-b356-b803e9f00415"),
                Name = "Pc",
                Description = "A powerful pc",
                Category = categories[0],
                Price = 1000
            },
            new Product()
            {
                Id = Guid.Parse("64a2f7a7-fb39-4e15-ba32-c933da611f8b"),
                Name = "Tv",
                Description = "A beautiful tv",
                Category = categories[0],
                Price = 700
            },
            new Product()
            {
                Id = Guid.Parse("94ca6a86-9e0a-49e7-bad0-ca78ca376f50"),
                Name = "Book",
                Description = "A nice book",
                Category = categories[2],
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

        //Action method per la navigazione verso la vista identificata dal file Add.cshtml
        public IActionResult Add()
        {
            var model = new AddProductModel()
            {
                Categories = categories
            };
            return View(model);
        }


        [HttpPost]
        public IActionResult Create(AddProductModel addProductModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Try again!";
                return RedirectToAction("Add");
            }

            var newProduct = new Product()
            {
                Id = Guid.NewGuid(),
                Name = addProductModel.Name,
                Description = addProductModel.Description,
                Category = categories.FirstOrDefault(c => c.Id == addProductModel.CategoryId),
                Price = addProductModel.Price,
            };

            products.Add(newProduct);

            return RedirectToAction("Index");
        }

        [HttpGet("product/edit/{id:guid}")]
        public IActionResult Edit(Guid id)
        {
            //cerco l'oggetto corrispondente all'id nella lista statica/database

            var existingProduct = products.FirstOrDefault(p => p.Id == id);

            if (existingProduct == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction("Index");
            }

            //Usare come fonte di verità dei dati sempre gli oggetti presenti e recuperati dal database, non i parametri dei metodi.
            var editProduct = new EditProduct()
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                CategoryId = existingProduct.Category?.Id,
                Price = existingProduct.Price
            };

            //Passo la lista di categorie alla vista tramite ViewBag. Ricordarsi di specificare il tipo di dato.
            ViewBag.Categories = categories;

            return View(editProduct);
        }

        [HttpPost("product/edit/save/{id:guid}")]
        public IActionResult SaveEdit(Guid id, EditProduct editProduct)
        {
            //cerco l'oggetto corrispondente all'id nella lista statica/database
            var existingProduct = products.FirstOrDefault(p => p.Id == id);

            //controllo se ho trovato il prodotto all'interno della lista/database
            if (existingProduct == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction("Index");
            }

            //assegno i valori conservati nel modello (presi dai campi di input) alle proprietà dell'oggetto trovato. Lo faccio perchè non quale proprietà l'utente ha modificato, quindi le devo assegnare tutte, tranne l'id che rimane lo stesso.
            existingProduct.Name = editProduct.Name;
            existingProduct.Description = editProduct.Description;
            existingProduct.Category = categories.FirstOrDefault(c => c.Id == editProduct.CategoryId);
            existingProduct.Price = editProduct.Price;

            return RedirectToAction("Index");
        }

        [HttpGet("product/delete/{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var existingProduct = products.FirstOrDefault(p => p.Id == id);

            //controllo se ho trovato il prodotto all'interno della lista/database
            if (existingProduct == null)
            {
                return RedirectToAction("Index");
            }

            //Il metodo Remove delle liste restituisce un booleano che sta a indicare il successo dell'operazione.
            var isRemoveSuccessful = products.Remove(existingProduct);

            if (!isRemoveSuccessful)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
