using CsvHelper;
using MeterReadApi.Models;
using System.Globalization;

namespace MeterReadApi.Persistence
{
    public class CsvAccountsSeeder : IAccountsSeeder
    {
        private readonly IMeterReadingRepository _repository;

        public CsvAccountsSeeder(IMeterReadingRepository repository)
        {
            _repository = repository;
        }

        public void SeedAccounts(Stream contents)
        {
            using StreamReader reader = new StreamReader(contents);
            using IReader csvReader = new CsvReader(reader, CultureInfo.GetCultureInfo("en-GB"));

            if (!csvReader.Read())
            {
                return;
            }

            if (!csvReader.ReadHeader())
            {
                return;
            }

            bool pendingChanges = false;
            while (csvReader.Read())
            {
                try
                {
                    Account account = csvReader.GetRecord<Account>()!;
                    _repository.AddAccount(account);
                    pendingChanges = true;
                }
                catch (Exception)
                {
                }
            };

            if (pendingChanges)
            {
                _repository.SaveChanges();
            }
        }
    }
}