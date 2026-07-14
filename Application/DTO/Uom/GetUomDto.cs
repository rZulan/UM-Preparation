namespace Application.DTO.Uom;

public class GetUomDto
{
    public required int Id { get; set; }
    public required bool IsActive { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public required bool IsInteger { get; set; }
}