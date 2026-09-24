namespace Room.Application.Commands;

public class DeleteRoomCommand(Guid id): IRequest<Response<Guid>>
{
    public Guid Id { get; } = id;
}