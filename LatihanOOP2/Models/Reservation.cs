namespace LatihanOOP2.Models;

public class Reservation
{
    public Guest Guest {  get; private set; }
    public Room Room { get; private set; }
    public int Nights
    {
        get; 
        set
        {
            if(value < 1 || value > 14)
            {
                Console.WriteLine("");
            }

            field = value;
        }
    }
    public int StartDay { get; set; }
    public string Status { get; private set; }

    public Reservation(Guest guest, Room room, int nights, int startDay)
    {
        Guest = guest;
        Room = room;
        Nights = nights;
        StartDay = startDay;
        Status = "Reserved";
    }

    public bool CheckIn()
    {
        if(Room.Status != "Reserved")
        {
            Console.WriteLine("Tidak bisa check in kalo statusnya bukan reserved");
            return false;
        }

        if (Room.Status == "Occupied")
        {
            Console.WriteLine("Tidak bisa check in ke kamar yang sudah diisi");
            return false;
        }

        Room.Occupy();

        Status = "CheckIn";

        return true;
    }

    public int CheckOut()
    {
        if (Status != "CheckedIn")
        {
            return 0;
        }

        int total = Room.CalculateTotal(Nights, StartDay);

        if (Guest.Member)
        {
            total = total * 90 / 100;
        }

        Room.Vacate();

        Status = "CheckedOut";

        return total;
    }

    public void PrintBill()
    {
        if (Status != "CheckedIn" && Status != "CheckedOut")
        {
            Console.WriteLine("Belum bisa mencetak tagihan.");
            return;
        }

        int roomTotal = Room.CalculateTotal(Nights, StartDay);

        Console.WriteLine("========== BILL ==========");
        Console.WriteLine($"Guest       : {Guest.Name}");
        Console.WriteLine($"Room        : {Room.RoomNumber}");
        Console.WriteLine($"Nights      : {Nights}");
        Console.WriteLine($"Room Charge : {roomTotal}");

        int discount = 0;

        if (Guest.Member)
        {
            discount = roomTotal * 10 / 100;
        }

        int total = roomTotal - discount;

        Console.WriteLine($"Member Disc : {discount}");
        Console.WriteLine($"Total       : {total}");
        Console.WriteLine("==========================");
    }
}
