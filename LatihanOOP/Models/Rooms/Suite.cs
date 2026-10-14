namespace LatihanOOP.Models.Rooms;

public class Suite : Room
{
    private const decimal ButlerServiceCharge = 250000m;

    public Suite(int no, int floor, decimal price) : base(no, floor, price)
    {
    }

    public override bool SupportRoomService()
    {
        return true; // Suite mendukung room service
    }

    public override decimal GetAdditionalChargePerNight()
    {
        return ButlerServiceCharge; // Layanan butler Rp 250.000 per malam
    }
}