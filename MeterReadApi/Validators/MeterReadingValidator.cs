using MeterReadApi.Models;
using MeterReadApi.Persistence;

namespace MeterReadApi.Validators
{
    public class MeterReadingValidator : IMeterReadingValidator
    {
        private readonly IMeterReadingRepository _repository;

        public MeterReadingValidator(IMeterReadingRepository repository)
        {
            _repository = repository;
        }

        public bool Validate(MeterReading reading)
        {
            if (!_repository.AccountExists(reading.AccountId))
            {
                return false;
            }

            if (reading.MeterReadValue < 0 || reading.MeterReadValue > 99999)
            {
                return false;
            }

            IEnumerable<MeterReading> previousReadings = _repository.GetReadings(reading.AccountId);
            if (previousReadings.Any(previous => previous.MeterReadingDateTime > reading.MeterReadingDateTime))
            {
                return false;
            }

            if (previousReadings.Any(previous => previous.MeterReadValue == reading.MeterReadValue && previous.MeterReadingDateTime == reading.MeterReadingDateTime))
            {
                return false;
            }

            return true;
        }
    }
}