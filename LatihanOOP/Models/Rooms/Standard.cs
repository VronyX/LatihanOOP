namespace LatihanOOP.Models.Rooms;

public class Standard : Room
{
    public Standard(int no, int floor, decimal price) : base(no, floor, price) { }

    public override bool SupportRoomService() => false;

    public override decimal GetAdditionalChargePerNight() => 0m;

    public override decimal GetNightlyPrice(DateTime date)
    {
        // Memanggil base untuk menghitung harga dasar + 20% weekend
        return base.GetNightlyPrice(date) + GetAdditionalChargePerNight();
    }
}