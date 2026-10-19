using System;

public abstract class Room
{
    public static int TotalRooms { get; set; } = 0;
    public string RoomNumber { get; private set; }
    public int Floor { get; set; }
    public int NightlyRate
    {
        get;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Tarif per malam harus lebih dari 0.");
            }
            field = value;
        }
    }
    public string Status { get; private set; }

    public Room(string roomNumber, int floor, int nightlyRate, string status)
    {
        RoomNumber = roomNumber;
        Floor = floor;
        NightlyRate = nightlyRate;
        Status = status;
        TotalRooms++;
    }

    public abstract int ExtraChargePerNight();

    protected virtual int NightlyRateValue(int day)
    {
        if (day == 5 || day == 6)
        {
            return NightlyRate + (NightlyRate * 20 / 100);
        }
        return NightlyRate;
    }

    public int CalculateTotal(int nights)
    {
        return (NightlyRateValue(1) + ExtraChargePerNight()) * nights;
    }

    public int CalculateTotal(int nights, int startDay)
    {
        int total = 0;
        for (int i = 0; i < nights; i++)
        {
            int currentDay = ((startDay + i - 1) % 7) + 1;
            total += NightlyRateValue(currentDay) + ExtraChargePerNight();
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
        string statusText = Status switch
        {
            "Available" => "Kosong",
            "Occupied" => "Terisi",
            "Cleaning" => "Sedang dibersihkan",
            _ => Status
        };
        Console.WriteLine($"Kamar {RoomNumber} | Lantai {Floor} | {GetType().Name} | Tarif {NightlyRate:N0}/malam | {statusText}");
    }

    public bool StartCleaning()
    {
        if (Status != "Occupied")
        {
            return false;
        }

        Status = "Cleaning";
        return true;
    }
}