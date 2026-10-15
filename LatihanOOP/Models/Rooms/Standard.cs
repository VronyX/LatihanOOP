namespace LatihanOOP.Models.Rooms;

public class Standard : Room
{
    public Standard(int no, int floor, decimal price)
        : base(no, floor, price)
    {
    }

    public override decimal GetAdditionalChargePerNight()
    {
        return 0m;
    }
}