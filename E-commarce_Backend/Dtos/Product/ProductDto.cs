namespace E_commarce_Backend.Dtos.Product
{
    public class ProductDto
    {
        public int? Id { get; set; }   // optional for create
        public string Name { get; set; }
        public string NameAr { get; internal set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; internal set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }

}
