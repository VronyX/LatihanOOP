using LatihanOOP2.Exceptions;
using LatihanOOP2.Interfaces;

namespace LatihanOOP2.Models;

public class Reservation
{
    public Guest Guest { get; private set; }
    public Room Room { get; private set; }

    // Lama menginap: harus 1 sampai 14 malam
    public int Nights
    {
        get;
        set
        {
            if (value < 1 || value > 14)
            {
                throw new InvalidStayDurationException(value);
            }
            else
            {
                field = value;
            }
        }
    }

    public int StartDay { get; set; }
    public string Status { get; private set; }

    public Reservation(Guest guest, Room room, int nights, int startDay)
    {
        Guest = guest;
        Room = room;
        Nights = nights;   
        StartDay = startDay;
        Status = "Reserved";
    }

    public bool CheckIn()
    {
        if (Status != "Reserved")
        {
            Console.WriteLine("Tidak bisa check-in: reservasi ini sudah check-in atau sudah selesai.");
            return false;
        }

        if (!Room.Occupy())
        {
            throw new RoomNotAvailableException(Room.RoomNumber, $"Kamar {Room.RoomNumber} sedang terisi.");
        }

        Status = "CheckedIn";
        return true;
    }

    public int CheckOut()
    {
        if (Status != "CheckedIn")
        {
            Console.WriteLine("Tamu belum check-in, tidak bisa check-out.");
            return 0;
        }

        PrintBill();

        int total = Room.CalculateTotal(Nights, StartDay);

        if (Room is IRoomService service)
        {
            total += service.TotalRoomService();
        }

        if (Guest.Member)
        {
            total = total - (total * 10 / 100);
        }

        Room.Vacate();
        Status = "CheckedOut";

        return total;
    }

    public void PrintBill()
    {
        if (Status != "CheckedIn")
        {
            Console.WriteLine("Belum bisa mencetak tagihan (tamu belum check-in).");
            return;
        }

        string[] dayNames = { "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu", "Minggu" };

        int extra = Room.ExtraChargePerNight();
        int extraTotal = extra * Nights;
        int roomOnly = Room.CalculateTotal(Nights, StartDay) - extraTotal;

        Console.WriteLine();
        Console.WriteLine("============== TAGIHAN ==============");
        Console.WriteLine($"Tamu  : {Guest.Name}");
        Console.WriteLine($"Kamar : {Room.RoomNumber} ({Room.GetType().Name})");
        Console.WriteLine("-------------------------------------");

        // Rincian tarif kamar per malam
        for (int i = 0; i < Nights; i++)
        {
            int day = ((StartDay + i - 1) % 7) + 1;
            int rate = Room.CalculateTotal(1, day) - extra;

            string mark = "";
            if (day == 5)
            {
                mark = "  (+20% malam Jumat)";
            }
            else if (day == 6)
            {
                mark = "  (+20% malam Sabtu)";
            }
            Console.WriteLine($"Malam {i + 1} ({dayNames[day - 1]}) : {rate:N0}{mark}");
        }
        Console.WriteLine($"Total kamar   : {roomOnly:N0}");

        if (Room is Deluxe)
        {
            Console.WriteLine($"Sarapan       : {extraTotal:N0}  ({Nights} x {extra:N0})");
        }
        else if (Room is Suite)
        {
            Console.WriteLine($"Butler        : {extraTotal:N0}  ({Nights} x {extra:N0})");
        }

        int roomService = 0;
        if (Room is IRoomService service)
        {
            roomService = service.TotalRoomService();
            Console.WriteLine($"Room service  : {roomService:N0}");
        }

        int subtotal = roomOnly + extraTotal + roomService;
        int discount = 0;
        if (Guest.Member)
        {
            discount = subtotal * 10 / 100;
        }

        Console.WriteLine("-------------------------------------");
        Console.WriteLine($"Subtotal      : {subtotal:N0}");
        Console.WriteLine($"Diskon member : -{discount:N0}");
        Console.WriteLine($"TOTAL         : {subtotal - discount:N0}");
        Console.WriteLine("=====================================");
    }
}
