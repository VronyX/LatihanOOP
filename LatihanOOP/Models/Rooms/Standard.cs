namespace LatihanOOP.Models.Rooms;

public class Standard : Room
{
    public Standard(int no, int floor, decimal price)
        : base(no, floor, price)
    {
    }

    public override bool SupportRoomService() => false;

    public override decimal GetAdditionalChargePerNight() => 0m;
}