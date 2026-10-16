namespace LatihanOOP2.Models;

public class WaitlistEntry
{
    public Guest Guest { get; }
    public string RoomType { get; }
    public int Nights { get; }
    public int StartDay { get; }

    public WaitlistEntry(Guest guest, string roomType, int nights, int startDay)
    {
        Guest = guest;
        RoomType = roomType;
        Nights = nights;
        StartDay = startDay;
    }
}