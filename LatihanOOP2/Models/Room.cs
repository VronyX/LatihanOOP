using LatihanOOP2.Exceptions;

public abstract class Room
{
    public static int TotalRooms { get; private set; }

    public string RoomNumber { get; }
    public int Floor { get; }
    public int NightlyRateValue { get; private set; }
    public string Status { get; private set; }

    protected Room(string roomNumber, int floor, int nightlyRate, string status)
    {
        if (nightlyRate <= 0)
            throw new ArgumentException("Tarif kamar harus lebih dari 0.");

        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException("Nomor kamar tidak boleh kosong.");

        RoomNumber = roomNumber;
        Floor = floor;
        NightlyRateValue = nightlyRate;
        Status = status;
        TotalRooms++;
    }

    public abstract int ExtraChargePerNight();

    protected virtual int NightlyRate(int day)
    {
        return day == 5 || day == 6
            ? NightlyRateValue + (NightlyRateValue * 20 / 100)
            : NightlyRateValue;
    }

    public int CalculateTotal(int nights)
    {
        return CalculateTotal(nights, 1);
    }

    public int CalculateTotal(int nights, int startDay)
    {
        if (nights < 1 || nights > 14)
            throw new InvalidStayDurationException(
                "Lama menginap harus antara 1 sampai 14 malam.");

        int total = 0;

        for (int i = 0; i < nights; i++)
        {
            int day = ((startDay + i - 1) % 7) + 1;
            total += NightlyRate(day) + ExtraChargePerNight();
        }

        return total;
    }

    public bool Occupy()
    {
        if (Status != "Available")
            return false;

        Status = "Occupied";
        return true;
    }

    public virtual void Vacate()
    {
        Status = "Available";
    }

    public virtual void Info()
    {
        Console.WriteLine(
            $"Kamar {RoomNumber} | Lantai {Floor} | " +
            $"{GetType().Name} | Tarif {NightlyRateValue:N0}/malam | {Status}");
    }
}