namespace E_commarce_Backend.Dtos
{
    public class ProductSearchDto
    {
        public string? Name { get; set; }       
        public string? Category { get; set; }      

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
