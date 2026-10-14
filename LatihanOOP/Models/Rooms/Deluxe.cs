namespace LatihanOOP.Models.Rooms;

public class Deluxe : Room
{
    private const decimal BreakfastCharge = 100000m;

    public Deluxe(int no, int floor, decimal price) : base(no, floor, price)
    {
    }

    public override bool SupportRoomService()
    {
        return true; // Deluxe mendukung room service
    }

    public override decimal GetAdditionalChargePerNight()
    {
        return BreakfastCharge; // Wajib sarapan Rp 100.000 per malam
    }
}