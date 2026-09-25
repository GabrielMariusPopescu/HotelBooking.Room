namespace Room.Application.Commands.Rooms;

public class UpdateRoomCommandHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<UpdateRoomCommand, Response<Domain.Models.Room>>
{
    public async Task<Response<Domain.Models.Room>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var dbRoom = await repository.Get(request.Id, cancellationToken);
        if (dbRoom == null)
            return Response<Domain.Models.Room>.Failure($"Room with '{request.Id}' identifier could not be found.");
        
        dbRoom.Name = request.Name;
        dbRoom.Number = request.Number;
        dbRoom.RoomType = request.RoomType.GetDisplayName();
        dbRoom.RoomStatus = request.RoomStatus.GetDisplayName();
        dbRoom.PricePerNight = request.PricePerNight;
dbRoom.LastUpdated = DateTime.UtcNow;

var updated = await repository.Update(dbRoom, cancellationToken);
return updated
    ? Response<Domain.Models.Room>.Success(dbRoom)
    : Response<Domain.Models.Room>.Failure($"Room with '{request.Id}' identifier could not be updated.");
    }
}