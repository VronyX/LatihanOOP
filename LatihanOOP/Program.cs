using LatihanOOP.Models;
using LatihanOOP.Services;
using System.ComponentModel.Design;

namespace LatihanOOP;

internal class Program
{
    static void Main(string[] args)
    {
        Guest guest = new Guest("Mas mas", "08123456789");

        while (true)
        {
            Menu();
        }
    }

    
}
