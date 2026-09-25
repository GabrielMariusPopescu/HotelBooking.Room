namespace Room.Application.Commands.Rooms;

public class DeleteRoomCommand(Guid id): IRequest<Response<Guid>>
{
    public Guid Id { get; } = id;
}