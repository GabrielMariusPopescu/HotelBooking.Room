namespace Room.Application.Queries.Rooms;

public class GetRoomsQuery : IRequest<Response<IEnumerable<Domain.Models.Room>>>;