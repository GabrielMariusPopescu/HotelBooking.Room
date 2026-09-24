namespace Room.Domain.Models;

public class NamedEntity : BaseEntity
{
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}