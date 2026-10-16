namespace LatihanOOP2.Exceptions;

public class InvalidStayDurationException : Exception
{
    public InvalidStayDurationException(int nights)
        : base($"Lama menginap harus 1 sampai 14 malam (diinput: {nights} malam).")
    { }
}
