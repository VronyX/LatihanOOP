using LatihanOOP.Enums;

namespace LatihanOOP.Models;

public class Invoice
{
    private decimal _roomCharge;
    private decimal _additionalCharge;
    private decimal _roomServiceCharge;
    private decimal _subTotal;
    private decimal _memberDiscount;
    private decimal _total;

    public InvoiceStatus Status { get; private set; }
    public decimal Total => _total;

    public Invoice(Reservation reservation)
    {
        CalculateRoomCharge(reservation);

        _additionalCharge =
            reservation.Room.GetAdditionalChargePerWeek()
            * reservation.GetLengthOfStay(
                reservation.DateIn, reservation.DateOut);

        _roomServiceCharge = reservation.RoomServiceTotal;

        CalculateSubTotal();
        CalculateDiscount(reservation.Guest.GetStatus());
        CalculateTotal();

        Status = InvoiceStatus.Pending;
    }

    public decimal CalculateRoomCharge(Reservation reservation)
    {
        _roomCharge = 0m;

        int nights = reservation.GetLengthOfStay(
            reservation.DateIn, reservation.DateOut);

        for (int i = 0; i < nights; i++)
        {
            DateTime date = reservation.DateIn.AddDays(i);

            // Harga kamar sudah memperhitungkan akhir pekan.
            _roomCharge += reservation.Room.GetNightlyPrice(date);
        }

        return _roomCharge;
    }

    public decimal CalculateSubTotal()
    {
        _subTotal = _roomCharge
                  + _additionalCharge
                  + _roomServiceCharge;

        return _subTotal;
    }

    public decimal CalculateDiscount(bool isMember)
    {
        _memberDiscount = isMember ? _subTotal * 0.10m : 0m;
        return _memberDiscount;
    }

    public decimal CalculateTotal()
    {
        _total = _subTotal - _memberDiscount;
        return _total;
    }

    public void PrintDetails(Reservation reservation)
    {
        Console.WriteLine("\n========== INVOICE ==========");
        Console.WriteLine($"ID Reservasi   : {reservation.ID}");
        Console.WriteLine($"Nama Tamu      : {reservation.Guest.Name}");
        Console.WriteLine($"Nomor Kamar    : {reservation.Room.No}");
        Console.WriteLine($"Check-in       : {reservation.DateIn:dd-MM-yyyy}");
        Console.WriteLine($"Check-out      : {reservation.DateOut:dd-MM-yyyy}");
        Console.WriteLine($"Tarif Kamar    : Rp{_roomCharge:N0}");
        Console.WriteLine($"Biaya Tambahan : Rp{_additionalCharge:N0}");
        Console.WriteLine($"Room Service   : Rp{_roomServiceCharge:N0}");

        foreach (var order in reservation.Orders)
            Console.WriteLine($"- {order.Name}: Rp{order.Amount:N0}");

        Console.WriteLine($"Subtotal       : Rp{_subTotal:N0}");
        Console.WriteLine($"Diskon Member  : Rp{_memberDiscount:N0}");
        Console.WriteLine($"Total          : Rp{_total:N0}");
        Console.WriteLine($"Status         : {Status}");
    }

    public void Payment(decimal paidAmount)
    {
        if (Status != InvoiceStatus.Pending || paidAmount < _total)
        {
            Status = InvoiceStatus.Failed;
            Console.WriteLine("Pembayaran gagal atau nominal kurang.");
            return;
        }

        Status = InvoiceStatus.Paid;

        Console.WriteLine("Pembayaran berhasil.");
        Console.WriteLine($"Kembalian: Rp{paidAmount - _total:N0}");
    }
}