﻿using EcommerceLive.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceLive.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> products = new List<Product>()
        {
            new Product()
            {
                Id = Guid.Parse("07a9299d-29db-4a84-b356-b803e9f00415"),
                Name = "Pc",
                Description = "A powerful pc",
                Category = "Electronics",
                Price = 1000
            },
            new Product()
            {
                Id = Guid.Parse("64a2f7a7-fb39-4e15-ba32-c933da611f8b"),
                Name = "Tv",
                Description = "A beautiful tv",
                Category = "Electronics",
                Price = 700
            },
            new Product()
            {
                Id = Guid.Parse("94ca6a86-9e0a-49e7-bad0-ca78ca376f50"),
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

        //Action method per la navigazione verso la vista identificata dal file Add.cshtml
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AddProductModel addProductModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Add");
            }

            var newProduct = new Product()
            {
                Id = Guid.NewGuid(),
                Name = addProductModel.Name,
                Description = addProductModel.Description,
                Category = addProductModel.Category,
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
                return RedirectToAction("Index");
            }

            //Usare come fonte di verità dei dati sempre gli oggetti presenti e recuperati dal database, non i parametri dei metodi.
            var editProduct = new EditProduct()
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                Category = existingProduct.Category,
                Price = existingProduct.Price
            };

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
                return RedirectToAction("Index");
            }

            //assegno i valori conservati nel modello (presi dai campi di input) alle proprietà dell'oggetto trovato. Lo faccio perchè non quale proprietà l'utente ha modificato, quindi le devo assegnare tutte, tranne l'id che rimane lo stesso.
            existingProduct.Name = editProduct.Name;
            existingProduct.Description = editProduct.Description;
            existingProduct.Category = editProduct.Category;
            existingProduct.Price = editProduct.Price;

            return RedirectToAction("Index");
        }
    }
}
