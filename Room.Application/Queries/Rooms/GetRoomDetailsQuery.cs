namespace Room.Application.Queries.Rooms;

public class GetRoomDetailsQuery(Guid id) : IRequest<Response<Domain.Models.Room>>
{
    public Guid Id { get; set; } = id;
}