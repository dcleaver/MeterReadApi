using CsvHelper;
using MeterReadApi.Models;
using MeterReadApi.Parsers;
using MeterReadApi.Persistence;
using NSubstitute;
using System.Text;

namespace MeterReadApi.Tests
{
    public class CsvAccountsSeederTests
    {
        private CsvAccountsSeeder _seeder;

        private IMeterReadingRepository _repository;

        private const string CsvHeader = "AccountId,FirstName,LastName";

        private const string ExampleAccount1 = "1234,Joe,Bloggs";
        private const string ExampleAccount2 = "5678,Jane,Doe";
        private const string ExampleAccount3 = "987654,John,Smith";

        public CsvAccountsSeederTests()
        {
            _repository = Substitute.For<IMeterReadingRepository>();

            _seeder = new CsvAccountsSeeder(_repository);
        }

        [Theory]
        [InlineData("")]
        [InlineData(CsvHeader)]
        public void GivenEmptyContents_WhenSeedAccounts_ThenNothingSeeded(string contents)
        {
            _seeder.SeedAccounts(new MemoryStream(Encoding.ASCII.GetBytes(contents)));

            _repository.DidNotReceiveWithAnyArgs().AddAccount(null);
            _repository.DidNotReceive().SaveChanges();
        }

        [Fact]
        public void GivenSingleEntry_WhenSeedAccounts_ThenAccountAdded()
        {
            string contents = $"{CsvHeader}{Environment.NewLine}{ExampleAccount1}";

            _seeder.SeedAccounts(new MemoryStream(Encoding.ASCII.GetBytes(contents)));

            CheckAccountAdded(1234, "Joe", "Bloggs");
            _repository.Received(1).SaveChanges();
        }

        [Fact]
        public void GivenMultipleEntries_WhenSeedAccounts_ThenAccountsAdded()
        {
            string contents = $"{CsvHeader}{Environment.NewLine}" +
                $"{ExampleAccount1}{Environment.NewLine}" +
                $"{ExampleAccount2}{Environment.NewLine}" +
                $"{ExampleAccount3}";

            _seeder.SeedAccounts(new MemoryStream(Encoding.ASCII.GetBytes(contents)));

            CheckAccountAdded(1234, "Joe", "Bloggs");
            CheckAccountAdded(5678, "Jane", "Doe");
            CheckAccountAdded(987654, "John", "Smith");
            _repository.Received(1).SaveChanges();
        }

        [Fact]
        public void GivenUnparsableEntry_WhenSeedAccounts_ThenOnlyValidAccountsAdded()
        {
            string contents = $"{CsvHeader}{Environment.NewLine}" +
                $"{ExampleAccount1}{Environment.NewLine}" +
                $"qwerty,,{Environment.NewLine}" +
                $"{ExampleAccount3}";

            _seeder.SeedAccounts(new MemoryStream(Encoding.ASCII.GetBytes(contents)));

            CheckAccountAdded(1234, "Joe", "Bloggs");
            CheckAccountAdded(987654, "John", "Smith");
            _repository.Received(1).SaveChanges();
        }

        private void CheckAccountAdded(int accountId, string firstName, string lastName)
        {
            _repository.Received(1).AddAccount(Arg.Is<Account>(
                account =>
                    account.AccountId == accountId &&
                    account.FirstName == firstName &&
                    account.LastName == lastName));
        }
    }
}