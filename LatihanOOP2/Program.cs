using LatihanOOP2.Exceptions;
using LatihanOOP2.Models;

namespace LatihanOOP2;

internal class Program
{
    private static readonly Hotel hotel = new();
    private static int guestCounter;

    static void Main()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n=========== SISTEM HOTEL ===========");
            Console.WriteLine("1. Daftar kamar");
            Console.WriteLine("2. Reservasi");
            Console.WriteLine("3. Check-in");
            Console.WriteLine("4. Room service");
            Console.WriteLine("5. Check-out");
            Console.WriteLine("6. Laporan hunian");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih menu: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    hotel.PrintRooms();
                    break;
                case "2":
                    ReserveMenu();
                    break;
                case "3":
                    CheckInMenu();
                    break;
                case "4":
                    RoomServiceMenu();
                    break;
                case "5":
                    CheckOutMenu();
                    break;
                case "6":
                    hotel.PrintOccupancyReport();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Menu tidak valid.");
                    break;
            }
        }

        Console.WriteLine("Terima kasih.");
    }

    static void ReserveMenu()
    {
        try
        {
            Console.Write("Nama tamu: ");
            string name = (Console.ReadLine() ?? "").Trim();

            Console.Write("Member? (y/n): ");
            bool member = (Console.ReadLine() ?? "")
                .Trim().Equals("y", StringComparison.OrdinalIgnoreCase);

            Console.Write("Jenis kamar (Standard/Deluxe/Suite): ");
            string type = (Console.ReadLine() ?? "").Trim();

            Console.Write("Lama menginap (1-14 malam): ");
            if (!int.TryParse(Console.ReadLine(), out int nights))
                throw new InvalidStayDurationException("Masukkan jumlah malam berupa angka.");

            Console.Write("Hari mulai (1=Senin, 7=Minggu): ");
            if (!int.TryParse(Console.ReadLine(), out int startDay))
                throw new ArgumentException("Hari mulai harus berupa angka.");

            var guest = new Guest(
                $"G{++guestCounter:000}", name, member);

            var reservation = hotel.CreateReservation(
                guest, type, nights, startDay);

            Console.WriteLine("Reservasi berhasil.");
            guest.Info();
            Console.WriteLine(
                $"Kamar: {reservation.Room.RoomNumber} | " +
                $"Durasi: {reservation.Nights} malam");
        }
        catch (Exception ex) when (
            ex is ArgumentException ||
            ex is RoomNotAvailableException)
        {
            Console.WriteLine($"Reservasi gagal: {ex.Message}");
        }
    }

    static void CheckInMenu()
    {
        try
        {
            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            hotel.CheckIn(roomNumber);
            Console.WriteLine("Check-in berhasil.");
        }
        catch (RoomNotAvailableException ex)
        {
            Console.WriteLine($"Check-in ditolak: {ex.Message}");
        }
    }

    static void RoomServiceMenu()
    {
        try
        {
            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            Console.Write("Nama menu: ");
            string menu = (Console.ReadLine() ?? "").Trim();

            Console.Write("Harga: ");
            if (!int.TryParse(Console.ReadLine(), out int price))
                throw new ArgumentException("Harga harus berupa angka.");

            int total = hotel.OrderRoomService(roomNumber, menu, price);
            Console.WriteLine($"Pesanan dicatat. Total room service: {total:N0}");
        }
        catch (Exception ex) when (
            ex is ArgumentException ||
            ex is RoomNotAvailableException ||
            ex is RoomServiceNotSupportedException)
        {
            Console.WriteLine($"Room service ditolak: {ex.Message}");
        }
    }

    static void CheckOutMenu()
    {
        try
        {
            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            int total = hotel.CheckOut(roomNumber);
            Console.WriteLine($"Check-out berhasil. Total dibayar: {total:N0}");
        }
        catch (RoomNotAvailableException ex)
        {
            Console.WriteLine($"Check-out gagal: {ex.Message}");
        }
    }
}