namespace Room.Application.Requests.Rooms;

public class CreateRoomRequest
{
    public required string Name { get; set; }

    public required int Number { get; set; }

    public required string RoomType { get; set; }

    public required string RoomStatus { get; set; }

    public required decimal PricePerNight { get; set; }
}