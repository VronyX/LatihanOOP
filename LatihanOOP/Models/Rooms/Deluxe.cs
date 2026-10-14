namespace LatihanOOP.Models.Rooms;

public class Deluxe : Room
{
    private const decimal BreakfastCharge = 100000m;

    public Deluxe(int no, int floor, decimal price) : base(no, floor, price) { }

    public override bool SupportRoomService() => true;

    public override decimal GetAdditionalChargePerNight() => BreakfastCharge;

    public override decimal GetNightlyPrice(DateTime date)
    {
        // Harga dasar + 20% weekend + 100.000 sarapan per malam
        return base.GetNightlyPrice(date);
    }
}