namespace Room.Domain.Enums;

public enum RoomStatus
{
    [Display(Name = "Unknown")]
    Unknown = 0,

    [Display(Name = "Available")]
    Available = 1,

    [Display(Name = "Maintenance")]
    Maintenance = 2,

    [Display(Name = "Out Of Order")]
    OutOfOrder = 3
}