using LatihanOOP.Enums;
using LatihanOOP.Models.Rooms;

namespace LatihanOOP.Models;

public class Reservation
{
    private static int _nextId = 1;
    private readonly List<(string Name, decimal Amount)> _orders = new();

    public int ID { get; }
    public DateTime DateIn { get; }
    public DateTime DateOut { get; }
    public RoomStatus Status { get; private set; }
    public Guest Guest { get; }
    public Room Room { get; }

    public decimal RoomServiceTotal =>
        _orders.Sum(order => order.Amount);

    public IReadOnlyList<(string Name, decimal Amount)> Orders => _orders;

    public Reservation(
        DateTime dateIn,
        int lengthOfStay,
        Guest guest,
        Room room)
    {
        if (dateIn.Date < DateTime.Today)
            throw new ArgumentException(
                $"Tanggal check-in tidak boleh sebelum hari ini ({DateTime.Today:dd-MM-yyyy}).");

        if (!ValidateLengthOfStay(lengthOfStay))
            throw new ArgumentException(
                "Lama menginap harus antara 1 sampai 14 malam.");

        ID = _nextId++;
        DateIn = dateIn.Date;
        DateOut = dateIn.Date.AddDays(lengthOfStay);
        Guest = guest;
        Room = room;
        Status = RoomStatus.Reserved;
    }

    public bool GetStatus() => Status == RoomStatus.Occupied;

    public bool ValidateLengthOfStay(int lengthOfStay)
        => lengthOfStay >= 1 && lengthOfStay <= 14;

    public int GetLengthOfStay(DateTime dateIn, DateTime dateOut)
        => (dateOut - dateIn).Days;

    public bool CheckIn()
    {
        if (Status != RoomStatus.Reserved || Room.IsBooked)
            return false;

        if (DateTime.Today < DateIn || DateTime.Today >= DateOut)
            return false;

        Room.SetIsBooked(true);
        Status = RoomStatus.Occupied;

        return true;
    }

    public Invoice? CheckOut()
    {
        if (Status != RoomStatus.Occupied)
            return null;

        Invoice invoice = new Invoice(this);

        Room.SetIsBooked(false);
        Status = RoomStatus.Available;

        return invoice;
    }

    // Mencatat room service jika tipe kamar mendukung.
    public bool AddRoomService(int amount)
    {
        if (Status != RoomStatus.Occupied || !Room.SupportRoomService())
            return false;

        (string Name, decimal Amount) order = amount switch
        {
            1 => ("Nasi Goreng", 50000m),
            2 => ("Coca-Cola", 15000m),
            3 => ("Ayam Penyet", 35000m),
            _ => ("", 0m)
        };

        if (order.Amount == 0)
            return false;

        _orders.Add(order);
        return true;
    }

    public void CancelReserved(int ID)
    {
        if (this.ID != ID || Status != RoomStatus.Reserved)
            return;

        Status = RoomStatus.Available;
    }
}