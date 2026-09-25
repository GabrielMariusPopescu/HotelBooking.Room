namespace Room.Application.Queries.Rooms;

public class GetRoomDetailsQueryHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<GetRoomDetailsQuery, Response<Domain.Models.Room>>
{
    public async Task<Response<Domain.Models.Room>> Handle(GetRoomDetailsQuery request, CancellationToken cancellationToken)
    {
        var room = await repository.Get(request.Id, cancellationToken);
        return room != null
            ? Response<Domain.Models.Room>.Success(room)
            : Response<Domain.Models.Room>.Failure($"Room with '{request.Id}' identifier was not found.");
    }
}