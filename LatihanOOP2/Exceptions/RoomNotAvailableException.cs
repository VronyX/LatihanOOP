namespace LatihanOOP2.Exceptions;

public class RoomNotAvailableException : Exception
{
    public RoomNotAvailableException(string message)
        : base(message)
    {
    }
}