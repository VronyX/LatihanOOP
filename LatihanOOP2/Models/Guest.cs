namespace LatihanOOP2.Models;

public class Guest
{
    public string GuestId { get; private set; }
    public string Name { get; set; }
    public bool Member { get; set; }

    public Guest(string guestId, string name)
    {
        GuestId = guestId;
        Name = name;
    }

    public void Info()
    {

    }
}
    