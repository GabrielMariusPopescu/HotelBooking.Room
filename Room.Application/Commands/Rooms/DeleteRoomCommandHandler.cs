namespace Room.Application.Commands.Rooms;

public class DeleteRoomCommandHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<DeleteRoomCommand, RoomResponse<Guid>>
{
    public async Task<RoomResponse<Guid>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var dbRoom = await repository.Get(request.Id, includeRelations: false, cancellationToken);
        if (dbRoom == null)
            return RoomResponse<Guid>.Failure($"Room with '{request.Id}' identifier could not be found.");

        dbRoom.IsExcluded = true;
        dbRoom.LastUpdated = DateTime.UtcNow;
        
        var disabled = await repository.Disable(dbRoom, cancellationToken);
        return disabled
            ? RoomResponse<Guid>.Success(dbRoom.Id)
            : RoomResponse<Guid>.Failure($"Room with '{request.Id}' identifier could not be disabled.");
    }
}