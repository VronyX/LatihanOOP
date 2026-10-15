using System;

public class Standard : Room
{
    public Standard(string roomNumber, int floor, int nightlyRate, string status)
        : base(roomNumber, floor, nightlyRate, status) { }

    public override int ExtraChargePerNight()
    {
        return 0;
    }

    public override void Info()
    {
        base.Info();
        Console.WriteLine("Type: Standard");
    }
}