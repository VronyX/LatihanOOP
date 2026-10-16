namespace LatihanOOP2.Models;

public class Guest
{
    public string GuestId { get; }
    public string Name { get; }
    public bool Member { get; }

    public Guest(string guestId, string name, bool member)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nama tamu tidak boleh kosong.");

        GuestId = guestId;
        Name = name;
        Member = member;
    }

    public void Info()
    {
        Console.WriteLine($"ID Tamu : {GuestId}");
        Console.WriteLine($"Nama    : {Name}");
        Console.WriteLine($"Member  : {(Member ? "Ya" : "Tidak")}");
    }
}