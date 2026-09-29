namespace Room.Application.Validators.Bookings;

public class UpdateBookingCommandValidator : AbstractValidator<UpdateBookingCommand>
{
    public UpdateBookingCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Booking ID is required.");

        RuleFor(x => x.CheckIn)
            .NotEmpty()
            .WithMessage("Check-in date is required.");

        RuleFor(x => x.CheckOut)
            .NotEmpty()
            .WithMessage("Check-out date is required.")
            .GreaterThan(x => x.CheckIn)
            .WithMessage("Check-out date must be strictly after the check-in date.");

        RuleFor(x => x.Guests)
            .GreaterThan(0)
            .WithMessage("Number of guests must be at least 1.");

        RuleFor(x => x.BookingStatus)
            .IsInEnum()
            .WithMessage("Invalid booking status.");

        RuleFor(x => x.TotalPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total price cannot be negative.");
    }
}
