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
    private int roomServiceTotal;

    public Reservation(DateTime dateIn, int lengthOfStay, Guest guest, Room room)
    {
        this.id = nextId++;
        this.dateIn = dateIn;
        this.dateOut = dateIn.AddDays(lengthOfStay);
        this.status = ReservationStatus.Reserved;
        this.guest = guest;
        this.room = room;
    }

    public ReservationStatus GetStatus()
    {
        return status;
    }

    public bool ValidateLengthOfStay(int lengthOfStay)
    {
        return lengthOfStay >= 1 && lengthOfStay <= 14;
    }

    public int GetLengthOfStay(DateTime dateIn, DateTime dateOut)
    {
        return (dateOut - dateIn).Days;
    }

    public int GetRoomServiceTotal()
    {
        return roomServiceTotal;
    }

    public bool CheckIn()
    {
        if (status != ReservationStatus.Reserved)
        {
            return false;
        }

        if (room.GetStatus())
        {
            return false;
        }

        room.SetIsBooked(true);
        status = ReservationStatus.CheckedIn;

        return true;
    }

    public Invoice CheckOut()
    {
        if (status != ReservationStatus.CheckedIn)
        {
            Console.WriteLine("Checkout gagal. Reservasi belum check-in atau sudah selesai.");

            return null;
        }

        Invoice invoice = new Invoice(this);

        room.SetIsBooked(false);
        status = ReservationStatus.CheckedOut;

        return invoice;
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

        int serviceCost = RoomServiceOrder.GetAdditionalServicePrice(serviceId);

        if (serviceCost <= 0)
        {
            return false;
        }

        roomServiceTotal += serviceCost;
        return true;
    }


    public void CancelReserved(int ID)
    {
        if (this.id != ID)
        {
            return;
        }

        if (status != ReservationStatus.Reserved)
        {
            return;
        }

        status = ReservationStatus.Cancelled;
    }

    public DateTime GetDateIn()
    {
        return dateIn;
    }

    public DateTime GetDateOut()
    {
        return dateOut;
    }

    public Room GetRoom()
    {
        return room;
    }

    public Guest GetGuest()
    {
        return guest;
    }
}
