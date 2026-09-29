namespace Room.Application.Validators;

public class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Room ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Room name is required.")
            .MaximumLength(100)
            .WithMessage("Room name cannot exceed 100 characters.");

        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Room number is required.")
            .GreaterThanOrEqualTo(100)
            .WithMessage("Room number must be a positive integer.");

        RuleFor(x => x.RoomType)
            .IsInEnum()
            .WithMessage("Invalid room type.");

        RuleFor(x => x.RoomStatus)
            .IsInEnum()
            .WithMessage("Invalid room status.");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0)
            .WithMessage("Price per night must be a positive value.");
    }
}