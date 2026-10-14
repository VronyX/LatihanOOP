namespace LatihanOOP.Models.Rooms;

public class Suite : Room
{
    private const decimal ButlerCharge = 250000m;

    public Suite(int no, int floor, decimal price) : base(no, floor, price) { }

    public override bool SupportRoomService() => true;

    public override decimal GetAdditionalChargePerNight() => ButlerCharge;

    public override decimal GetNightlyPrice(DateTime date)
    {
        // Harga dasar + 20% weekend + 250.000 butler service per malam
        return base.GetNightlyPrice(date);
    }
}