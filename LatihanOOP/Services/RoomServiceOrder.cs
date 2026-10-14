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
}