using LatihanOOP.Models.Rooms;

namespace LatihanOOP.Services;

public class Hotel
{
    // Menyimpan kamar menggunakan Array sesuai permintaan
    private Standard[] _standardRooms;
    private Deluxe[] _deluxeRooms;
    private Suite[] _suiteRooms;

    // Total keseluruhan kamar (5 Standard + 5 Deluxe + 5 Suite = 15 Kamar)
    public int TotalRooms => _standardRooms.Length + _deluxeRooms.Length + _suiteRooms.Length;

    public Hotel()
    {
        // Inisialisasi masing-masing array dengan kapasitas 5 kamar
        _standardRooms = new Standard[5];
        _deluxeRooms = new Deluxe[5];
        _suiteRooms = new Suite[5];
    }

    public void InitializeRooms()
    {
        // 1. Mengisi 5 Kamar Standard (Lantai 1, Nomor 101 - 105, Tarif Rp 300.000)
        for (int i = 0; i < 5; i++)
        {
            _standardRooms[i] = new Standard(101 + i, 1, 300000m);
        }

        // 2. Mengisi 5 Kamar Deluxe (Lantai 2, Nomor 201 - 205, Tarif Rp 600.000)
        for (int i = 0; i < 5; i++)
        {
            _deluxeRooms[i] = new Deluxe(201 + i, 2, 500000m);
        }

        // 3. Mengisi 5 Kamar Suite (Lantai 3, Nomor 301 - 305, Tarif Rp 1.200.000)
        for (int i = 0; i < 5; i++)
        {
            _suiteRooms[i] = new Suite(301 + i, 3, 800000m);
        }
    }

    public void DisplayRooms()
    {
        Console.WriteLine("\n=== DAFTAR SELURUH KAMAR HOTEL ===");

        // Menampilkan Kamar Standard
        Console.WriteLine("\n-- Kamar Standard --");
        foreach (var room in _standardRooms)
        {
            PrintRoomDetails(room);
        }

        // Menampilkan Kamar Deluxe
        Console.WriteLine("\n-- Kamar Deluxe --");
        foreach (var room in _deluxeRooms)
        {
            PrintRoomDetails(room);
        }

        // Menampilkan Kamar Suite
        Console.WriteLine("\n-- Kamar Suite --");
        foreach (var room in _suiteRooms)
        {
            PrintRoomDetails(room);
        }
    }

    private void PrintRoomDetails(Room room)
    {
        string type = room.GetType().Name;
        string status = room.IsBooked ? "Terisi (Occupied)" : "Kosong (Available)";
        Console.WriteLine($"No: {room.No} | Lantai: {room.Floor} | Tipe: {type,-8} | Tarif: Rp {room.Price:N0} | Status: {status}");
    }

    public Room FindAvailableRoom(DateTime dateIn, DateTime dateOut, string roomType)
    {
        // Cek pencarian di array Standard
        if (roomType.Equals("Standard", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var room in _standardRooms)
            {
                if (!room.IsBooked) return room;
            }
        }
        // Cek pencarian di array Deluxe
        else if (roomType.Equals("Deluxe", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var room in _deluxeRooms)
            {
                if (!room.IsBooked) return room;
            }
        }
        // Cek pencarian di array Suite
        else if (roomType.Equals("Suite", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var room in _suiteRooms)
            {
                if (!room.IsBooked) return room;
            }
        }

        return null;
    }

    public float GetRoomPercentage()
    {
        int total = TotalRooms;
        if (total == 0) return 0f;

        int bookedCount = 0;

        foreach (var room in _standardRooms) if (room.IsBooked) bookedCount++;
        foreach (var room in _deluxeRooms) if (room.IsBooked) bookedCount++;
        foreach (var room in _suiteRooms) if (room.IsBooked) bookedCount++;

        return ((float)bookedCount / total) * 100f;
    }

    // Menggabungkan seluruh array ke dalam satu List/Enumerable jika dibutuhkan oleh bagian lain
    public List<Room> GetRooms()
    {
        List<Room> allRooms = new List<Room>();
        allRooms.AddRange(_standardRooms);
        allRooms.AddRange(_deluxeRooms);
        allRooms.AddRange(_suiteRooms);
        return allRooms;
    }
}