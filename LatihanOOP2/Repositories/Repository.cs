namespace LatihanOOP2.Repositories;

public class Repository<T> where T : Room
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public List<T> GetAll() => new(_items);

    public T? FindByRoomNumber(string roomNumber)
    {
        return _items.FirstOrDefault(
            room => room.RoomNumber == roomNumber);
    }
}