namespace LatihanOOP.Views;

public class HotelView
{
    public void Menu()
    {
        Console.WriteLine("\n========== HOTEL MANAGEMENT ==========");
        Console.WriteLine("1. Daftar kamar");
        Console.WriteLine("2. Booking");
        Console.WriteLine("3. Check-in");
        Console.WriteLine("4. Room service");
        Console.WriteLine("5. Check-out");
        Console.WriteLine("6. Daftar reservasi");
        Console.WriteLine("7. Laporan hunian");
        Console.WriteLine("0. Keluar");
        Console.Write("Pilih menu: ");
    }

    public void DisplayRoomServiceMenu()
    {
        Console.WriteLine("\n========== ROOM SERVICE ==========");
        Console.WriteLine("1. Nasi Goreng - Rp50.000");
        Console.WriteLine("2. Coca-Cola   - Rp15.000");
        Console.WriteLine("3. Ayam Penyet - Rp35.000");
    }
}