using LatihanOOP.Models;
using LatihanOOP.Services;
using LatihanOOP.Views;

namespace LatihanOOP;

public class Program
{
    private static readonly Hotel _hotel = new();
    private static readonly HotelView _view = new();

    public static void Main()
    {
        _hotel.InitializeRooms();

        bool running = true;

        while (running)
        {
            _view.Menu();

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Pilihan tidak valid.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    _hotel.DisplayRooms();
                    break;

                case 2:
                    Booking();
                    break;

                case 3:
                    CheckIn();
                    break;

                case 4:
                    RoomService();
                    break;

                case 5:
                    CheckOut();
                    break;

                case 6:
                    _hotel.DisplayReservations();
                    break;

                case 7:
                    Console.WriteLine(
                        $"Persentase hunian: {_hotel.GetRoomPercentage():N2}%");
                    break;

                case 0:
                    running = false;
                    break;

                default:
                    Console.WriteLine("Menu tidak tersedia.");
                    break;
            }
        }
    }

    private static void Booking()
    {
        Console.Write("Nama tamu: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Nomor telepon: ");
        string phone = Console.ReadLine() ?? "";

        Console.Write("Apakah member? (y/n): ");
        bool isMember = string.Equals(
            Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase);

        Console.Write("Tipe kamar (Standard/Deluxe/Suite): ");
        string roomType = Console.ReadLine() ?? "";

        Console.Write("Tanggal check-in (dd-MM-yyyy): ");
        string? inputTanggal = Console.ReadLine();

        if (!DateTime.TryParseExact(
                inputTanggal,
                "dd-MM-yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime dateIn))
        {
            Console.WriteLine(
                "Format tanggal salah! Gunakan format dd-MM-yyyy, contoh: 15-10-2026.");
            return;
        }

        // Tanggal check-in tidak boleh sebelum hari ini.
        if (dateIn.Date < DateTime.Today)
        {
            Console.WriteLine(
                $"Tanggal check-in tidak boleh sebelum hari ini ({DateTime.Today:dd-MM-yyyy}).");
            return;
        }

        Console.Write("Lama menginap (1-14 malam): ");

        if (!int.TryParse(Console.ReadLine(), out int nights)
            || nights < 1 || nights > 14)
        {
            Console.WriteLine("Lama menginap harus antara 1 sampai 14 malam.");
            return;
        }

        try
        {
            Guest guest = new Guest(name, phone, isMember);
            _hotel.CreateReservation(dateIn, nights, guest, roomType);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void CheckIn()
    {
        Console.Write("ID reservasi: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        Reservation? reservation = _hotel.FindReservation(id);

        if (reservation != null && reservation.CheckIn())
            Console.WriteLine("Check-in berhasil.");
        else
            Console.WriteLine("Check-in gagal. Periksa ID, tanggal, dan status.");
    }

    private static void RoomService()
    {
        Console.Write("ID reservasi: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        Reservation? reservation = _hotel.FindReservation(id);

        if (reservation == null)
        {
            Console.WriteLine("Reservasi tidak ditemukan.");
            return;
        }

        _view.DisplayRoomServiceMenu();
        Console.Write("Pilih pesanan (1-3): ");

        if (!int.TryParse(Console.ReadLine(), out int serviceId)
            || !reservation.AddRoomService(serviceId))
        {
            Console.WriteLine("Pesanan ditolak.");
            return;
        }

        Console.WriteLine("Pesanan berhasil dicatat.");
    }

    private static void CheckOut()
    {
        Console.Write("ID reservasi: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        Reservation? reservation = _hotel.FindReservation(id);

        if (reservation == null)
        {
            Console.WriteLine("Reservasi tidak ditemukan.");
            return;
        }

        Invoice? invoice = reservation.CheckOut();

        if (invoice == null)
        {
            Console.WriteLine("Check-out gagal. Tamu belum check-in.");
            return;
        }

        invoice.PrintDetails(reservation);

        Console.Write("Uang pembayaran: Rp");
        if (!decimal.TryParse(Console.ReadLine(), out decimal paidAmount))
        {
            Console.WriteLine("Nominal pembayaran tidak valid.");
            return;
        }

        invoice.Payment(paidAmount);
    }
}