using System;
using System.Threading.Tasks;
using LatihanOOP2.Exceptions;
using LatihanOOP2.Interfaces;

namespace LatihanOOP2.Models;

public class Reservation
{
    public int TotalBill { get; set; }

    public Guest Guest { get; }
    public Room Room { get; }
    public int Nights { get; }
    public int StartDay { get; }

    public string Status { get; private set; } = "Reserved";

    public Reservation(
        Guest guest,
        global::Room room,
        int nights,
        int startDay)
    {
        if (nights < 1 || nights > 14)
            throw new InvalidStayDurationException(
                "Lama menginap harus antara 1 sampai 14 malam.");

        if (startDay < 1 || startDay > 7)
            throw new ArgumentException(
                "Hari mulai harus antara 1 sampai 7.");

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

    // Check-out dan pembersihan kamar secara asynchronous.
    public async Task<int> CheckOutAsync()
    {
        if (Status != "CheckedIn")
            throw new InvalidOperationException(
                "Tamu harus check-in terlebih dahulu sebelum check-out.");

        // Tampilkan rincian tagihan.
        PrintBill();

        // Hitung dan simpan total tagihan.
        int total = CalculateBill();
        TotalBill = total;

        // Reservasi selesai, tetapi kamar masih dibersihkan.
        Status = "CheckedOut";

        // Tunggu sampai proses pembersihan selesai.
        await Room.CleanAsync();

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
        Console.WriteLine(
            $"Kamar : {Room.RoomNumber} ({Room.GetType().Name})");

        for (int i = 0; i < Nights; i++)
        {
            int day = ((StartDay + i - 1) % 7) + 1;

            int nightly = Room.CalculateTotal(1, day);
            roomTotal += nightly;

            string note = day == 5 || day == 6
                ? " (+20%)"
                : "";

            Console.WriteLine(
                $"Malam {i + 1} ({days[day - 1]}) : " +
                $"{nightly:N0}{note}");
        }

        int extraCharge = Room.ExtraChargePerNight() * Nights;

        Console.WriteLine($"Total kamar   : {roomTotal:N0}");
        Console.WriteLine($"Biaya tambahan: {extraCharge:N0}");

        int serviceTotal = 0;

        if (Room is IRoomService service)
        {
            serviceTotal = service.TotalRoomService();
            Console.WriteLine($"Room service  : {serviceTotal:N0}");
        }

        int subtotal = roomTotal + extraCharge + serviceTotal;
        int discount = Guest.Member ? subtotal * 10 / 100 : 0;
        int total = subtotal - discount;

        Console.WriteLine($"Subtotal      : {subtotal:N0}");
        Console.WriteLine($"Diskon member : -{discount:N0}");
        Console.WriteLine($"TOTAL         : {total:N0}");
        Console.WriteLine("=====================================");
    }

    public int CalculateBill()
    {
        // Gunakan tagihan yang sudah tersimpan setelah check-out.
        if (Status == "CheckedOut")
            return TotalBill;

        int roomTotal = Room.CalculateTotal(Nights, StartDay);

        int extraCharge =
            Room.ExtraChargePerNight() * Nights;

        int serviceTotal = 0;

        if (Room is IRoomService service)
            serviceTotal = service.TotalRoomService();

        int subtotal = roomTotal + extraCharge + serviceTotal;

        int discount = Guest.Member
            ? subtotal * 10 / 100
            : 0;

        return subtotal - discount;
    }
}