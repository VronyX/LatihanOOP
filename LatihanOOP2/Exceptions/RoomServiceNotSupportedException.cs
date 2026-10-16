namespace LatihanOOP2.Exceptions;

public class RoomServiceNotSupportedException : Exception
{
    public RoomServiceNotSupportedException(string roomNumber)
        : base($"Kamar {roomNumber} (Standard) tidak menyediakan room service.")
    { }
}
