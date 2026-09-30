namespace Room.Application.Commands.Bookings;

public class DeleteBookingCommand(Guid id): IRequest<RoomResponse<Guid>>
{
    public Guid Id { get; set; } = id;
}