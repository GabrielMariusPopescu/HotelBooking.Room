namespace Room.Application.Commands.Rooms;

public class CreateRoomCommandHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<CreateRoomCommand, Response<Domain.Models.Room>>
{
    public async Task<Response<Domain.Models.Room>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        Domain.Models.Room room = new(
            request.Name,
            DateTime.UtcNow,
            request.Number,
            request.RoomType.GetDisplayName(),
            request.RoomStatus.GetDisplayName(),
            request.PricePerNight,
            false);
        {
            var dbRoom = await repository.Add(room, cancellationToken);
            return dbRoom != null
                ? Response<Domain.Models.Room>.Success(dbRoom)
                : Response<Domain.Models.Room>.Failure($"Room '{request.Name}' could not be created.");
        }
    }
}