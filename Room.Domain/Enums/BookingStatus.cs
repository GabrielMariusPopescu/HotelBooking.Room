namespace Room.Domain.Enums;

public enum BookingStatus
{
    [Display(Name = "Unknown")]
    Unknown = 0,

    [Display(Name = "Pending")]
    Pending = 1,

    [Display(Name = "Confirmed")]
    Confirmed = 2,

    [Display(Name = "Cancelled")]
    Cancelled = 3,

    [Display(Name = "Completed")]
    Completed = 4
}