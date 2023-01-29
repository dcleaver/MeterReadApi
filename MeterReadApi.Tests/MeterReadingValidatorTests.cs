using MeterReadApi.Models;
using MeterReadApi.Persistence;
using MeterReadApi.Validators;
using NSubstitute;

namespace MeterReadApi.Tests
{
    public class MeterReadingValidatorTests
    {
        private IMeterReadingRepository _repository;
        private MeterReadingValidator _validator;

        public MeterReadingValidatorTests()
        {
            _repository = Substitute.For<IMeterReadingRepository>();

            _validator = new MeterReadingValidator(_repository);

            _repository.AccountExists(1).Returns(false);
            _repository.AccountExists(2).Returns(true);
        }

        [Fact]
        public void GivenAccountDoesNotExist_WhenValidate_ThenInvalid()
        {
            Assert.False(_validator.Validate(CreateReading(1, DateTimeOffset.UtcNow, 2)));
        }

        [Fact]
        public void GivenAccountExists_WhenValidate_ThenValid()
        {
            Assert.True(_validator.Validate(CreateReading(2, DateTimeOffset.UtcNow, 2)));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-999)]
        [InlineData(100000)]
        [InlineData(9082948)]
        public void GivenInvalidReading_WhenValidate_ThenInvalid(int reading)
        {
            Assert.False(_validator.Validate(CreateReading(2, DateTimeOffset.UtcNow, reading)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99999)]
        [InlineData(7923)]
        public void GivenValidReading_WhenValidate_ThenValid(int reading)
        {
            Assert.True(_validator.Validate(CreateReading(2, DateTimeOffset.UtcNow, reading)));
        }

        [Fact]
        public void GivenOlderReading_WhenValidate_ThenInvalid()
        {
            _repository.AccountExists(3).Returns(true);
            List<MeterReading> previousReadings = new()
            {
                CreateReading(3, DateTimeOffset.UtcNow.AddDays(-4), 1000),
                CreateReading(3, DateTimeOffset.UtcNow.AddDays(-3), 2000),
                CreateReading(3, DateTimeOffset.UtcNow.AddDays(-1), 3000)
            };
            _repository.GetReadings(3).Returns(previousReadings);

            Assert.False(_validator.Validate(CreateReading(3, DateTimeOffset.UtcNow.AddDays(-2), 2500)));
        }

        [Fact]
        public void GivenNewerReading_WhenValidate_ThenValid()
        {
            _repository.AccountExists(3).Returns(true);
            List<MeterReading> previousReadings = new()
            {
                CreateReading(3, DateTimeOffset.UtcNow.AddDays(-4), 1000),
                CreateReading(3, DateTimeOffset.UtcNow.AddDays(-3), 2000),
                CreateReading(3, DateTimeOffset.UtcNow.AddDays(-1), 3000)
            };
            _repository.GetReadings(3).Returns(previousReadings);

            Assert.True(_validator.Validate(CreateReading(3, DateTimeOffset.UtcNow, 3500)));
        }

        [Fact]
        public void GivenReadingAlreadyExists_WhenValidate_ThenInvalid()
        {
            _repository.AccountExists(3).Returns(true);
            DateTimeOffset readingTime = DateTimeOffset.UtcNow;

            List<MeterReading> previousReadings = new()
            {
                CreateReading(3, readingTime, 1000)
            };
            _repository.GetReadings(3).Returns(previousReadings);

            Assert.False(_validator.Validate(CreateReading(3, readingTime, 1000)));
        }

        private static MeterReading CreateReading(int accountId, DateTimeOffset readingDateTime, int readValue)
        {
            return new()
            {
                AccountId = accountId,
                MeterReadingDateTime = readingDateTime,
                MeterReadValue = readValue
            };
        }
    }
}