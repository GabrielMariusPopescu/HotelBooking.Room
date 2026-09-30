namespace Room.Application.Commands.Rooms;

public class CreateRoomCommandHandler(IRepository<Domain.Models.Room> repository): IRequestHandler<CreateRoomCommand, RoomResponse<Domain.Models.Room>>
{
    public async Task<RoomResponse<Domain.Models.Room>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateRoomCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return RoomResponse<Domain.Models.Room>.Failure($"Errors: {string.Join(",", validationResult.Errors)}");
        
        Domain.Models.Room room = new(
            request.Name,
            DateTime.UtcNow,
            request.Number,
            request.RoomType.GetDisplayName(),
            request.RoomStatus.GetDisplayName(),
            request.PricePerNight,
            false);
        
            var dbRoom = await repository.Add(room, cancellationToken);
            return dbRoom != null
                ? RoomResponse<Domain.Models.Room>.Success(dbRoom)
                : RoomResponse<Domain.Models.Room>.Failure($"Room '{request.Name}' could not be created.");
}
}