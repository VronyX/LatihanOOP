namespace LatihanOOP.Models.Rooms;

public class Deluxe : Room
{
    private const decimal BreakfastCharge = 100000m;

    public Deluxe(int no, int floor, decimal price)
        : base(no, floor, price)
    {
    }

    public override bool SupportRoomService() => true;

    public override decimal GetAdditionalChargePerNight()
        => BreakfastCharge;
}