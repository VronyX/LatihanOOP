namespace LatihanOOP2.Exceptions;

public class RoomServiceNotSupportedException : Exception
{
    public RoomServiceNotSupportedException(string message)
        : base(message)
    {
    }
}