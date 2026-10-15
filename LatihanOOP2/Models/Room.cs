using System;

public abstract class Room
{
    public static int TotalRooms { get; set; } = 0;
    public string RoomNumber { get; private set; }
    public int Floor { get; set; }
    public int NightlyRateValue { get; set; }
    public string Status { get; private set; }

    public Room(string roomNumber, int floor, int nightlyRate, string status)
    {
        RoomNumber = roomNumber;
        Floor = floor;
        NightlyRateValue = nightlyRate;
        Status = status;
        TotalRooms++;
    }

    public abstract int ExtraChargePerNight();

    protected virtual int NightlyRate(int day)
    {
        if (day == 5 || day == 6)
        {
            return NightlyRateValue + (NightlyRateValue * 20 / 100);
        }
        return NightlyRateValue;
    }

    public int CalculateTotal(int nights)
    {
        return (NightlyRate(1) + ExtraChargePerNight()) * nights;
    }

    public int CalculateTotal(int nights, int startDay)
    {
        int total = 0;
        for (int i = 0; i < nights; i++)
        {
            int currentDay = ((startDay + i - 1) % 7) + 1;
            total += NightlyRate(currentDay) + ExtraChargePerNight();
        }
        return total;
    }

    public bool Occupy()
    {
        if (Status == "Available")
        {
            Status = "Occupied";
            return true;
        }
        return false;
    }

    public virtual void Vacate()
    {
        Status = "Available";
    }

    public virtual void Info()
    {
        string statusText = (Status == "Available") ? "Kosong" : "Terisi";
        Console.WriteLine($"Kamar {RoomNumber} | Lantai {Floor} | {GetType().Name} | Tarif {NightlyRateValue:N0}/malam | {statusText}");
    }
}