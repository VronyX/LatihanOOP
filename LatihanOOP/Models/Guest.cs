namespace LatihanOOP.Models;

public class Guest
{
    public string Name { get; }
    public string Phone { get; }
    private bool IsMember { get; }

    public Guest(string name, string phone, bool isMember)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nama wajib diisi.");

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Nomor telepon wajib diisi.");

        Name = name;
        Phone = phone;
        IsMember = isMember;
    }

    public bool GetStatus() => IsMember;
}