namespace Application.DTO.Uom;

public class AddUomDto
{
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public required bool IsInteger { get; set; }
}