namespace Room.Application.Commands.Rooms;

public class DeleteRoomCommand(Guid id): IRequest<RoomResponse<Guid>>
{
    public Guid Id { get; } = id;
}