using LatihanOOP.Models;
using LatihanOOP.Services;
using System.ComponentModel.Design;

namespace LatihanOOP;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== SISTEM MANAJEMEN HOTEL ===");

        // 1. Inisialisasi Objek Hotel
        Hotel hotel = new Hotel();

        // 2. Memanggil metode InitializeRooms untuk mengisi array kamar
        // (Masing-masing 5 kamar untuk Standard, Deluxe, dan Suite)
        hotel.InitializeRooms();

        // 3. Menampilkan seluruh daftar kamar hotel beserta statusnya
        hotel.DisplayRooms();

        // 4. Menampilkan informasi total kamar dan persentase hunian awal
        Console.WriteLine($"\nTotal Kamar di Hotel : {hotel.TotalRooms}");
        Console.WriteLine($"Persentase Hunian Awal : {hotel.GetRoomPercentage():N2}%");

        // 5. Menguji pencarian kamar kosong (Contoh: Mencari tipe "Deluxe")
        Console.WriteLine("\n--- Pengujian Pencarian Kamar ---");
        DateTime checkIn = DateTime.Now;
        DateTime checkOut = checkIn.AddDays(3);

        Room availableRoom = hotel.FindAvailableRoom(checkIn, checkOut, "Deluxe");

        if (availableRoom != null)
        {
            Console.WriteLine($"Kamar ditemukan! No: {availableRoom.No} | Tipe: Deluxe | Tarif: Rp {availableRoom.Price:N0}");

            // Mensimulasikan pemesanan kamar (Check-in)
            availableRoom.IsBooked = true;
            Console.WriteLine($"-> Status kamar No. {availableRoom.No} berhasil diubah menjadi Terisi (Booked).");
        }
        else
        {
            Console.WriteLine("Maaf, kamar Deluxe tidak tersedia.");
        }

        // 6. Mengecek kembali persentase hunian setelah ada kamar yang terisi
        Console.WriteLine($"\nPersentase Hunian Terbaru: {hotel.GetRoomPercentage():N2}%");

        // 7. Menampilkan daftar kamar terbaru untuk melihat perubahan status
        hotel.DisplayRooms();
        Guest guest = new Guest("Mas mas", "08123456789");

        while (true)
        {
            Menu();
        }
    }
}