using System;
using System.Threading.Tasks;
using LatihanOOP2.Exceptions;

public abstract class Room
{
    public static int TotalRooms { get; private set; }

    public string RoomNumber { get; }
    public int Floor { get; }
    public int NightlyRateValue { get; private set; }
    public string Status { get; private set; }

    protected Room(
        string roomNumber,
        int floor,
        int nightlyRate,
        string status)
    {
        if (nightlyRate <= 0)
            throw new ArgumentException(
                "Tarif kamar harus lebih dari 0.");

        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException(
                "Nomor kamar tidak boleh kosong.");

        RoomNumber = roomNumber;
        Floor = floor;
        NightlyRateValue = nightlyRate;
        Status = status;
        TotalRooms++;
    }

    public abstract int ExtraChargePerNight();

    // Pembersihan kamar dilakukan secara asynchronous.
    public async Task CleanAsync(int cleaningMilliseconds = 3000)
    {
        if (Status == "Cleaning")
            return;

        Status = "Cleaning";

        Console.WriteLine(
            $"Kamar {RoomNumber} mulai dibersihkan...");

        await Task.Delay(cleaningMilliseconds);

        Status = "Available";

        Console.WriteLine(
            $"Kamar {RoomNumber} selesai dibersihkan.");
    }

    protected virtual int NightlyRate(int day)
    {
        // Jumat dan Sabtu mendapatkan tambahan tarif 20%.
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

        if (startDay < 1 || startDay > 7)
            throw new ArgumentException(
                "Hari mulai harus antara 1 sampai 7.");

        int total = 0;

        for (int i = 0; i < nights; i++)
        {
            int day = ((startDay + i - 1) % 7) + 1;

            // Hanya menghitung tarif kamar per malam.
            total += NightlyRate(day);
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
        string roomStatus = Status switch
        {
            "Available" => "Kosong",
            "Occupied" => "Terisi",
            "Cleaning" => "Sedang dibersihkan",
            _ => Status
        };

        Console.WriteLine(
            $"Kamar {RoomNumber} | Lantai {Floor} | " +
            $"{GetType().Name} | Tarif {NightlyRateValue:N0}/malam | " +
            $"Status: {roomStatus}");
    }
}