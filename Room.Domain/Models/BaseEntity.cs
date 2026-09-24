namespace Room.Domain.Models;

public class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime? LastUpdated { get; set; }
}