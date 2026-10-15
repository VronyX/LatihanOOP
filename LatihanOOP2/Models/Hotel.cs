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

        };
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
