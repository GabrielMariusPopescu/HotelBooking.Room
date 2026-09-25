namespace Room.Application.Commands.Bookings;

public class CreateBookingCommand(
    DateOnly checkIn,
    DateOnly checkOut,
    int guests,
    decimal totalPrice,
    BookingStatus bookingStatus) : IRequest<Response<Booking>>
{
    public DateOnly CheckIn { get; set; } = checkIn;

    public DateOnly CheckOut { get; set; } = checkOut;

    public int Guests { get; set; } = guests;

    public decimal TotalPrice { get; set; } = totalPrice;

    public BookingStatus BookingStatus { get; set; } = bookingStatus;
}