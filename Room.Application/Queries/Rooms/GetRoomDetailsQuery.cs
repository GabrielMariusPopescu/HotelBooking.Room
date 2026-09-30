namespace Room.Application.Queries.Rooms;

public class GetRoomDetailsQuery(Guid id) : IRequest<RoomResponse<Domain.Models.Room>>
{
    public Guid Id { get; set; } = id;
}