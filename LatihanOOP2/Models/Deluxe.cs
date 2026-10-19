
using LatihanOOP2.Interfaces;
using System;
using System.Linq;

public class Deluxe : Room, IRoomService
{
    private int[] roomServicePrices = Array.Empty<int>();

    public Deluxe(string roomNumber, int floor, int nightlyRate, string status)
        : base(roomNumber, floor, nightlyRate, status)
    {
    }

    public override int ExtraChargePerNight()
    {
        return 100000;
    }

    public bool OrderRoomService(string menu, int price)
    {
        if (string.IsNullOrWhiteSpace(menu) || price <= 0)
        {
            return false;
        }

        roomServicePrices = roomServicePrices
            .Append(price)
            .ToArray();

        return true;
    }

    public int TotalRoomService()
    {
        return roomServicePrices.Sum();
    }

    public void PrintTotalRoomService()
    {
        Console.WriteLine($"Room {RoomNumber} | Total Room Service: {TotalRoomService():N0}");
    }

    public override void Info()
    {
        base.Info();
        Console.WriteLine($"Type: Deluxe, Room Service Total: {TotalRoomService():N0}");
    }

    public override void Vacate()
    {
        base.Vacate();
        roomServicePrices = Array.Empty<int>();
    }
}