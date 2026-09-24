namespace Room.Application.Commands;

public class CreateRoomCommand(
    string name,
    int number,
    RoomType roomType,
    RoomStatus roomStatus,
    decimal pricePerNight) : IRequest<Response<Domain.Models.Room>>
{
    public string Name { get; } = name;

    public int Number { get; } = number;

    public RoomType RoomType { get; } = roomType;

    public RoomStatus RoomStatus { get; } = roomStatus;

    public decimal PricePerNight { get; } = pricePerNight;
}