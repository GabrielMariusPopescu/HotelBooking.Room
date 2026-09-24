namespace Room.Domain.Models;

public class Room : NamedEntity
{
    public int Number { get; set; }

    public string RoomType { get; set; }

    public string RoomStatus { get; set; }

    public decimal PricePerNight { get; set; }
    
    public bool IsExcluded { get; set; }

    public Room()
    {
        
    }
    
    public Room(string name, DateTime created, int number, string roomType, string roomStatus, decimal pricePerNight, bool isExcluded)
    {
        Name = name;
        Created = created;
        Number = number;
        RoomType = roomType;
        RoomStatus = roomStatus;
        PricePerNight = pricePerNight;
        IsExcluded = isExcluded;
    }
}