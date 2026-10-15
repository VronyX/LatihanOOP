using LatihanOOP.Models;
using LatihanOOP.Services;
using LatihanOOP.Views;
using System.Threading.Channels;

namespace LatihanOOP;

public class Program
{
    private static readonly Hotel _hotel = new();
    private static readonly HotelView _view = new();

    public static void Main()
    {
        _hotel.InitializeRooms();

        bool running = true;

        while (running)
        {
            _view.Menu();

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid menu choice.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.Clear();
                    _hotel.DisplayRooms();
                    break;

                case 2:
                    Console.Clear();
                    Booking();
                    break;

                case 3:
                    Console.Clear();
                    CheckIn();
                    break;

                case 4:
                    Console.Clear();
                    RoomService();
                    break;

                case 5:
                    Console.Clear();
                    CheckOut();
                    break;

                case 6:
                    Console.Clear();
                    _hotel.DisplayReservations();
                    break;

                case 7:
                    Console.Clear();
                    Console.WriteLine(
                        $"Occupancy rate: {_hotel.GetRoomPercentage():N2}%");
                    break;

                case 8:
                    Console.Clear();
                    CancelBooking();
                    break;

                case 0:
                    running = false;
                    break;

                default:
                    Console.WriteLine("Menu not available.");
                    break;
            }
        }
    }

    private static void Booking()
    {
        Console.Write("Guest name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Phone number: ");
        string phone = Console.ReadLine() ?? "";

        Console.Write("Is member? (y/n): ");
        bool isMember = string.Equals(
            Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase);

        Console.Write("Room type (Standard/Deluxe/Suite): ");
        string roomType = Console.ReadLine() ?? "";

        Console.Write("Check-in date (dd-MM-yyyy): ");
        string? inputTanggal = Console.ReadLine();

        ValidateBooking(name, phone, roomType, inputTanggal);

        if (!DateTime.TryParseExact(
                inputTanggal,
                "dd-MM-yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime dateIn))
        {
            Console.WriteLine(
                "Invalid date format. Use dd-MM-yyyy. example: 15-10-2026.");
            return;
        }

        // Tanggal check-in tidak boleh sebelum hari ini.
        if (dateIn.Date < DateTime.Today)
        {
            Console.WriteLine(
                $"Check-in date cannot be before today. ({DateTime.Today:dd-MM-yyyy}).");
            return;
        }

        Console.Write("Length of stay (1-14): ");

        if (!int.TryParse(Console.ReadLine(), out int nights)
            || nights < 1 || nights > 14)
        {
            Console.WriteLine("Length of stay must be between 1 and 14 nights.");
            return;
        }

        try
        {
            Guest guest = new Guest(name, phone, isMember);
            _hotel.CreateReservation(dateIn, nights, guest, roomType);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void CheckIn()
    {
        Console.Write("Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid reservation ID.");
            return;
        }

        Reservation? reservation = _hotel.FindReservation(id);
        if (reservation == null)
        {
            Console.WriteLine("Reservation not found.");
            return;
        }

        if (reservation != null && reservation.CheckIn())
            Console.WriteLine("Check-in successful.");
        else
            Console.WriteLine(
                "Check-in failed. Check the ID, date, and reservation status.");
    }

    private static void RoomService()
    {
        Console.Write("Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid reservation ID.");
            return;
        }

        Reservation? reservation = _hotel.FindReservation(id);

        if (reservation == null)
        {
            Console.WriteLine("Reservation not found.");
            return;
        }

        _view.DisplayRoomServiceMenu();
        Console.Write("Choose service (1-3): ");

        if (!int.TryParse(Console.ReadLine(), out int serviceId))
        {
            Console.WriteLine("Invalid service selection.");
            return;
        }

        if (!reservation.AddRoomService(serviceId))
        {
            Console.WriteLine(
                "Order rejected. Check the reservation status, room type, and service selection.");
            return;
        }

        Console.WriteLine("Room service order recorded successfully.");
    }

    private static void CheckOut()
    {
        Console.Write("Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid reservation ID.");
            return;
        }

        Reservation? reservation = _hotel.FindReservation(id);

        if (reservation == null)
        {
            Console.WriteLine("Reservation not found.");
            return;
        }

        Invoice? invoice = reservation.CheckOut();

        if (invoice == null)
        {
            Console.WriteLine(
                "Check-out failed. The guest must be checked in.");
            return;
        }

        invoice.PrintDetails(reservation);

        while (true)
        {
            Console.Write("Payment amount: Rp");

            if (!decimal.TryParse(Console.ReadLine(), out decimal paidAmount) || paidAmount < 0)
            {
                Console.WriteLine("Invalid payment amount.");
                continue;
            }

            if (!invoice.Payment(paidAmount))
            {
                continue;
            }

            if (reservation.CompleteCheckOut())
            {
                Console.WriteLine("Check-out completed successfully.");
            }
            else
            {
                Console.WriteLine(
                    "Payment succeeded, but check-out status could not be updated.");
            }

            break;
        }
    }
    private static void CancelBooking()
    {
        Console.Write("Reservation ID: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid reservation ID.");
            return;
        }

        Reservation? reservation = _hotel.FindReservation(id);

        if (reservation == null)
        {
            Console.WriteLine("Reservation not found.");
            return;
        }

        if (reservation.CancelReserved())
        {
            Console.WriteLine("Booking cancelled successfully.");
        }
        else
        {
            Console.WriteLine(
                "Cancellation failed. Only reserved bookings can be cancelled.");
        }
    }

    private static void ValidateBooking(string name, string phone, string roomType, string inputTanggal)
    {
        if (name.IsWhiteSpace())
        {
            Console.WriteLine("Name cannot be Empty");
        }
        if (phone.IsWhiteSpace())
        {
            Console.WriteLine("Phone cannot be Empty");
        }
        if (roomType.IsWhiteSpace())
        {
            Console.WriteLine("Room Type cannot be Empty");
        }
        if (!string.IsNullOrEmpty(inputTanggal))
        {
            Console.WriteLine("Date cannot be Empty");
        }
    }
}