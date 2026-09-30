namespace Room.Application.Queries.Rooms;

public class GetRoomDetailsQueryHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<GetRoomDetailsQuery, RoomResponse<Domain.Models.Room>>
{
    public async Task<RoomResponse<Domain.Models.Room>> Handle(GetRoomDetailsQuery request, CancellationToken cancellationToken)
    {
        var room = await repository.Get(request.Id, includeRelations: false, cancellationToken);
        return room != null
            ? RoomResponse<Domain.Models.Room>.Success(room)
            : RoomResponse<Domain.Models.Room>.Failure($"Room with '{request.Id}' identifier was not found.");
    }
}