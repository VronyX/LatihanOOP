namespace LatihanOOP2.Models;

public class Hotel
{
    private Room[] rooms = new Room[0];
    private Reservation[] reservations = new Reservation[0];
    private int reservationCount {  get; }

    public Hotel()
    {
        rooms = new Room[]
        {

        };
    }

    public Room FindAvailableRoom(string type)
    {
        Room room = new Room();

        return room;
    }

    public bool CreateReservation(Reservation res)
    {
        return true;
    }

    public int OccupancyRate()
    {
        int percentage = 0;
        foreach (var room in rooms)
        {
            if(room.Status == "Occupied")
            {
                percentage += 
            }
        }
        return 0;
    }

    public void PrintRooms()
    {

    }
}
