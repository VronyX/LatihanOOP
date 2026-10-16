using LatihanOOP2.Exceptions;

namespace LatihanOOP2.Models;

public class WaitingGuest
{
    public Guest Guest { get; private set; }
    public string RoomType { get; private set; }
    public int Nights { get; private set; }
    public int StartDay { get; private set; }

    public WaitingGuest(Guest guest, string roomType, int nights, int startDay)
    {
        if (nights < 1 || nights > 14)
        {
            throw new InvalidStayDurationException(nights);
        }

        Guest = guest;
        RoomType = roomType;
        Nights = nights;
        StartDay = startDay;
    }
}
