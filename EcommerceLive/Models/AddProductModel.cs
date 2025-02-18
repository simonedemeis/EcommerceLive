using System.ComponentModel.DataAnnotations;

namespace EcommerceLive.Models
{
    public class AddProductModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio!")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "La descrizione è obbligatoria!")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "La categoria è obbligatoria!")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Il prezzo è obbligatorio!")]
        public decimal Price { get; set; }
    }
}
