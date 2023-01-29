using MeterReadApi.Models;
using MeterReadApi.Persistence;
using MeterReadApi.Processors;
using MeterReadApi.Validators;
using NSubstitute;

namespace MeterReadApi.Tests
{
    public class MeterReadingProcessorTests
    {
        private MeterReadingProcessor _processor;

        private IMeterReadingRepository _repository;
        private IMeterReadingValidator _validator;

        public MeterReadingProcessorTests()
        {
            _repository = Substitute.For<IMeterReadingRepository>();
            _validator = Substitute.For<IMeterReadingValidator>();

            _processor = new MeterReadingProcessor(_repository, _validator);
        }

        [Fact]
        public void GivenNoReadings_WhenSubmitReadings_ThenZeroReturned()
        {
            SubmissionResult result = _processor.SubmitReadings(Array.Empty<MeterReading>());

            Assert.Equal(0, result.Successful);
            Assert.Equal(0, result.Failed);

            _repository.DidNotReceive().SaveChanges();
        }

        [Fact]
        public void GivenMixtureOfValidInvalidReadings_WhenSubmitReadings_ThenCorrectResultReturned()
        {
            MeterReading reading1 = AddNewReading(true);
            MeterReading reading2 = AddNewReading(true);
            MeterReading reading3 = AddNewReading(false);
            MeterReading reading4 = AddNewReading(true);

            SubmissionResult result = _processor.SubmitReadings(new[] { reading1, reading2, reading3, reading4 });

            Assert.Equal(3, result.Successful);
            Assert.Equal(1, result.Failed);

            _repository.Received(1).SaveChanges();
        }

        [Fact]
        public void GivenMixtureOfValidInvalidReadings_WhenSubmitReadings_ThenOnlyValidReadingsSubmitted()
        {
            MeterReading reading1 = AddNewReading(true);
            MeterReading reading2 = AddNewReading(true);
            MeterReading reading3 = AddNewReading(false);
            MeterReading reading4 = AddNewReading(true);

            _processor.SubmitReadings(new[] { reading1, reading2, reading3, reading4 });

            _repository.Received().AddMeterReading(reading1);
            _repository.Received().AddMeterReading(reading2);
            _repository.Received().AddMeterReading(reading4);

            _repository.DidNotReceive().AddMeterReading(reading3);

            _repository.Received(1).SaveChanges();
        }

        [Fact]
        public void GivenNoValidReadings_WhenSubmitReadings_ThenChangesNotSaved()
        {
            MeterReading reading1 = AddNewReading(false);
            MeterReading reading2 = AddNewReading(false);
            MeterReading reading3 = AddNewReading(false);
            MeterReading reading4 = AddNewReading(false);

            _processor.SubmitReadings(new[] { reading1, reading2, reading3, reading4 });

            _repository.DidNotReceiveWithAnyArgs().AddMeterReading(null);
            _repository.DidNotReceive().SaveChanges();
        }

        private MeterReading AddNewReading(bool valid)
        {
            MeterReading reading = new();
            _validator.Validate(reading).Returns(valid);

            return reading;
        }
    }
}