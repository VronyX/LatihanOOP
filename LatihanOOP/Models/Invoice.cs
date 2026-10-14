using LatihanOOP.Enums;
using LatihanOOP.Models.Rooms;

namespace LatihanOOP.Models;

public class Invoice
{
    private decimal roomCharge;
    private decimal additionalCharge;
    private decimal roomServiceCharge;
    private decimal subTotal;
    private decimal memberDiscount;
    private decimal total;
    private InvoiceStatus status;

    public Invoice(Reservation reservation)
    {
        roomCharge = CalculateRoomCharge(reservation);
        additionalCharge = CalculateAdditionalCharge(reservation);
        roomServiceCharge = reservation.GetRoomServiceTotal();

        subTotal = CalculateSubTotal();
        memberDiscount = CalculateDiscount(
            reservation.GetGuest().GetStatus()
        );
        total = CalculateTotal();

        status = InvoiceStatus.Pending;
    }

    public decimal CalculateRoomCharge(Reservation reservation)
    {
        decimal totalCharge = 0;

        DateTime dateIn = reservation.GetDateIn();
        DateTime dateOut = reservation.GetDateOut();
        int lengthOfStay = reservation.GetLengthOfStay(dateIn, dateOut);

        Room room = reservation.GetRoom();

        for (int i = 0; i < lengthOfStay; i++)
        {
            DateTime currentDate = dateIn.AddDays(i);
            totalCharge += room.GetNightlyPrice(currentDate);
        }

        return totalCharge;
    }

    private decimal CalculateAdditionalCharge(Reservation reservation)
    {
        Room room = reservation.GetRoom();
        decimal additionalChargePerNight = room.GetAdditionalChargePerNight();

        int lengthOfStay = reservation.GetLengthOfStay(reservation.GetDateIn(),reservation.GetDateOut());

        return additionalChargePerNight * lengthOfStay;
    }

    public decimal CalculateSubTotal()
    {
        return roomCharge + additionalCharge + roomServiceCharge;
    }

    public decimal CalculateDiscount(bool isMember)
    {
        if (!isMember)
        {
            return 0;
        }
        
        return subTotal * (decimal)0.10;
    }

    public decimal CalculateTotal()
    {
        return subTotal - memberDiscount;
    }

    public void PrintDetails()
    {
        Console.WriteLine("\n========== INVOICE ==========");
        Console.WriteLine($"Room Charge       : Rp{roomCharge:N0}");
        Console.WriteLine($"Additional Charge : Rp{additionalCharge:N0}");
        Console.WriteLine($"Room Service      : Rp{roomServiceCharge:N0}");
        Console.WriteLine($"Subtotal          : Rp{subTotal:N0}");
        Console.WriteLine($"Member Discount   : Rp{memberDiscount:N0}");
        Console.WriteLine("-----------------------------");
        Console.WriteLine($"Total             : Rp{total:N0}");
        Console.WriteLine($"Payment Status    : {status}");
        Console.WriteLine("=============================");
    }

    public void Payment(int paidAmount)
    {
        if (paidAmount <= 0 || paidAmount != total)
        {
            status = InvoiceStatus.Failed;

            Console.WriteLine("Pembayaran gagal. Masukkan nominal yang sesuai.");
            return;
        }

        status = InvoiceStatus.Paid;

        Console.WriteLine("Pembayaran berhasil!");
        Console.WriteLine($"Total dibayar: Rp{paidAmount:N0}");
    }
}
