using LatihanOOP.Enums;

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
        memberDiscount = CalculateDiscount(reservation.GetGuest().GetStatus());
        total = CalculateTotal();
        status = InvoiceStatus.Pending;
    }

    private decimal CalculateRoomCharge(Reservation reservation)
    {
        decimal totalCharge = 0;

        int lengthOfStay = reservation.GetLengthOfStay(reservation.GetDateIn(), reservation.GetDateOut());

        for (int i = 0; i < lengthOfStay; i++)
        {
            DateTime currentDate = reservation.GetDateIn().AddDays(i);

            totalCharge += reservation.GetRoom().GetNightlyPrice(currentDate);
        }

        return totalCharge;
    }

    private decimal CalculateAdditionalCharge(Reservation reservation)
    {
        decimal additionalChargePerNight = reservation.GetRoom().GetAdditionalChargePerNight();

        int lengthOfStay = reservation.GetLengthOfStay(reservation.GetDateIn(), reservation.GetDateOut());

        return additionalChargePerNight * lengthOfStay;
    }

    private decimal CalculateSubTotal()
    {
        return roomCharge + additionalCharge + roomServiceCharge;
    }

    private decimal CalculateDiscount(bool isMember)
    {
        if (!isMember)
        {
            return 0;
        }

        return subTotal * 0.10m;
    }

    private decimal CalculateTotal()
    {
        return subTotal - memberDiscount;
    }

    public void PrintDetails(Reservation reservation)
    {
        Console.WriteLine("\n========== INVOICE ==========");
        Console.WriteLine($"Reservation ID   : {reservation.GetId()}");
        Console.WriteLine($"Guest Name       : {reservation.GetGuest().Name}");
        Console.WriteLine($"Room             : {reservation.GetRoom().No}");
        Console.WriteLine($"Room Type        : {reservation.GetRoom().GetType().Name}");
        Console.WriteLine($"Check-in         : {reservation.GetDateIn():dd-MM-yyyy}");
        Console.WriteLine($"Check-out        : {reservation.GetDateOut():dd-MM-yyyy}");
        Console.WriteLine($"Room Charge      : Rp{roomCharge:N0}");
        Console.WriteLine($"Additional Charge: Rp{additionalCharge:N0}");
        Console.WriteLine($"Room Service     : Rp{roomServiceCharge:N0}");

        foreach (var order in reservation.GetOrders())
        { 
            Console.WriteLine($"- {order.Name}: Rp{order.Amount:N0}");
        }

        Console.WriteLine("-------------------------------");
        Console.WriteLine($"Subtotal       : Rp{subTotal:N0}");
        Console.WriteLine($"Diskon Member  : Rp{memberDiscount:N0}");
        Console.WriteLine("-------------------------------");
        Console.WriteLine($"Total          : Rp{total:N0}");
        Console.WriteLine($"Status         : {status}");
        Console.WriteLine("===============================");
    }

    public bool Payment(decimal paidAmount)
    {
        if (paidAmount < total)
        {
            status = InvoiceStatus.Failed;

            Console.WriteLine("Payment failed. The amount is insufficient.");

            Console.WriteLine($"Remaining amount: Rp{total - paidAmount:N0}");

            status = InvoiceStatus.Pending;

            return false;
        }

        status = InvoiceStatus.Paid;

        Console.WriteLine("Payment successful.");

        decimal change = paidAmount - total;

        Console.WriteLine($"Change: Rp{change:N0}");

        return true;
    }
}