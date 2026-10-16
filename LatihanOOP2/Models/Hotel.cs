
using LatihanOOP2.Exceptions;
using LatihanOOP2.Repositories;

namespace LatihanOOP2.Models;

public class Hotel
{
    private readonly List<Reservation> _reservations = new();
    private readonly Dictionary<string, Room> _roomsByNumber = new();
    private readonly Queue<WaitlistEntry> _waitlist = new();
    private readonly Repository<Room> _repository = new();

    public List<Room> Rooms => _repository.GetAll();
    public List<Reservation> Reservations => new(_reservations);

    public Hotel()
    {
        AddRoom(new Standard("101", 1, 500000, "Available"));
        AddRoom(new Standard("102", 1, 500000, "Available"));
        AddRoom(new Standard("103", 1, 500000, "Available"));

        AddRoom(new Deluxe("201", 2, 800000, "Available"));
        AddRoom(new Deluxe("202", 2, 800000, "Available"));
        AddRoom(new Deluxe("203", 2, 800000, "Available"));

        AddRoom(new Suite("301", 3, 1200000, "Available"));
        AddRoom(new Suite("302", 3, 1200000, "Available"));
        AddRoom(new Suite("303", 3, 1200000, "Available"));
    }

    private void AddRoom(Room room)
    {
        _repository.Add(room);
        _roomsByNumber.Add(room.RoomNumber, room);
    }

    public Room? FindRoom(string roomNumber)
    {
        return _roomsByNumber.TryGetValue(roomNumber, out var room)
            ? room
            : null;
    }

    public Reservation? FindReservation(string roomNumber, string status)
    {
        return _reservations.LastOrDefault(r =>
            r.Room.RoomNumber == roomNumber &&
            r.Status == status);
    }

    public Room? FindAvailableRoom(string type)
    {
        return Rooms.FirstOrDefault(room =>
            room.GetType().Name.Equals(
                type, StringComparison.OrdinalIgnoreCase) &&
            room.Status == "Available" &&
            !_reservations.Any(r =>
                r.Room == room && r.Status == "Reserved"));
    }

    public Reservation CreateReservation(
        Guest guest, string type, int nights, int startDay)
    {
        if (nights < 1 || nights > 14)
        {
            throw new InvalidStayDurationException(
                "Lama menginap harus antara 1 sampai 14 malam.");
        }

        Room? room = FindAvailableRoom(type);

        if (room == null)
        {
            _waitlist.Enqueue(
                new WaitlistEntry(guest, type, nights, startDay));

            throw new RoomNotAvailableException(
                $"Kamar {type} penuh. Tamu {guest.Name} " +
                "dimasukkan ke daftar tunggu.");
        }

        var reservation = new Reservation(guest, room, nights, startDay);
        _reservations.Add(reservation);

        return reservation;
    }

    public void CheckIn(string roomNumber)
    {
        var reservation = FindReservation(roomNumber, "Reserved");

        if (reservation == null)
        {
            throw new RoomNotAvailableException(
                $"Tidak ada reservasi aktif untuk kamar {roomNumber}.");
        }

        reservation.CheckIn();
    }

    public int CheckOut(string roomNumber)
    {
        var reservation = FindReservation(roomNumber, "CheckedIn");

        if (reservation == null)
        {
            throw new RoomNotAvailableException(
                $"Tidak ada tamu yang check-in di kamar {roomNumber}.");
        }

        int total = reservation.CheckOut();

        AssignWaitlistGuest(reservation.Room.GetType().Name);

        return total;
    }

    public int OrderRoomService(
        string roomNumber, string menu, int price)
    {
        var reservation = FindReservation(roomNumber, "CheckedIn");

        if (reservation == null)
        {
            throw new RoomNotAvailableException(
                $"Tidak ada tamu yang check-in di kamar {roomNumber}.");
        }

        return reservation.OrderRoomService(menu, price);
    }

    private void AssignWaitlistGuest(string type)
    {
        if (_waitlist.Count == 0)
            return;

        var waitingGuests = _waitlist.ToArray();
        _waitlist.Clear();

        bool assigned = false;

        foreach (var entry in waitingGuests)
        {
            if (!assigned &&
                entry.RoomType.Equals(
                    type, StringComparison.OrdinalIgnoreCase))
            {
                var room = FindAvailableRoom(type);

                if (room != null)
                {
                    _reservations.Add(new Reservation(
                        entry.Guest,
                        room,
                        entry.Nights,
                        entry.StartDay));

                    Console.WriteLine(
                        $"Kamar {room.RoomNumber} disiapkan untuk tamu " +
                        $"{entry.Guest.Name} dari daftar tunggu.");

                    assigned = true;
                    continue;
                }
            }

            _waitlist.Enqueue(entry);
        }
    }

    public void PrintRooms()
    {
        Console.WriteLine("\n========== DAFTAR KAMAR ==========");

        foreach (var room in Rooms)
        {
            room.Info();

            var reservation = _reservations.LastOrDefault(r =>
                r.Room == room && r.Status == "Reserved");

            if (reservation != null)
            {
                Console.WriteLine(
                    $"  Dipesan oleh: {reservation.Guest.Name}");
            }
        }

        Console.WriteLine(
            $"Jumlah tamu dalam antrean: {_waitlist.Count}");
    }

    public int OccupancyRate()
    {
        if (Rooms.Count == 0)
            return 0;

        int occupied = Rooms.Count(room => room.Status == "Occupied");

        return occupied * 100 / Rooms.Count;
    }

    public void PrintOccupancyReport()
    {
        Console.WriteLine("\n========== LAPORAN HUNIAN ==========");

        foreach (string type in new[] { "Standard", "Deluxe", "Suite" })
        {
            int total = Rooms.Count(room => room.GetType().Name == type);

            int occupied = Rooms.Count(room =>
                room.GetType().Name == type &&
                room.Status == "Occupied");

            Console.WriteLine(
                $"{type,-10}: {occupied}/{total} kamar terisi");
        }

        Console.WriteLine($"Total kamar  : {Rooms.Count}");
        Console.WriteLine($"Hunian       : {OccupancyRate()}%");
        Console.WriteLine($"Daftar tunggu: {_waitlist.Count}");
    }
}
