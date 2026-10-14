namespace LatihanOOP.Views;

public class HotelView
{
    public void Menu()
    {
        Console.WriteLine("========== HOTEL MANAGEMENT SYSTEM ==========");
        Console.WriteLine("1.Booking");
        Console.WriteLine("2.Check -in");
        Console.WriteLine("3.Check -out");
        Console.WriteLine("4.Laporan Hunian");
        Console.WriteLine("5.Cek reservasi");
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
