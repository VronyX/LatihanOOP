namespace LatihanOOP.Views;

public class HotelView
{
    public void Menu()
    {
        Console.WriteLine("\n========== HOTEL MANAGEMENT ==========");
        Console.WriteLine("1. Room List");
        Console.WriteLine("2. Booking");
        Console.WriteLine("3. Check-in");
        Console.WriteLine("4. Room Service");
        Console.WriteLine("5. Check-out");
        Console.WriteLine("6. Reserved List");
        Console.WriteLine("7. Occupancy Rate");
        Console.WriteLine("8. Cancel Booking");
        Console.WriteLine("0. Exit");
        Console.Write("Choose Menu: ");
    }

    public void DisplayRoomServiceMenu()
    {
        Console.WriteLine("\n========== ROOM SERVICE ==========");
        Console.WriteLine("1. Nasi Goreng - Rp50.000");
        Console.WriteLine("2. Coca-Cola   - Rp15.000");
        Console.WriteLine("3. Ayam Penyet - Rp35.000");
    }
}