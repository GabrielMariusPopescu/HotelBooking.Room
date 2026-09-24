namespace Room.Application.Commands;

public class DeleteRoomCommandHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<DeleteRoomCommand, Response<Guid>>
{
    public async Task<Response<Guid>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var dbRoom = await repository.Get(request.Id, cancellationToken);
        if (dbRoom == null)
            return Response<Guid>.Failure($"Room with '{request.Id}' identifier could not be found.");

        dbRoom.IsExcluded = true;
        dbRoom.LastUpdated = DateTime.UtcNow;
        
        var deleted = await repository.Disable(dbRoom, cancellationToken);
        return deleted
            ? Response<Guid>.Success(dbRoom.Id)
            : Response<Guid>.Failure($"Room with '{request.Id}' identifier could not be disabled.");
    }
}