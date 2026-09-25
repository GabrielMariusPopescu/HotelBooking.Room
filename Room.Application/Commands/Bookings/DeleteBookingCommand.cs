namespace Room.Application.Commands.Bookings;

public class DeleteBookingCommand(Guid id): IRequest<Response<Guid>>
{
    public Guid Id { get; set; } = id;
}