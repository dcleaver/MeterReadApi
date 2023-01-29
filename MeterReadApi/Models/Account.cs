namespace MeterReadApi.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        public string FirstName { get; set; } 

        public string LastName { get; set; }

        public List<MeterReading> MeterReadings { get; } = new();
    }
}