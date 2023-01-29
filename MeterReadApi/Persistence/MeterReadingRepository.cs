using MeterReadApi.Models;

namespace MeterReadApi.Persistence
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly MeterReadingContext _context;

        public MeterReadingRepository(MeterReadingContext context)
        {
            _context = context;
        }

        public bool AccountExists(int accountId) => _context.Accounts.Any(account => account.AccountId == accountId);

        public void AddAccount(Account account) => _context.Accounts.Add(account);

        public IEnumerable<Account> GetAccounts() => _context.Accounts;

        public void AddMeterReading(MeterReading reading) => _context.MeterReadings.Add(reading);

        public IEnumerable<MeterReading> GetReadings(int accountId) => _context.MeterReadings.Where(reading => reading.AccountId == accountId);

        public void SaveChanges() => _context.SaveChanges();
    }
}