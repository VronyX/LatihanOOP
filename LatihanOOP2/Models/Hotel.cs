namespace LatihanOOP2.Models;

public class Hotel
{
    private Room[] rooms;
    private Reservation[] reservations;
    private int reservationCount { get; set; }

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

        reservations = new Reservation[0];
        reservationCount = 0;
    }

    public Room? FindAvailableRoom(string type)
    {
        foreach (var room in rooms)
        {
            if (room.Status == "Available" && room.GetType().Name.Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                return room;
            }
        }

        return null;
    }

    public bool CreateReservation(Reservation res)
    {
        if (res.Room.Status == "Occupied")
        {
            return false;
        }

        reservations = reservations.Append(res).ToArray();
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
        foreach (var room in rooms)
        {
            if(room.Status == "Occupied")
            {
                occupiedRooms++;
            }
        }

        return (occupiedRooms * 100) / rooms.Length;
    }

    public void PrintRooms()
    {

    }
}
