namespace LatihanOOP.Models.Rooms;

public abstract class Room
{
    public int No { get; protected set; }
    public int Floor { get; protected set; }
    public bool IsBooked { get; private set; }
    public decimal Price { get; protected set; }

    protected Room(int no, int floor, decimal price)
    {
        No = no;
        Floor = floor;
        Price = price;
        IsBooked = false;
    }

    public virtual decimal GetNightlyPrice(DateTime date)
    {
        bool isWeekend = date.DayOfWeek is DayOfWeek.Friday
            or DayOfWeek.Saturday;

        return isWeekend ? Price * 1.2m : Price;
    }

    public bool GetStatus() => IsBooked;

    public virtual bool SupportRoomService() => false;

    public abstract decimal GetAdditionalChargePerNight();

    public void SetIsBooked(bool isCheckIn)
    {
        IsBooked = isCheckIn;
    }
}