using LatihanOOP.Models;
using LatihanOOP.Models.Rooms;
using LatihanOOP.Enums;

namespace LatihanOOP.Services;

public class Hotel
{
    private Room[] _rooms = Array.Empty<Room>();
    private Reservation[] _reservations = new Reservation[100];
    private int reservationCount;

    public int TotalRooms => _rooms.Length;

    public void InitializeRooms()
    {
        _rooms = new Room[15];

        for (int i = 0; i < 5; i++)
        {
            _rooms[i] = new Standard(101 + i, 1, 300000m);
            _rooms[i + 5] = new Deluxe(201 + i, 2, 500000m);
            _rooms[i + 10] = new Suite(301 + i, 3, 800000m);
        }
    }

    public void DisplayRooms()
    {
        Console.WriteLine("\n========== ROOM LIST ==========");

        foreach (Room room in _rooms)
        {
            string status = room.IsBooked ? "Occupied" : "Available";

            Console.WriteLine(
                $"No: {room.No} | Floor: {room.Floor} | " +
                $"Type: {room.GetType().Name} | " +
                $"Charge: Rp{room.Price:N0} | Status: {status}");
        }
    }

    public Reservation? CreateReservation(
        DateTime dateIn, int lengthOfStay,
        Guest guest, string roomType)
    {
        if (!Reservation.ValidateLengthOfStay(lengthOfStay))
        {
            Console.WriteLine(
                "Length of stay must be between 1 and 14 nights.");
            return null;
        }

        if (dateIn.Date < DateTime.Today)
        {
            Console.WriteLine(
                $"Booking failed. Check-in date cannot be before today ({DateTime.Today:dd-MM-yyyy}).");

            return null;
        }

        DateTime dateOut = dateIn.Date.AddDays(lengthOfStay);
        Room? room = Reservation.FindAvailableRoom(
            _rooms,
            _reservations,
            reservationCount,
            dateIn,
            dateOut,
            roomType);

        if (room == null)
        {
            Console.WriteLine("No room is available for the selected dates and room type.");
            return null;
        }

        Reservation reservation =
            new Reservation(dateIn, lengthOfStay, guest, room);

        _reservations[reservationCount] = reservation;
        reservationCount++;

        Console.WriteLine($"Booking successful. Reservation ID: {reservation.GetId()}");
        Console.WriteLine($"Room number: {room.No}");

        return reservation;
    }

    public Reservation? FindReservation(int id)
    {
        return _reservations.FirstOrDefault(item => item.GetId() == id);
    }

    public float GetRoomPercentage()
    {
        if (TotalRooms == 0)
            return 0;

        int occupied = _rooms.Count(room => room.IsBooked);

        return (float)occupied / TotalRooms * 100;
    }

    public void DisplayReservations()
    {
        Console.WriteLine("\n========== RESERVATION LIST ==========");

        if (reservationCount == 0)
        {
            Console.WriteLine("No reservations found.");
            return;
        }

        for (int i = 0; i < reservationCount; i++)
        {
            Reservation reservation = _reservations[i];

            Console.WriteLine(
                $"ID: {reservation.GetId()} | " +
                $"Guest: {reservation.GetGuest().Name} | " +
                $"Room: {reservation.GetRoom().No} | " +
                $"Check-in: {reservation.GetDateIn():dd-MM-yyyy} | " +
                $"Check-out: {reservation.GetDateOut():dd-MM-yyyy} | " +
                $"Status: {reservation.GetReservationStatus()}");
        }
    }
}