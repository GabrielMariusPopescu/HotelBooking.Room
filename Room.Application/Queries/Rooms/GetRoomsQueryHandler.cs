namespace Room.Application.Queries.Rooms;

public class GetRoomsQueryHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<GetRoomsQuery, RoomResponse<IEnumerable<Domain.Models.Room>>>
{
    public async Task<RoomResponse<IEnumerable<Domain.Models.Room>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = (await repository.Get(cancellationToken)).ToList();
        return rooms.Any()
            ? RoomResponse<IEnumerable<Domain.Models.Room>>.Success(rooms)
            : RoomResponse<IEnumerable<Domain.Models.Room>>.Failure("No rooms were found.");
    }
}