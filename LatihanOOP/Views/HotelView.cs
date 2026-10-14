namespace LatihanOOP.Views;

public class HotelView
{
    public void Menu()
    {
        Console.WriteLine("========== HOTEL MANAGEMENT SYSTEM ==========");
        Console.WriteLine("1.Daftar Kamar");
        Console.WriteLine("2.Booking / Check -in");
        Console.WriteLine("3.Room Service");
        Console.WriteLine("4.Check -out");
        Console.WriteLine("5.Laporan Hunian");
        Console.WriteLine("0.Keluar");
        Console.WriteLine("=============================================");
        Console.Write("Pilih menu:");
    }

    public static void DisplayMenu()
    {
        Console.WriteLine("\n=== ROOM SERVICE ===");
        Console.WriteLine("1. Nasi Goreng  - Rp50.000");
        Console.WriteLine("2. Coca-Cola    - Rp15.000");
        Console.WriteLine("3. Ayam Penyet  - Rp35.000");
        Console.WriteLine("0. Selesai");
    }
}
