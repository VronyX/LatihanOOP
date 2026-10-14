namespace LatihanOOP.Models.Rooms;

public class Suite : Room
{
    private const decimal ServiceCharge = 250000m;

    public Suite(int no, int floor, decimal price)
        : base(no, floor, price)
    {
    }

    public override bool SupportRoomService() => true;

    public override decimal GetAdditionalChargePerNight()
        => ServiceCharge;
}