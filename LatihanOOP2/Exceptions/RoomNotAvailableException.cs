namespace LatihanOOP2.Exceptions;

public class RoomNotAvailableException : Exception
{
    public RoomNotAvailableException(string roomNumber, string message)
        : base(message)
    { }
}
