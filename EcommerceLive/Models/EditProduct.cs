using System.ComponentModel.DataAnnotations;

namespace EcommerceLive.Models
{
    public class EditProduct
    {
        public Guid? Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Il nome è obbligatorio!")]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "La descrizione è obbligatoria!")]
        public string? Description { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "La categoria è obbligatoria!")]
        public Guid? CategoryId { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Il prezzo è obbligatorio!")]
        public decimal Price { get; set; }
    }
}
