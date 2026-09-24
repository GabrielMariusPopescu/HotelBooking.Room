namespace Room.Domain.Enums;

public enum RoomType
{
    [Display(Name = "Unknown")]
    Unknown = 0,

    [Display(Name = "Standard")]
    Standard = 1,

    [Display(Name = "Suite")]
    Suite = 2,

    [Display(Name = "Apartment")]
    Apartment = 3,

    [Display(Name = "Hostel")]
    Hostel = 4
}