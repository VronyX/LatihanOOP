namespace LatihanOOP.Models.Rooms;

public class RoomServiceOrder
{
    public string Name { get; set; }
    public decimal Amount { get; set; }

    public RoomServiceOrder(string name, decimal amount)
    {
        Name = name;
        Amount = amount;
    }
}