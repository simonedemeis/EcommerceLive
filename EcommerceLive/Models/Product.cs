namespace EcommerceLive.Models
{
    public class Product
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Category? Category { get; set; }
        public decimal Price { get; set; }
    }
}
