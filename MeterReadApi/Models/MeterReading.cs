namespace MeterReadApi.Models
{

    public class MeterReading
    {
        public int MeterReadingId { get; set; }

        public int AccountId { get; set; }

        public DateTimeOffset MeterReadingDateTime { get; set; }

        public int MeterReadValue { get; set; }
    }
}