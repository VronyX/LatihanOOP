namespace LatihanOOP2.Models;

public class Hotel
{
    private Room[] rooms;
    private Reservation[] reservations;
    private int reservationCount;

    public Hotel()
    {
        rooms = new Room[]
        {
            new Standard("101", 1, 500000, "Available"),
            new Standard("102", 1, 500000, "Available"),
            new Standard("103", 1, 500000, "Available"),

            new Deluxe("201", 2, 800000, "Available"),
            new Deluxe("202", 2, 800000, "Available"),
            new Deluxe("203", 2, 800000, "Available"),

            new Suite("301", 3, 1200000, "Available"),
            new Suite("302", 3, 1200000, "Available"),
            new Suite("303", 3, 1200000, "Available")
        };

        reservations = new Reservation[50];
        reservationCount = 0;
    }

    public Room? FindAvailableRoom(string type)
    {
        foreach (Room room in rooms)
        {
            if (room.Status != "Available")
            {
                continue;
            }
            if (!room.GetType().Name.Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            bool sudahDipesan = false;
            for (int i = 0; i < reservationCount; i++)
            {
                if (reservations[i].Status == "Reserved" && reservations[i].Room == room)
                {
                    sudahDipesan = true;
                }
            }

            if (!sudahDipesan)
            {
                return room;
            }
        }

        return null;
    }

    public bool CreateReservation(Reservation res)
    {
        if (res.Nights < 1 || res.Nights > 14)
        {
            return false;
        }

        if (res.Room.Status == "Occupied")
        {
            return false;
        }

        for (int i = 0; i < reservationCount; i++)
        {
            if (reservations[i].Status == "Reserved" && reservations[i].Room == res.Room)
            {
                return false;
            }
        }

        Array.Resize(ref reservations, reservationCount + 1);
        reservations[reservationCount] = res;
        reservationCount++;

        return true;
    }

    public int OccupancyRate()
    {
        if (rooms.Length == 0)
        {
            return 0;
        }

        int occupiedRooms = 0;
        foreach (Room room in rooms)
        {
            if (room.Status == "Occupied")
            {
                occupiedRooms++;
            }
        }

        return (occupiedRooms * 100) / rooms.Length;
    }

    public void PrintRooms()
    {
        Console.WriteLine("\n========== DAFTAR KAMAR ==========");

        foreach (Room room in rooms)
        {
            room.Info();

            for (int i = 0; i < reservationCount; i++)
            {
                if (reservations[i].Status == "Reserved" && reservations[i].Room == room)
                {
                    Console.WriteLine($"         >> Dipesan oleh {reservations[i].Guest.Name}, belum check-in");
                }
            }
        }
    }
}
