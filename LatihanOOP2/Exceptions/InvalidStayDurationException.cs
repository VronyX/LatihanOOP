namespace LatihanOOP2.Exceptions;

public class InvalidStayDurationException : Exception
{
    public InvalidStayDurationException(string message)
        : base(message)
    {
    }
}