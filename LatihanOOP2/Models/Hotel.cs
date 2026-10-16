using LatihanOOP2.Exceptions;
using LatihanOOP2.Repositories;
using System.Xml.Linq;

namespace LatihanOOP2.Models;

public class Hotel
{
    private static readonly string[] RoomTypes = { "Standard", "Deluxe", "Suite" };

    private Repository<Room> rooms;
    private List<Reservation> reservations = new List<Reservation>();
    private Dictionary<string, Queue<WaitingGuest>> waitingLists;
    private int reservationCount
    {
        get { return reservations.Count; }
    }

    public Hotel()
    {
        rooms = new Repository<Room>();
        rooms.Add(new Standard("101", 1, 500000, "Available"));
        rooms.Add(new Standard("102", 1, 500000, "Available"));
        rooms.Add(new Standard("103", 1, 500000, "Available"));

        rooms.Add(new Deluxe("201", 2, 800000, "Available"));
        rooms.Add(new Deluxe("202", 2, 800000, "Available"));
        rooms.Add(new Deluxe("203", 2, 800000, "Available"));

        rooms.Add(new Suite("301", 3, 1200000, "Available"));
        rooms.Add(new Suite("302", 3, 1200000, "Available"));
        rooms.Add(new Suite("303", 3, 1200000, "Available"));

        reservations = new List<Reservation>();
        waitingLists = new Dictionary<string, Queue<WaitingGuest>>(StringComparer.OrdinalIgnoreCase);
        foreach (string type in RoomTypes)
        {
            waitingLists.Add(type, new Queue<WaitingGuest>());
        }
    }

    public bool CreateReservation(Reservation res)
    {
        if (res.Room.Status == "Occupied")
        {
            throw new RoomNotAvailableException(res.Room.RoomNumber, $"Kamar {res.Room.RoomNumber} sedang terisi.");
        }

        if (IsReserved(res.Room))
        {
            throw new RoomNotAvailableException(res.Room.RoomNumber, $"Kamar {res.Room.RoomNumber} sudah dipesan.");
        }

        reservations.Add(res);
        return true;
    }

    public int AddToWaitingList(Guest guest, string type, int nights, int startDay)
    {
        if (!IsValidType(type))
        {
            throw new ArgumentException("Jenis kamar '" + type + "' tidak dikenal.");
        }

        WaitingGuest waiting = new WaitingGuest(guest, type, nights, startDay);   // validasi 1-14 malam
        Queue<WaitingGuest> queue = waitingLists[type];
        queue.Enqueue(waiting);
        return queue.Count;
    }

    public Reservation? AddRoom(Room room)
    {
        try
        {
            rooms.Add(room);
        }
        catch (ArgumentException)
        {
            Room.TotalRooms--; 
            throw;
        }

        return ProcessWaitingList(room.GetType().Name);
    }

    public Reservation? ProcessWaitingList(string type)
    {
        Queue<WaitingGuest>? queue;
        if (!waitingLists.TryGetValue(type, out queue) || queue.Count == 0)
        {
            return null;
        }

        Room? room = FindAvailableRoom(type);
        if (room == null)
        {
            return null;
        }

        WaitingGuest next = queue.Dequeue();
        Reservation res = new Reservation(next.Guest, room, next.Nights, next.StartDay);
        CreateReservation(res);
        return res;
    }

    public int OccupiedCount()
    {
        int occupiedRooms = 0;
        foreach (Room room in rooms.GetAll())
        {
            if (room.Status == "Occupied")
            {
                occupiedRooms++;
            }
        }
        return occupiedRooms;
    }

    public int OccupancyRate()
    {
        if (rooms.GetAll().Count == 0)
        {
            return 0;
        }

        return (OccupiedCount() * 100) / rooms.GetAll().Count;
    }

    public void PrintOccupancyByType()
    {
        Console.WriteLine("Per jenis kamar:");
        foreach (string type in RoomTypes)
        {
            int total = 0;
            int occupied = 0;
            foreach (Room room in rooms.GetAll())
            {
                if (room.GetType().Name.Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    total++;
                    if (room.Status == "Occupied")
                    {
                        occupied++;
                    }
                }
            }

            int percent = total == 0 ? 0 : (occupied * 100) / total;
            Console.WriteLine($"  {type,-9}: {occupied} dari {total} kamar ({percent}%) | menunggu: {waitingLists[type].Count}");
        }
    }

    public void PrintWaitingList()
    {
        Console.WriteLine("\n========== DAFTAR TUNGGU ==========");
        bool any = false;
        foreach (string type in RoomTypes)
        {
            int position = 1;
            foreach (WaitingGuest w in waitingLists[type])
            {
                Console.WriteLine($"{type,-9} #{position} : {w.Guest.Name} ({w.Nights} malam)");
                position++;
                any = true;
            }
        }
        if (!any)
        {
            Console.WriteLine("Tidak ada tamu dalam daftar tunggu.");
        }
    }

    public void PrintRooms()
    {
        Console.WriteLine("\n========== DAFTAR KAMAR ==========");

        foreach (Room room in rooms.GetAll())
        {
            room.Info();

            GetRoomInfo(room);
            Console.WriteLine();
        }
    }

    #region Helper
    public Room? FindAvailableRoom(string type)
    {
        foreach (Room room in rooms.GetAll())
        {
            if (room.Status != "Available")
            {
                continue;
            }
            if (!room.GetType().Name.Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!IsReserved(room))
            {
                return room;
            }
        }

        return null;
    }

    public Room? FindRoomByNumber(string roomNumber)
    {
        return rooms.GetByNumber(roomNumber);
    }

    public Reservation? FindReservation(string roomNumber, string status)
    {
        foreach (Reservation res in reservations)
        {
            if (res.Status == status && res.Room.RoomNumber == roomNumber)
            {
                return res;
            }
        }
        return null;
    }

    private bool IsReserved(Room room)
    {
        for (int i = 0; i < reservationCount; i++)
        {
            if (reservations[i].Status == "Reserved" && reservations[i].Room == room)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsValidType(string type)
    {
        foreach (string t in RoomTypes)
        {
            if (t.Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private void GetRoomInfo(Room room)
    {
        for (int i = 0; i < reservationCount; i++)
        {
            if (reservations[i].Status == "Reserved" && reservations[i].Room == room)
            {
                Console.WriteLine($"         >> Dipesan oleh {reservations[i].Guest.Name}, belum check-in");
            }
        }
    }
#endregion
}
