namespace Application.DTO.Uom
{
    public class AddUomDTO
    {
        public required string Name { get; set; }
        public required string ShortName { get; set; }
        public required bool IsInteger { get; set; }
    }
}
