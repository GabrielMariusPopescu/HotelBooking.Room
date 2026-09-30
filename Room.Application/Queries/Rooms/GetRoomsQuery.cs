namespace Room.Application.Queries.Rooms;

public class GetRoomsQuery : IRequest<RoomResponse<IEnumerable<Domain.Models.Room>>>;