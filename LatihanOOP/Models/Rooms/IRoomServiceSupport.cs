namespace LatihanOOP.Models.Rooms;

public interface IRoomServiceSupport
{
    decimal GetServicePrice(int serviceId);
    string GetServiceName(int serviceId);
}