namespace Room.Application.Commands;

public class UpdateRoomCommand(
    Guid id,
    string name,
    int number,
    RoomType roomType,
    RoomStatus roomStatus,
    decimal pricePerNight) : IRequest<Response<Domain.Models.Room>>
{
    public Guid Id { get; } = id;

    public string Name { get; } = name;

    public int Number { get; } = number;

    public RoomType RoomType { get; } = roomType;

    public RoomStatus RoomStatus { get; } = roomStatus;

    public decimal PricePerNight { get; } = pricePerNight;
}