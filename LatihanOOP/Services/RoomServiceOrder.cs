namespace LatihanOOP.Services;

public class RoomServiceOrder
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }

    public RoomServiceOrder(string description, decimal amount)
    {
        Description = description;
        Amount = amount;
        OrderDate = DateTime.Now;
    }

    public static int GetAdditionalServicePrice(int serviceId)
    {
        return serviceId switch
        {
            1 => 50000,
            2 => 15000,
            3 => 35000,
            _ => 0
        };
    }
}