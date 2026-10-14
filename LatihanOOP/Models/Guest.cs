namespace LatihanOOP.Models;

public class Guest
{
    public string Name { get; }
    public string Phone { get; }
    private bool IsMember { get; }

    public Guest(string name, string phone, bool isMember)
    {
        Name = name;
        Phone = phone;
        IsMember = isMember;
    }

    public bool GetStatus()
    {
        return IsMember;
    }
}