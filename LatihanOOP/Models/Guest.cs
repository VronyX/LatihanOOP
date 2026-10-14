namespace LatihanOOP.Models;

public class Guest
{
    public string Name {  get; set; }
    public string Phone {  get; set; }
    private bool isMember;

    public Guest(string name, string phone)
    {
        Name = name;
        Phone = phone;
        isMember = true;
    }

    public bool GetStatus()
    {
        return isMember;
    }
}
