using LatihanOOP2.Interfaces;
using System;

public class Deluxe : Room, IRoomService
{
    private int totalRoomService;

    public Deluxe(string roomNumber, int floor, int nightlyRate, string status)
        : base(roomNumber, floor, nightlyRate, status)
    {
        totalRoomService = 0;
    }

    public override int ExtraChargePerNight()
    {
        return 50000;
    }

    public bool OrderRoomService(string menu, int price)
    {
        if (price > 0)
        {
            totalRoomService += price;
            return true;
        }
        return false;
    }

    public int TotalRoomService()
    {
        return totalRoomService;
    }

    public override void Info()
    {
        base.Info();
        Console.WriteLine($"Type: Deluxe, Room Service Total: {totalRoomService}");
    }
}