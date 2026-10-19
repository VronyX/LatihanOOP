namespace LatihanOOP2.Interfaces;

public interface IRoomService
{
    bool OrderRoomService(string menu, int price);
    int TotalRoomService();
    void PrintTotalRoomService();
}