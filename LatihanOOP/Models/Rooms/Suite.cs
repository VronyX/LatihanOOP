namespace LatihanOOP.Models.Rooms;

public class Suite : Room, IRoomServiceSupport
{
    private readonly decimal serviceCharge = 250000m;

    public Suite(int no, int floor, decimal price)
        : base(no, floor, price)
    {
    }

    public override decimal GetAdditionalChargePerNight()
    {
        return serviceCharge;
    }

    public override bool SupportRoomService() => true;

    public decimal GetServicePrice(int serviceId)
    {
        return serviceId switch
        {
            1 => 50000m,
            2 => 75000m,
            3 => 100000m,
            _ => 0m
        };
    }

    public string GetServiceName(int serviceId)
    {
        return serviceId switch
        {
            1 => "Nasi Goreng",
            2 => "Mie Goreng",
            3 => "Steak",
            _ => string.Empty
        };
    }
}