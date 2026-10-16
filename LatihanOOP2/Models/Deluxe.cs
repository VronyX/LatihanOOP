using LatihanOOP2.Interfaces;

public class Deluxe : Room, IRoomService
{
    private int _totalRoomService;

    public Deluxe(string roomNumber, int floor, int nightlyRate, string status)
        : base(roomNumber, floor, nightlyRate, status)
    {
    }

    public override int ExtraChargePerNight() => 100000;

    public bool OrderRoomService(string menu, int price)
    {
        if (string.IsNullOrWhiteSpace(menu))
            throw new ArgumentException("Nama menu tidak boleh kosong.");

        if (price <= 0)
            throw new ArgumentException("Harga harus lebih dari 0.");

        _totalRoomService += price;
        return true;
    }

    public int TotalRoomService() => _totalRoomService;

    public override void Vacate()
    {
        base.Vacate();
        _totalRoomService = 0;
    }

    public override void Info()
    {
        base.Info();
        Console.WriteLine("Sarapan: 100.000/malam");
    }
}