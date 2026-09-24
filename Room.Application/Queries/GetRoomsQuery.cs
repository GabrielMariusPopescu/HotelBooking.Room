namespace Room.Application.Queries;

public class GetRoomsQuery : IRequest<Response<IEnumerable<Domain.Models.Room>>>;