namespace Application.DTO.Misc
{
    public class GenericFiltersDTO
    {
        public bool? IsActive { get; set; }

        public bool UsePagination { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }
    }
}
