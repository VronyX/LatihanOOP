using LatihanOOP2.Exceptions;
using LatihanOOP2.Interfaces;
using LatihanOOP2.Models;
using System;

namespace LatihanOOP2
{
    internal class Program
    {
        static Hotel hotel = new Hotel();
        static int guestCounter = 0;
        const int timeoutSeconds = 5;

        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("=========== SISTEM HOTEL ===========");
                Console.WriteLine("1. Daftar kamar");
                Console.WriteLine("2. Reservasi");
                Console.WriteLine("3. Check-in");
                Console.WriteLine("4. Room service");
                Console.WriteLine("5. Check-out");
                Console.WriteLine("6. Laporan hunian");
                Console.WriteLine("7. Tambah kamar baru");
                Console.WriteLine("8. Daftar tunggu");
                Console.WriteLine("9. List Kamar Kosong");
                Console.WriteLine("10. Total Room Service");
                Console.WriteLine("11. Reservasi Online (simulasi timeout)");
                Console.WriteLine("0. Keluar");
                Console.Write("Pilih menu: ");

                switch ((Console.ReadLine() ?? "0").Trim())
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
                        CheckOutMenuAsync().Wait();
                        break;
                    case "6":
                        OccupancyReport();
                        break;
                    case "7":
                        AddRoomMenu();
                        break;
                    case "8":
                        hotel.PrintWaitingList();
                        break;
                    case "9":
                        hotel.PrintEmptyRoomList();
                        break;
                    case "10":
                        TotalRoomService();
                        break;
                    case "11":
                        OnlineReservationMenuAsync().Wait();
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
            Console.WriteLine("\n------ RESERVASI ------");

            Console.Write("Nama tamu: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (name == "")
            {
                Console.WriteLine("Reservasi ditolak: nama tamu tidak boleh kosong.");
                return;
            }

            Console.Write("Member? (y/n): ");
            string memberInput = (Console.ReadLine() ?? "").Trim().ToLower();
            if (memberInput == "")
            {
                Console.WriteLine("Reservasi ditolak: member tamu tidak boleh kosong.");
                return;
            }

            Console.Write("Jenis kamar (standard/deluxe/suite): ");
            string type = (Console.ReadLine() ?? "").Trim();
            if (type == "")
            {
                Console.WriteLine("Reservasi ditolak: type tidak boleh kosong.");
                return;
            }

            Console.Write("Lama menginap (malam): ");
            int nights;
            if (!int.TryParse(Console.ReadLine(), out nights))
            {
                nights = 0;
            }

            Console.Write("Hari mulai menginap (1=Senin, 2=Selasa, ... 7=Minggu): ");
            int startDay;
            if (!int.TryParse(Console.ReadLine(), out startDay) || startDay < 1 || startDay > 7)
            {
                Console.WriteLine("Reservasi ditolak: hari mulai harus angka 1 sampai 7.");
                return;
            }

            try
            {
                Guest guest = new Guest("G" + (guestCounter + 1).ToString("000"), name);
                guest.Member = (memberInput == "y");

                Room? room = hotel.FindAvailableRoom(type);
                if (room == null)
                {
                    // semua kamar jenis ini penuh -> masuk daftar tunggu (Queue)
                    int position = hotel.AddToWaitingList(guest, type, nights, startDay);
                    guestCounter++;
                    Console.WriteLine("Semua kamar " + type + " penuh. " + name + " masuk daftar tunggu, antrian ke-" + position + ".");
                    Console.WriteLine("Kamar akan diberikan otomatis saat ada tamu " + type + " yang check-out.");
                    return;
                }

                Reservation res = new Reservation(guest, room, nights, startDay);
                hotel.CreateReservation(res);
                guestCounter++;

                Console.WriteLine("Reservasi berhasil!");
                guest.Info();
                Console.WriteLine($"Kamar    : {room.RoomNumber} ({room.GetType().Name}) selama {res.Nights} malam");
                Console.WriteLine("Silakan lakukan check-in lewat menu 3.");
            }
            catch (InvalidStayDurationException ex)
            {
                Console.WriteLine($"Reservasi ditolak:  {ex.Message}");
            }
            catch (RoomNotAvailableException ex)
            {
                Console.WriteLine($"Reservasi ditolak: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Reservasi ditolak: {ex.Message}");
            }
        }

        static void CheckInMenu()
        {
            Console.WriteLine("\n------ CHECK-IN ------");

            Console.Write("Nomor kamar yang dipesan: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();
            
            try
            {
                if (hotel.FindRoomByNumber(roomNumber) == null)
                {
                    Console.WriteLine($"Check-in ditolak: kamar {roomNumber} tidak ditemukan.");
                    return;
                }

                Reservation? found = hotel.FindReservation(roomNumber, "Reserved");

                if (found == null)
                {
                    if (hotel.FindReservation(roomNumber, "CheckedIn") != null)
                    {
                        throw new RoomNotAvailableException(roomNumber, $"Kamar {roomNumber} sedang terisi.");
                    }

                    Console.WriteLine($"Check-in ditolak: tidak ada reservasi untuk kamar {roomNumber}. Silakan reservasi dulu (menu 2).");
                    return;
                }

                if (found.CheckIn())
                {
                    Console.WriteLine($"Check-in berhasil! {found.Guest.Name} (ID {found.Guest.GuestId}) di kamar {roomNumber} selama {found.Nights} malam.");
                }
                else
                {
                    Console.WriteLine("Check-in ditolak.");
                }
            }
            catch (RoomNotAvailableException ex)
            {
                Console.WriteLine($"Check-in ditolak: {ex.Message}");
            }
        }

        static void RoomServiceMenu()
        {
            Console.WriteLine("\n------ ROOM SERVICE ------");

            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            Reservation? found = hotel.FindReservation(roomNumber, "CheckedIn");
            if (found == null)
            {
                Console.WriteLine("Ditolak: tidak ada tamu yang menginap di kamar " + roomNumber + ".");
                return;
            }

            try
            {
                IRoomService? service = found.Room as IRoomService;
                if (service == null)
                {
                    throw new RoomServiceNotSupportedException(roomNumber);
                }

                Console.Write("Nama menu: ");
                string menu = (Console.ReadLine() ?? "").Trim();

                Console.Write("Harga: ");
                int price;
                if (!int.TryParse(Console.ReadLine(), out price))
                {
                    price = 0;
                }

                if (service.OrderRoomService(menu, price))
                {
                    Console.WriteLine($"Pesanan '{menu}' ({price:N0}) dicatat di kamar {roomNumber}. Total room service: {service.TotalRoomService():N0}");
                }
                else
                {
                    Console.WriteLine("Ditolak: nama menu tidak boleh kosong dan harga harus lebih dari 0.");
                }
            }
            catch (RoomServiceNotSupportedException ex)
            {
                Console.WriteLine("Ditolak: " + ex.Message);
            }
        }

        static async Task CheckOutMenuAsync()
        {
            Console.WriteLine("\n------ CHECK-OUT ------");

            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            Reservation? found = hotel.FindReservation(roomNumber, "CheckedIn");
            if (found == null)
            {
                Console.WriteLine("Check-out gagal: tidak ada tamu yang menginap di kamar " + roomNumber + ".");
                return;
            }

            string type = found.Room.GetType().Name;
            int total = found.CheckOut();
            Console.WriteLine($"Check-out selesai. Total dibayar: {total:N0}");
            Console.WriteLine($"Status kamar {roomNumber}: {found.Room.Status}");

            await hotel.CleanRoomAsync(found.Room);

            // kamar kosong -> tamu terdepan di daftar tunggu jenis ini mendapat kamar
            Reservation? promoted = hotel.ProcessWaitingList(type);
            if (promoted != null)
            {
                Console.WriteLine();
                Console.WriteLine($"[Daftar tunggu] {promoted.Guest.Name} (ID {promoted.Guest.GuestId}) mendapat kamar {promoted.Room.RoomNumber} ({type}) selama {promoted.Nights} malam.");
                Console.WriteLine("Silakan lakukan check-in lewat menu 3.");
            }
        }

        static void OccupancyReport()
        {
            Console.WriteLine("\n------ LAPORAN HUNIAN ------");
            Console.WriteLine($"Kamar terisi : {hotel.OccupiedCount()} dari {Room.TotalRooms} kamar");
            Console.WriteLine($"Persentase   : {hotel.OccupancyRate()}%");
            hotel.PrintOccupancyByType();
        }

        static void AddRoomMenu()
        {
            Console.WriteLine("\n------ TAMBAH KAMAR ------");

            Console.Write("Jenis kamar (standard/deluxe/suite): ");
            string type = (Console.ReadLine() ?? "").Trim().ToLower();

            Console.Write("Nomor kamar: ");
            string number = (Console.ReadLine() ?? "").Trim();

            Console.Write("Lantai: ");
            int floor;
            if (!int.TryParse(Console.ReadLine(), out floor))
            {
                Console.WriteLine("Ditolak: lantai harus angka.");
                return;
            }

            Console.Write("Tarif per malam: ");
            int rate;
            if (!int.TryParse(Console.ReadLine(), out rate))
            {
                Console.WriteLine("Ditolak: tarif harus angka.");
                return;
            }

            try
            {
                Room room;
                switch (type)
                {
                    case "standard":
                        room = new Standard(number, floor, rate, "Available");
                        break;
                    case "deluxe":
                        room = new Deluxe(number, floor, rate, "Available");
                        break;
                    case "suite":
                        room = new Suite(number, floor, rate, "Available");
                        break;
                    default:
                        Console.WriteLine("Ditolak: jenis kamar tidak dikenal.");
                        return;
                }

                Reservation? promoted = hotel.AddRoom(room);
                Console.WriteLine("Kamar " + number + " (" + room.GetType().Name + ") berhasil ditambahkan.");

                if (promoted != null)
                {
                    Console.WriteLine($"[Daftar tunggu] {promoted.Guest.Name} (ID {promoted.Guest.GuestId}) mendapat kamar {promoted.Room.RoomNumber} selama {promoted.Nights} malam.");
                    Console.WriteLine("Silakan lakukan check-in lewat menu 3.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ditolak: {ex.Message}");
            }
        }

        private static void TotalRoomService()
        {
            Console.WriteLine("\n------ TOTAL ROOM SERVICE ------");

            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            Reservation? found = hotel.FindReservation(roomNumber, "CheckedIn");
            if (found == null)
            {
                Console.WriteLine("Ditolak: tidak ada tamu yang menginap di kamar " + roomNumber + ".");
                return;
            }

            try
            {
                IRoomService? service = found.Room as IRoomService;
                if (service == null)
                {
                    throw new RoomServiceNotSupportedException(roomNumber);
                }

                service.PrintTotalRoomService();

            }
            catch (RoomServiceNotSupportedException ex)
            {
                Console.WriteLine("Ditolak: " + ex.Message);
            }
        }

        static async Task OnlineReservationMenuAsync()
        {
            Console.WriteLine("\n------ RESERVASI ONLINE (SIMULASI) ------");

            Console.Write("Nama tamu: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (name == "")
            {
                Console.WriteLine("Reservasi ditolak: nama tamu tidak boleh kosong.");
                return;
            }

            Console.Write("Member? (y/n): ");
            string memberInput = (Console.ReadLine() ?? "").Trim().ToLower();
            if (memberInput == "")
            {
                Console.WriteLine("Reservasi ditolak: member tamu tidak boleh kosong.");
                return;
            }

            Console.Write("Jenis kamar (standard/deluxe/suite): ");
            string type = (Console.ReadLine() ?? "").Trim();
            if (type == "")
            {
                Console.WriteLine("Reservasi ditolak: type tidak boleh kosong.");
                return;
            }

            Console.Write("Lama menginap (malam): ");
            int nights;
            if (!int.TryParse(Console.ReadLine(), out nights))
            {
                nights = 0;
            }

            Console.Write("Hari mulai menginap (1=Senin, 2=Selasa, ... 7=Minggu): ");
            int startDay;
            if (!int.TryParse(Console.ReadLine(), out startDay) || startDay < 1 || startDay > 7)
            {
                Console.WriteLine("Reservasi ditolak: hari mulai harus angka 1 sampai 7.");
                return;
            }

            try
            {
                Guest guest = new Guest("G" + (guestCounter + 1).ToString("000"), name);
                guest.Member = memberInput == "y";
                Room? room = hotel.FindAvailableRoom(type);
                if (room == null)
                {
                    Console.WriteLine($"Tidak ada kamar {type} yang tersedia untuk reservasi online.");
                    return;
                }

                Reservation reservation = new Reservation(guest, room, nights, startDay);
                reservation.MarkPendingConfirmation();
                hotel.CreateReservation(reservation);
                guestCounter++;

                Console.WriteLine($"Reservasi sementara untuk kamar {room.RoomNumber} dibuat.");
                Console.WriteLine($"Ketik Y lalu Enter untuk konfirmasi dalam {timeoutSeconds} detik.");
                Console.Write("Konfirmasi: ");

                using CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

                try
                {
                    Task<string?> inputTask = Task.Run(Console.ReadLine);

                    string? confirmation = await inputTask.WaitAsync(cts.Token);

                    if (string.Equals(confirmation?.Trim(), "Y", StringComparison.OrdinalIgnoreCase))
                    { 
                        reservation.ConfirmOnlineReservation();
                        Console.WriteLine($"Reservasi dikonfirmasi. Kamar {room.RoomNumber} berstatus Reserved.");
                    }
                    else
                    {
                        reservation.CancelPendingReservation();
                        Console.WriteLine("Reservasi online dibatalkan karena tidak dikonfirmasi.");
                    }
                }
                catch (OperationCanceledException)
                {
                    reservation.CancelPendingReservation();
                    Console.WriteLine("Waktu konfirmasi habis. Reservasi online dibatalkan.");
                }
            }
            catch (InvalidStayDurationException ex)
            {
                Console.WriteLine($"Reservasi ditolak: {ex.Message}");
            }
            catch (RoomNotAvailableException ex)
            {
                Console.WriteLine($"Reservasi ditolak: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Reservasi ditolak: {ex.Message}");
            }
        }
    }
}
