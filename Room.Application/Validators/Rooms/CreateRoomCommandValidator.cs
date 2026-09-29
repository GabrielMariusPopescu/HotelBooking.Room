namespace Room.Application.Validators.Rooms;

public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(command => command.Name)
            .MaximumLength(50)
            .WithMessage("Room name can not be empty or more than 50 characters.");

        RuleFor(command => command.Number)
            .GreaterThanOrEqualTo(100)
            .WithMessage("Room number can not be less than 100.");
        
        RuleFor(command => command.RoomType)
            .NotEqual(RoomType.Unknown)
            .WithMessage("Room type can not be unknown.");

        RuleFor(command => command.RoomStatus)
            .NotEqual(RoomStatus.Unknown)
            .WithMessage("Room status can not be unknown.");

        RuleFor(command => command.PricePerNight)
            .GreaterThanOrEqualTo(50.00M)
            .WithMessage("Room price per night can not be less than 50.00.");
    }
}