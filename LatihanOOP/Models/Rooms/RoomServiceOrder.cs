namespace LatihanOOP.Models.Rooms;

public class RoomServiceOrder
{
    public string Name { get; }
    public decimal Amount { get; }

    public RoomServiceOrder(string name, decimal amount)
    {
        Name = name;
        Amount = amount;
    }

    public static decimal GetServicePrice(int serviceId)
    {
        return serviceId switch
        {
            1 => 50000m,
            2 => 15000m,
            3 => 35000m,
            _ => 0m
        };
    }

    public static string GetServiceName(int serviceId)
    {
        return serviceId switch
        {
            1 => "Fried Rice",
            2 => "Coca-Cola",
            3 => "Fried Chicken",
            _ => ""
        };
    }
}
