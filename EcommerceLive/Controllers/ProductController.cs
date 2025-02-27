using EcommerceLive.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace EcommerceLive.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ProductController : Controller
    {
        //campo privato readonly che può essere valorizzato solo all'interno del costruttore.
        private readonly string _connectionString;

        public ProductController()
        {
            //creo un'istanza della configurazione, per leggere la stringa di connessione al database
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true) //il secondo parametro indica l'obligatoreità del file, il terzo parametro indica se il file deve essere ricaricato e letto nuovamente quando viene modificato durante l'esecuzione del programa
                .Build();

            //Lettura della configurazione dal file appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> Index()
        {
            var productsList = new ProductsViewModel()
            {
                Products = new List<Product>()
            };

            //mi connetto al server di database
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //se non apro la connessione, non mi connetto al db e non posso eseguire query
                await connection.OpenAsync();
                string query = "SELECT Prodotti.prodotto_id, Prodotti.nome, Categorie.categoria_nome, Prodotti.prezzo, Prodotti.descrizione FROM Prodotti INNER JOIN Categorie ON Prodotti.categoria_id = Categorie.categoria_id;";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productsList.Products.Add(
                                new Product()
                                {
                                    Id = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    Category = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Description = reader.GetString(4)
                                }
                            );
                        }
                    }
                }
            }

            return View(productsList);
        }

        //Action method per la navigazione verso la vista identificata dal file Add.cshtml
        public async Task<IActionResult> Add()
        {
            var model = new AddProductModel()
            {
                Categories = await GetCategories()
            };

            return View(model);
        }

        //metodo per il recuper delle categorie dal database
        private async Task<List<Category>> GetCategories()
        {
            List<Category> listaCategorie = new List<Category>();
            //mi connetto al serve del database
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //apro la connessione
                await connection.OpenAsync();
                var query = "SELECT * FROM Categorie";

                //istanzio ed esego il comando sql
                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        //leggiamo tutte le righe dalla tabella Categorie finche non sono esaurite

                        while(await reader.ReadAsync())
                        {
                            listaCategorie.Add(
                                new Category()
                                {
                                    Id = reader.GetGuid(0),
                                    Name = reader.GetString(1)
                                }    
                            );
                        }
                    }
                }
            }

            return listaCategorie;

        }


        [HttpPost]
        public async Task<IActionResult> Create(AddProductModel addProductModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Try again!";
                return RedirectToAction("Add");
            }

            //mi connetto al server del database
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //apro la connessione al db
                await connection.OpenAsync();
                //Il simbolo @ identifica un placeholder che verrà sostituito dal valore reale
                var query = "INSERT INTO Prodotti VALUES (@prodotto_id, @nome, @descrizione, @prezzo, @categoria_id)";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@prodotto_id", Guid.NewGuid());
                    command.Parameters.AddWithValue("@nome", addProductModel.Name);
                    command.Parameters.AddWithValue("@descrizione", addProductModel.Description);
                    command.Parameters.AddWithValue("@prezzo", addProductModel.Price);
                    command.Parameters.AddWithValue("@categoria_id", addProductModel.CategoryId);

                    int righeInteressate = await command.ExecuteNonQueryAsync();
                }

            }

            return RedirectToAction("Index");
        }

        [HttpGet("product/edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            //Usare come fonte di verità dei dati sempre gli oggetti presenti e recuperati dal database, non i parametri dei metodi.
            var editProduct = new EditProduct();

            //mi collego al serve db
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //apro la connessione
                await connection.OpenAsync();
                var query = "SELECT * FROM Prodotti WHERE prodotto_id = @Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            editProduct.Id = Guid.Parse(reader["prodotto_id"].ToString());
                            editProduct.Name = reader.GetString(1);
                            editProduct.CategoryId = reader.GetGuid(4);
                            editProduct.Description = reader.GetString(2);
                            editProduct.Price = reader.GetDecimal(3);
                        }
                    }
                }
            }

            //Passo la lista di categorie alla vista tramite ViewBag. Ricordarsi di specificare il tipo di dato.

            //uso il metodo GetCategories per non ripetere il codice della selezione delle categorie
            ViewBag.Categories = await GetCategories();

            return View(editProduct);
        }

        [HttpPost("product/edit/save/{id:guid}")]
        public async Task<IActionResult> SaveEdit(Guid id, EditProduct editProduct)
        {
            //mi collego al server del db
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //apro la connessione al db
                await connection.OpenAsync();
                var query = $"UPDATE Prodotti SET nome=@name, descrizione=@descrizione, prezzo=@prezzo, categoria_id=@categoria_id WHERE prodotto_id=@Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@nome", editProduct.Name);
                    command.Parameters.AddWithValue("@descrizione", editProduct.Description);
                    command.Parameters.AddWithValue("@prezzo", editProduct.Price);
                    command.Parameters.AddWithValue("@categoria_id", editProduct.CategoryId);

                    int righeInteressate = await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet("product/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            //mi connetto al server del db
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //apro la connessione
                await connection.OpenAsync();
                var query = "DELETE FROM Prodotti WHERE prodotto_id = @Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int righeInteressate = await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
