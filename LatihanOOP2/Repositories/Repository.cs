namespace LatihanOOP2.Repositories;

public class Repository <TRoom> where TRoom : Room
{
    private List<TRoom> rooms = new List<TRoom>();
    private Dictionary<string, TRoom> byNumber = new Dictionary<string, TRoom>();

    public void Add(TRoom room)
    {
        if (room == null)
        {
            throw new ArgumentNullException("room");
        }
        if (byNumber.ContainsKey(room.RoomNumber))
        {
            throw new ArgumentException($"Nomor kamar {room.RoomNumber} sudah terdaftar.");
        }

        rooms.Add(room);
        byNumber.Add(room.RoomNumber, room);
    }

    public TRoom? GetByNumber(string roomNumber)
    {
        TRoom? result;
        if (byNumber.TryGetValue(roomNumber, out result))
        {
            return result;
        }
        return null;
    }

    public List<TRoom> GetAll()
    {
        return new List<TRoom>(rooms);
    }
}
