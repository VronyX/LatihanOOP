using LatihanOOP.Enums;
using LatihanOOP.Models.Rooms;
using LatihanOOP.Services;

namespace LatihanOOP.Models;

public class Reservation
{
    private static int nextId = 1;

    private int id;
    private DateTime dateIn;
    private DateTime dateOut;
    private ReservationStatus status;
    private Guest guest;
    private Room room;
    private decimal roomServiceTotal;
    private RoomServiceOrder[] orders = new RoomServiceOrder[50];
    private int orderCount;


    public Reservation(DateTime dateIn, int lengthOfStay, Guest guest, Room room)
    {
        this.id = nextId++;
        this.dateIn = dateIn.Date;
        this.dateOut = dateIn.Date.AddDays(lengthOfStay);
        this.guest = guest;
        this.room = room;
        this.status = ReservationStatus.Reserved;

        this.roomServiceTotal = 0;
        this.orderCount = 0;
    }

    public static bool ValidateLengthOfStay(int lengthOfStay)
    {
        return lengthOfStay >= 1 && lengthOfStay <= 14;
    }

    public static Room? FindAvailableRoom(Room[] rooms, Reservation[] reservations, int reservationCount, DateTime dateIn, DateTime dateOut, string roomType)
    {
        foreach (Room room in rooms)
        {
            if (!room.GetType().Name.Equals(
                    roomType,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (IsRoomAvailable(
                    room,
                    reservations,
                    reservationCount,
                    dateIn,
                    dateOut))
            {
                return room;
            }
        }

        return null;
    }

    private static bool IsRoomAvailable(Room room, Reservation[] reservations, int reservationCount, DateTime dateIn, DateTime dateOut)
    {
        for (int i = 0; i < reservationCount; i++)
        {
            Reservation reservation = reservations[i];

            if (reservation.room.No != room.No)
            {
                continue;
            }

            if (reservation.status is ReservationStatus.Cancelled or ReservationStatus.CheckedOut)
            {
                continue;
            }

            bool dateOverlap = dateIn.Date < reservation.dateOut.Date && dateOut.Date > reservation.dateIn.Date;

            if (dateOverlap)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckIn()
    {
        if (status != ReservationStatus.Reserved)
        {
            return false;
        }
        if (room.IsBooked)
        {
            return false;
        }

        if (DateTime.Today < dateIn || DateTime.Today >= dateOut)
        {
            return false;
        }

        room.SetIsBooked(true);
        status = ReservationStatus.CheckedIn;

        return true;
    }

    public Invoice? CheckOut()
    {
        if (status != ReservationStatus.CheckedIn)
        {
            return null;
        }

        return new Invoice(this);
    }

    public bool CompleteCheckOut()
    {
        if (status != ReservationStatus.CheckedIn)
        {
            return false;
        }

        room.SetIsBooked(false);
        status = ReservationStatus.CheckedOut;

        return true;
    }

    public bool AddRoomService(int serviceId)
    {
        if (status != ReservationStatus.CheckedIn)
        { 
            return false;
        }

        if (!room.SupportRoomService())
        { 
            return false;
        }

        decimal servicePrice = RoomServiceOrder.GetServicePrice(serviceId);

        string serviceName = RoomServiceOrder.GetServiceName(serviceId);

        if (servicePrice <= 0 || string.IsNullOrEmpty(serviceName))
        {
            return false;
        }

        if (orderCount >= orders.Length)
        { 
            return false;
        }

        RoomServiceOrder order = new RoomServiceOrder(serviceName, servicePrice);

        orders[orderCount] = order;
        orderCount++;

        roomServiceTotal += servicePrice;

        return true;
    }

    public RoomServiceOrder[] GetOrders()
    {
        RoomServiceOrder[] result = new RoomServiceOrder[orderCount];

        for (int i = 0; i < orderCount; i++)
        {
            result[i] = orders[i];
        }

        return result;
    }

    public bool CancelReserved()
    {
        if (status != ReservationStatus.Reserved)
        {
            return false;
        }

        status = ReservationStatus.Cancelled;
        return true;
    }

    #region Get Methods
    public int GetId() => id;
    public ReservationStatus GetReservationStatus() => status;
    public DateTime GetDateIn() => dateIn;
    public DateTime GetDateOut() => dateOut;
    public int GetLengthOfStay(DateTime dateIn, DateTime dateOut) => (dateOut - dateIn).Days;
    public Room GetRoom() => room;
    public Guest GetGuest() => guest;
    public decimal GetRoomServiceTotal() => roomServiceTotal;
    #endregion
}