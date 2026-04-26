namespace E_commarce_Backend.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }        // English
        public string? NameAr { get; set; }      // Arabic

        public decimal Price { get; set; } = 0;

        public string Description { get; set; }
        public string DescriptionAr { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int Stock { get; set; } = 0;
    }

}
