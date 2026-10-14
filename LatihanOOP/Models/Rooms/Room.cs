namespace LatihanOOP.Models.Rooms;

public abstract class Room
{
    private int _no;
    private int _floor;
    private decimal _price;
    private bool _isBooked;

    public int No
    {
        get => _no;
        private set => _no = value;
    }

    public int Floor
    {
        get => _floor;
        private set => _floor = value;
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Nightly rate must be more than 0.");

            _price = value;
        }
    }

    public bool IsBooked
    {
        get => _isBooked;
        private set => _isBooked = value;
    }

    protected Room(int no, int floor, decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Tarif per malam harus lebih dari 0");

        _no = no;
        _floor = floor;
        _price = price;
        _isBooked = false;
    }

    // Mengubah status kamar menjadi terisi
    public void SetIsBooked(bool isCheckIn)
    {
        if (isCheckIn)
            IsBooked = true;
        else
        {
            IsBooked = false;
        }
    }

    // Mengecek status kamar
    public virtual bool GetStatus()
    {
        return IsBooked;
    }

    // Harga akhir pekan naik 20%
    public virtual decimal GetNightlyPrice(DateTime date)
    {
        decimal nightlyPrice = Price;

        // Cek apakah hari Jumat atau Sabtu
        if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
        {
            nightlyPrice += nightlyPrice * 0.20m;
        }

        return nightlyPrice;
    }

    public abstract bool SupportRoomService();

    public abstract decimal GetAdditionalChargePerNight();

    public virtual void AddRoomService(string description, decimal amount)
    {
        if (!SupportRoomService())
        {
            throw new InvalidOperationException(
                "Kamar ini tidak mendukung room service.");
        }
    }
}