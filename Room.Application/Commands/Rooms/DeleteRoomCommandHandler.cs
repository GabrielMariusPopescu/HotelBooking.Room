namespace Room.Application.Commands.Rooms;

public class DeleteRoomCommandHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<DeleteRoomCommand, Response<Guid>>
{
    public async Task<Response<Guid>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var dbRoom = await repository.Get(request.Id, cancellationToken);
        if (dbRoom == null)
            return Response<Guid>.Failure($"Room with '{request.Id}' identifier could not be found.");

        dbRoom.IsExcluded = true;
        dbRoom.LastUpdated = DateTime.UtcNow;
        
        var disabled = await repository.Disable(dbRoom, cancellationToken);
        return disabled
            ? Response<Guid>.Success(dbRoom.Id)
            : Response<Guid>.Failure($"Room with '{request.Id}' identifier could not be disabled.");
    }
}