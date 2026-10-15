using System;
using LatihanOOP2.Interfaces;
using LatihanOOP2.Models;

namespace LatihanOOP2
{
    internal class Program
    {
        static Hotel hotel = new Hotel();
        static int guestCounter = 0;
        static Reservation[] reservations = new Reservation[50];
        static int reservationCount = 0;

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
                        CheckOutMenu();
                        break;
                    case "6":
                        OccupancyReport();
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

            Room? room = hotel.FindAvailableRoom(type);
            if (room == null)
            {
                Console.WriteLine("Reservasi ditolak: tidak ada kamar " + type + " yang tersedia (atau jenis kamar salah).");
                return;
            }

            Guest guest = new Guest("G" + (guestCounter + 1).ToString("000"), name);
            guest.Member = (memberInput == "y");

            Reservation res = new Reservation(guest, room, nights, startDay);

            if (!hotel.CreateReservation(res))
            {
                if (res.Nights == 0)
                {
                    Console.WriteLine("Reservasi ditolak: lama menginap di luar 1-14 malam.");
                }
                else
                {
                    Console.WriteLine("Reservasi ditolak: kamar sedang terisi atau sudah dipesan.");
                }
                return;
            }

            guestCounter++;
            Array.Resize(ref reservations, reservationCount + 1);
            reservations[reservationCount] = res;
            reservationCount++;

            Console.WriteLine("Reservasi berhasil!");
            guest.Info();
            Console.WriteLine($"Kamar    : {room.RoomNumber} ({room.GetType().Name}) selama {res.Nights} malam");
            Console.WriteLine("Silakan lakukan check-in lewat menu 3.");
        }

        static void CheckInMenu()
        {
            Console.WriteLine("\n------ CHECK-IN ------");

            Console.Write("Nomor kamar yang dipesan: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            Reservation? found = null;
            bool kamarTerisi = false;
            for (int i = 0; i < reservationCount; i++)
            {
                if (reservations[i].Room.RoomNumber == roomNumber)
                {
                    if (reservations[i].Status == "Reserved")
                    {
                        found = reservations[i];
                    }
                    else if (reservations[i].Status == "CheckedIn")
                    {
                        kamarTerisi = true;
                    }
                }
            }

            if (found == null)
            {
                if (kamarTerisi)
                {
                    Console.WriteLine("Check-in ditolak: kamar " + roomNumber + " sedang terisi.");
                }
                else
                {
                    Console.WriteLine("Check-in ditolak: tidak ada reservasi untuk kamar " + roomNumber + ". Silakan reservasi dulu (menu 2).");
                }
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

        static void RoomServiceMenu()
        {
            Console.WriteLine("\n------ ROOM SERVICE ------");

            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            Reservation? found = null;
            for (int i = 0; i < reservationCount; i++)
            {
                if (reservations[i].Status == "CheckedIn" && reservations[i].Room.RoomNumber == roomNumber)
                {
                    found = reservations[i];
                }
            }

            if (found == null)
            {
                Console.WriteLine("Ditolak: tidak ada tamu yang menginap di kamar " + roomNumber + ".");
                return;
            }

            if (found.Room is IRoomService service)
            {
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
            else
            {
                Console.WriteLine("Ditolak: kamar Standard tidak menyediakan room service.");
            }
        }

        static void CheckOutMenu()
        {
            Console.WriteLine("\n------ CHECK-OUT ------");

            Console.Write("Nomor kamar: ");
            string roomNumber = (Console.ReadLine() ?? "").Trim();

            Reservation? found = null;
            for (int i = 0; i < reservationCount; i++)
            {
                if (reservations[i].Status == "CheckedIn" && reservations[i].Room.RoomNumber == roomNumber)
                {
                    found = reservations[i];
                }
            }

            if (found == null)
            {
                Console.WriteLine("Check-out gagal: tidak ada tamu yang menginap di kamar " + roomNumber + ".");
                return;
            }

            int total = found.CheckOut();
            Console.WriteLine($"Check-out selesai. Total dibayar: {total:N0}");
        }

        static void OccupancyReport()
        {
            int occupied = 0;
            for (int i = 0; i < reservationCount; i++)
            {
                if (reservations[i].Status == "CheckedIn")
                {
                    occupied++;
                }
            }

            Console.WriteLine("\n------ LAPORAN HUNIAN ------");
            Console.WriteLine($"Kamar terisi : {occupied} dari {Room.TotalRooms} kamar");
            Console.WriteLine($"Persentase   : {hotel.OccupancyRate()}%");
        }

    }
}
