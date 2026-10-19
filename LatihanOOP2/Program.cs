using System;
using System.Linq;
using System.Threading.Tasks;
using LatihanOOP2.Exceptions;
using LatihanOOP2.Interfaces;
using LatihanOOP2.Models;

namespace LatihanOOP2;

internal class Program
{
    private static readonly Hotel hotel = new();
    private static int guestCounter;

    static async Task Main()
    {
        SeedDummyData();

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
            Console.WriteLine("7. Persentase hunian per jenis kamar (LINQ)");
            Console.WriteLine("8. Total tagihan per tamu (LINQ)");
            Console.WriteLine("9. Member dengan tagihan terbesar (LINQ)");
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
                    await CheckOutMenu();
                    break;

                case "6":
                    hotel.PrintOccupancyReport();
                    break;

                case "7":
                    OccupancyByRoomTypeReport();
                    break;

                case "8":
                    TotalBillPerGuestReport();
                    break;

                case "9":
                    HighestMemberBillReport();
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

    // RESERVASI

    static void ReserveMenu()
    {
        try
        {
            Console.Write("Nama tamu: ");
            string name = (Console.ReadLine() ?? "").Trim();

            Console.Write("Member? (y/n): ");
            bool member = (Console.ReadLine() ?? "")
                .Trim()
                .Equals("y", StringComparison.OrdinalIgnoreCase);

            Console.Write("Jenis kamar (Standard/Deluxe/Suite): ");
            string type = (Console.ReadLine() ?? "").Trim();

            Console.Write("Lama menginap (1-14 malam): ");

            if (!int.TryParse(Console.ReadLine(), out int nights))
            {
                throw new InvalidStayDurationException(
                    "Masukkan jumlah malam berupa angka.");
            }

            Console.Write("Hari mulai (1=Senin, 7=Minggu): ");

            if (!int.TryParse(Console.ReadLine(), out int startDay))
            {
                throw new ArgumentException(
                    "Hari mulai harus berupa angka.");
            }

            var guest = new Guest(
                $"G{++guestCounter:000}",
                name,
                member);

            var reservation = hotel.CreateReservation(
                guest,
                type,
                nights,
                startDay);

            Console.WriteLine("\nReservasi berhasil.");

            guest.Info();

            Console.WriteLine(
                $"Kamar: {reservation.Room.RoomNumber} | " +
                $"Jenis: {reservation.Room.GetType().Name} | " +
                $"Durasi: {reservation.Nights} malam");
        }
        catch (Exception ex) when (
            ex is ArgumentException ||
            ex is RoomNotAvailableException)
        {
            Console.WriteLine($"Reservasi gagal: {ex.Message}");
        }
    }

    // CHECK-IN

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
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Check-in ditolak: {ex.Message}");
        }
    }

    // ROOM SERVICE

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
            {
                throw new ArgumentException(
                    "Harga harus berupa angka.");
            }

            int total = hotel.OrderRoomService(
                roomNumber,
                menu,
                price);

            Console.WriteLine(
                $"Pesanan dicatat. Total room service: Rp{total:N0}");
        }
        catch (Exception ex) when (
            ex is ArgumentException ||
            ex is RoomNotAvailableException ||
            ex is RoomServiceNotSupportedException ||
            ex is InvalidOperationException)
        {
            Console.WriteLine($"Room service ditolak: {ex.Message}");
        }
    }

    // CHECK-OUT ASYNC

    static async Task CheckOutMenu()
    {
        try
        {
            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            // Cari reservasi yang masih berstatus CheckedIn.
            var reservation = hotel.Reservations.FirstOrDefault(
                res =>
                    res.Room.RoomNumber.Equals(
                        roomNumber,
                        StringComparison.OrdinalIgnoreCase)
                    && res.Status == "CheckedIn");

            if (reservation == null)
            {
                throw new InvalidOperationException(
                    "Reservasi tidak ditemukan atau tamu belum check-in.");
            }

            Console.WriteLine("\nMemproses check-out...");

            int total = await reservation.CheckOutAsync();

            Console.WriteLine(
                $"\nCheck-out berhasil. Total dibayar: Rp{total:N0}");

            Console.WriteLine(
                $"Status reservasi: {reservation.Status}");

            Console.WriteLine(
                $"Status kamar: {reservation.Room.Status}");
        }
        catch (RoomNotAvailableException ex)
        {
            Console.WriteLine($"Check-out gagal: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Check-out gagal: {ex.Message}");
        }
    }

    // LAPORAN HUNIAN LINQ

    static void OccupancyByRoomTypeReport()
    {
        var report = hotel.Rooms
            .GroupBy(room => room.GetType().Name)
            .Select(group => new
            {
                RoomType = group.Key,
                TotalRooms = group.Count(),

                OccupiedRooms = group.Count(
                    room => room.Status == "Occupied"),

                OccupancyPercentage = group.Count() == 0
                    ? 0
                    : group.Count(
                        room => room.Status == "Occupied")
                        * 100.0 / group.Count()
            })
            .OrderBy(item => item.RoomType);

        Console.WriteLine(
            "\n====== HUNIAN PER JENIS KAMAR ======");

        foreach (var item in report)
        {
            Console.WriteLine(
                $"{item.RoomType,-10} | " +
                $"Terisi: {item.OccupiedRooms}/{item.TotalRooms} | " +
                $"Hunian: {item.OccupancyPercentage:F2}%");
        }

        Console.WriteLine("====================================");
    }

    // TOTAL TAGIHAN PER TAMU LINQ

    static void TotalBillPerGuestReport()
    {
        var report = hotel.Reservations
            .GroupBy(res => new
            {
                res.Guest.GuestId,
                res.Guest.Name,
                res.Guest.Member
            })
            .Select(group => new
            {
                GuestId = group.Key.GuestId,
                GuestName = group.Key.Name,
                IsMember = group.Key.Member,

                TotalBill = group.Sum(
                    res => res.CalculateBill())
            })
            .OrderByDescending(item => item.TotalBill)
            .ToList();

        Console.WriteLine(
            "\n========== TOTAL TAGIHAN TAMU ==========");

        if (report.Count == 0)
        {
            Console.WriteLine("Belum ada data reservasi.");
            return;
        }

        foreach (var item in report)
        {
            Console.WriteLine(
                $"{item.GuestId} | {item.GuestName} | " +
                $"Member: {(item.IsMember ? "Ya" : "Tidak")} | " +
                $"Total: Rp{item.TotalBill:N0}");
        }

        Console.WriteLine("========================================");
    }

    // MEMBER DENGAN TAGIHAN TERBESAR LINQ

    static void HighestMemberBillReport()
    {
        var highestMember = hotel.Reservations
            .Where(res => res.Guest.Member)
            .GroupBy(res => new
            {
                res.Guest.GuestId,
                res.Guest.Name
            })
            .Select(group => new
            {
                GuestId = group.Key.GuestId,
                GuestName = group.Key.Name,

                TotalBill = group.Sum(
                    res => res.CalculateBill())
            })
            .OrderByDescending(item => item.TotalBill)
            .FirstOrDefault();

        Console.WriteLine(
            "\n===== MEMBER DENGAN TAGIHAN TERBESAR =====");

        if (highestMember == null)
        {
            Console.WriteLine(
                "Belum ada data reservasi tamu member.");
            return;
        }

        Console.WriteLine($"ID Tamu : {highestMember.GuestId}");
        Console.WriteLine($"Nama    : {highestMember.GuestName}");
        Console.WriteLine($"Tagihan : Rp{highestMember.TotalBill:N0}");
        Console.WriteLine("==========================================");
    }

    // DATA DUMMY

    static void SeedDummyData()
    {
        Console.WriteLine("Memuat data dummy...");

        AddDummyReservation("Andi", false, "Standard", 2, 1);
        AddDummyReservation("Budi", true, "Standard", 3, 5);
        AddDummyReservation("Citra", true, "Standard", 1, 2);
        AddDummyReservation("Joko", false, "Standard", 4, 3);
        AddDummyReservation("Lina", true, "Standard", 2, 6);

        AddDummyReservation("Dewi", true, "Deluxe", 3, 5, 200000);
        AddDummyReservation("Eka", false, "Deluxe", 2, 1, 100000);
        AddDummyReservation("Fajar", false, "Deluxe", 4, 4, 300000);
        AddDummyReservation("Maya", true, "Deluxe", 5, 2, 250000);
        AddDummyReservation("Rizky", true, "Deluxe", 3, 6, 400000);

        AddDummyReservation("Gita", true, "Suite", 5, 5, 1500000);
        AddDummyReservation("Hadi", false, "Suite", 2, 1, 300000);
        AddDummyReservation("Indah", true, "Suite", 3, 3, 500000);
        AddDummyReservation("Nadia", true, "Suite", 4, 6, 700000);
        AddDummyReservation("Putra", false, "Suite", 2, 4, 600000);

        Console.WriteLine("Data dummy selesai dimuat.");
    }

    static void AddDummyReservation(
        string name,
        bool member,
        string type,
        int nights,
        int startDay,
        int roomServiceTotal = 0)
    {
        Guest guest = new Guest(
            $"G{++guestCounter:000}",
            name,
            member);

        try
        {
            Reservation reservation = hotel.CreateReservation(
                guest,
                type,
                nights,
                startDay);

            // Check-in membuat status kamar menjadi Occupied.
            reservation.CheckIn();

            // Tambahkan room service jika kamar mendukungnya.
            if (roomServiceTotal > 0 &&
                reservation.Room is IRoomService service)
            {
                service.OrderRoomService(
                    "Paket Dummy",
                    roomServiceTotal);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Gagal membuat dummy {name}: {ex.Message}");
        }
    }
}