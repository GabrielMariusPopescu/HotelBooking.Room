namespace Room.Application.Queries;

public class GetRoomsQueryHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<GetRoomsQuery, Response<IEnumerable<Domain.Models.Room>>>
{
    public async Task<Response<IEnumerable<Domain.Models.Room>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = (await repository.Get(cancellationToken)).ToList();
        return rooms.Any()
            ? Response<IEnumerable<Domain.Models.Room>>.Success(rooms)
            : Response<IEnumerable<Domain.Models.Room>>.Failure("No rooms was found.");
    }
}