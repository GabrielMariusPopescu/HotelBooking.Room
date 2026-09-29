namespace Room.Application.Commands.Rooms;

public class UpdateRoomCommand(
    Guid id,
    string name,
    int number,
    RoomType roomType,
    RoomStatus roomStatus,
    decimal pricePerNight,
    bool isExcluded,
    DateTime? lastUpdated) : IRequest<Response<Domain.Models.Room>>
{
    public Guid Id { get; } = id;

    public string Name { get; } = name;

    public int Number { get; } = number;

    public RoomType RoomType { get; } = roomType;

    public RoomStatus RoomStatus { get; } = roomStatus;

    public decimal PricePerNight { get; } = pricePerNight;

    public bool IsExcluded { get; } = isExcluded;

    public DateTime? LastUpdated { get; } = lastUpdated;
}