using MeterReadApi.Models;

namespace MeterReadApi.Persistence
{
    public interface IMeterReadingRepository
    {
        void AddAccount(Account account);

        bool AccountExists(int accountId);

        IEnumerable<Account> GetAccounts();

        IEnumerable<MeterReading> GetReadings(int accountId);

        void AddMeterReading(MeterReading reading);

        void SaveChanges();
    }
}