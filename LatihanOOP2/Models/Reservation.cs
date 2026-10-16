using LatihanOOP2.Exceptions;
using LatihanOOP2.Interfaces;

namespace LatihanOOP2.Models;

public class Reservation
{
    public Guest Guest { get; }
    public global::Room Room { get; }
    public int Nights { get; }
    public int StartDay { get; }
    public string Status { get; private set; } = "Reserved";

    public Reservation(Guest guest, global::Room room, int nights, int startDay)
    {
        if (nights < 1 || nights > 14)
            throw new InvalidStayDurationException(
                "Lama menginap harus antara 1 sampai 14 malam.");

        if (startDay < 1 || startDay > 7)
            throw new ArgumentException("Hari mulai harus antara 1 sampai 7.");

        Guest = guest;
        Room = room;
        Nights = nights;
        StartDay = startDay;
    }

    public void CheckIn()
    {
        if (Status != "Reserved" || !Room.Occupy())
            throw new RoomNotAvailableException(
                $"Kamar {Room.RoomNumber} sedang terisi atau tidak tersedia.");

        Status = "CheckedIn";
    }

    public int OrderRoomService(string menu, int price)
    {
        if (Status != "CheckedIn")
            throw new InvalidOperationException(
                "Room service hanya dapat dipesan setelah check-in.");

        if (Room is not IRoomService service)
            throw new RoomServiceNotSupportedException(
                "Kamar Standard tidak menyediakan room service.");

        service.OrderRoomService(menu, price);
        return service.TotalRoomService();
    }

    public int CheckOut()
    {
        if (Status != "CheckedIn")
            throw new InvalidOperationException(
                "Tamu belum check-in atau sudah check-out.");

        PrintBill();

        int total = Room.CalculateTotal(Nights, StartDay);

        if (Room is IRoomService service)
            total += service.TotalRoomService();

        if (Guest.Member)
            total -= total * 10 / 100;

        Room.Vacate();
        Status = "CheckedOut";

        return total;
    }

    public void PrintBill()
    {
        string[] days =
        {
            "Senin", "Selasa", "Rabu", "Kamis",
            "Jumat", "Sabtu", "Minggu"
        };

        int roomTotal = 0;

        Console.WriteLine("\n============== TAGIHAN ==============");
        Console.WriteLine($"Tamu  : {Guest.Name}");
        Console.WriteLine($"Kamar : {Room.RoomNumber} ({Room.GetType().Name})");

        for (int i = 0; i < Nights; i++)
        {
            int day = ((StartDay + i - 1) % 7) + 1;
            int nightly = Room.CalculateTotal(1, day);
            roomTotal += nightly;

            string note = day == 5 || day == 6
                ? " (+20%)"
                : "";

            Console.WriteLine(
                $"Malam {i + 1} ({days[day - 1]}) : {nightly:N0}{note}");
        }

        Console.WriteLine($"Total kamar   : {roomTotal:N0}");
        Console.WriteLine($"Biaya tambahan: {Room.ExtraChargePerNight() * Nights:N0}");

        int serviceTotal = 0;

        if (Room is IRoomService service)
        {
            serviceTotal = service.TotalRoomService();
            Console.WriteLine($"Room service  : {serviceTotal:N0}");
        }

        int subtotal = roomTotal + serviceTotal;
        int discount = Guest.Member ? subtotal * 10 / 100 : 0;

        Console.WriteLine($"Subtotal      : {subtotal:N0}");
        Console.WriteLine($"Diskon member : -{discount:N0}");
        Console.WriteLine($"TOTAL         : {subtotal - discount:N0}");
        Console.WriteLine("=====================================");
    }
}