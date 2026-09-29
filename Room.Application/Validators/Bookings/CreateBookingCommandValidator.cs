namespace Room.Application.Validators.Bookings; 

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(command => command.CheckIn)
            .NotEmpty()
            .WithMessage("Check-in date is required.");

        RuleFor(command => command.CheckOut)
            .NotEmpty()
            .WithMessage("Check-out date is required.")
            .GreaterThan(command => command.CheckIn)
            .WithMessage("Check-out date must be strictly after the check-in date.");

        RuleFor(command => command.Guests)
            .GreaterThan(0)
            .WithMessage("Number of guests must be at least 1.");

        RuleFor(command => command.RoomIds)
            .NotEmpty()
            .WithMessage("At least one room must be selected.");
    }
}