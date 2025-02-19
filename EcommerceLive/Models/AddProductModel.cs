using System.ComponentModel.DataAnnotations;

namespace EcommerceLive.Models
{
    public class AddProductModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Il nome è obbligatorio!")]
        [StringLength(20, ErrorMessage = "Il nome deve essere compreso tra 5 e 20 caratteri", MinimumLength = 5)]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "La descrizione è obbligatoria!")]
        [StringLength(2000, ErrorMessage = "La descrizione deve essere compresa tra 10 e 2000 caratteri", MinimumLength = 10)]
        public string? Description { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "La categoria è obbligatoria!")]
        public string? Category { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Il prezzo è obbligatorio!")]
        [Range(1.00, 10.000, ErrorMessage = "Il prezzo deve essere in un range compreso tra 1 e 10000 euro")]
        public decimal Price { get; set; }

    }
}
