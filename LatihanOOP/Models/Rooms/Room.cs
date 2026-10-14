namespace LatihanOOP.Models.Rooms;

public abstract class Room
{
    public int No { get; }
    public int Floor { get; }
    public decimal Price { get; private set; }
    public bool IsBooked { get; private set; }

    protected Room(int no, int floor, decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Tarif harus lebih dari 0.");

        No = no;
        Floor = floor;
        Price = price;
    }

    // Mengubah status kamar melalui method.
    public void SetIsBooked(bool status)
    {
        IsBooked = status;
    }

    // Tarif akhir pekan naik 20%.
    public virtual decimal GetNightlyPrice(DateTime date)
    {
        bool weekend = date.DayOfWeek is DayOfWeek.Friday
            or DayOfWeek.Saturday;

        return weekend ? Price * 1.20m : Price;
    }

    public bool GetStatus() => IsBooked;

    public abstract bool SupportRoomService();
    public abstract decimal GetAdditionalChargePerWeek();
}