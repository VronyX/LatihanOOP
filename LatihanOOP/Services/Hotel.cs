using LatihanOOP.Models;
using LatihanOOP.Models.Rooms;
using LatihanOOP.Enums;

namespace LatihanOOP.Services;

public class Hotel
{
    private Room[] _rooms = Array.Empty<Room>();
    private readonly List<Reservation> _reservations = new();

    public int TotalRooms => _rooms.Length;

    public void InitializeRooms()
    {
        _rooms = new Room[15];

        for (int i = 0; i < 5; i++)
        {
            _rooms[i] = new Standard(101 + i, 1, 300000m);
            _rooms[i + 5] = new Deluxe(201 + i, 2, 500000m);
            _rooms[i + 10] = new Suite(301 + i, 3, 800000m);
        }
    }

    public void DisplayRooms()
    {
        Console.WriteLine("\n========== DAFTAR KAMAR ==========");

        foreach (Room room in _rooms)
        {
            string status = room.IsBooked ? "Terisi" : "Kosong";

            Console.WriteLine(
                $"No: {room.No} | Lantai: {room.Floor} | " +
                $"Tipe: {room.GetType().Name} | " +
                $"Tarif: Rp{room.Price:N0} | Status: {status}");
        }
    }

    public Room? FindAvailableRoom(
        DateTime dateIn, DateTime dateOut, string roomType)
    {
        return _rooms.FirstOrDefault(room =>
            room.GetType().Name.Equals(
                roomType, StringComparison.OrdinalIgnoreCase)
            && !_reservations.Any(reservation =>
                reservation.Room.No == room.No
                && (reservation.Status == RoomStatus.Reserved
                    || reservation.Status == RoomStatus.Occupied)
                && dateIn.Date < reservation.DateOut
                && dateOut.Date > reservation.DateIn));
    }

    public Reservation? CreateReservation(
        DateTime dateIn, int lengthOfStay,
        Guest guest, string roomType)
    {
        if (lengthOfStay < 1 || lengthOfStay > 14)
        {
            Console.WriteLine("Lama menginap harus 1-14 malam.");
            return null;
        }

        if (dateIn.Date < DateTime.Today)
        {
            Console.WriteLine(
                $"Booking gagal. Tanggal check-in minimal hari ini ({DateTime.Today:dd-MM-yyyy}).");

            return null;
        }

        DateTime dateOut = dateIn.Date.AddDays(lengthOfStay);
        Room? room = FindAvailableRoom(dateIn, dateOut, roomType);

        if (room == null)
        {
            Console.WriteLine("Kamar tidak tersedia pada tanggal tersebut.");
            return null;
        }

        Reservation reservation =
            new Reservation(dateIn, lengthOfStay, guest, room);

        _reservations.Add(reservation);

        Console.WriteLine($"Reservasi berhasil. ID: {reservation.ID}");
        Console.WriteLine($"Nomor kamar: {room.No}");

        return reservation;
    }

    public Reservation? FindReservation(int id)
    {
        return _reservations.FirstOrDefault(item => item.ID == id);
    }

    public float GetRoomPercentage()
    {
        if (TotalRooms == 0)
            return 0;

        int occupied = _rooms.Count(room => room.IsBooked);

        return (float)occupied / TotalRooms * 100;
    }

    public void DisplayReservations()
    {
        foreach (Reservation reservation in _reservations)
        {
            Console.WriteLine(
                $"ID: {reservation.ID} | " +
                $"Tamu: {reservation.Guest.Name} | " +
                $"Kamar: {reservation.Room.No} | " +
                $"Status: {reservation.Status}");
        }
    }
}