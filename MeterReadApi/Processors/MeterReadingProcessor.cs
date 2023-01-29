using MeterReadApi.Models;
using MeterReadApi.Persistence;
using MeterReadApi.Validators;

namespace MeterReadApi.Processors
{

    public class MeterReadingProcessor : IMeterReadingProcessor
    {
        private readonly IMeterReadingRepository _repository;
        private readonly IMeterReadingValidator _validator;

        public MeterReadingProcessor(IMeterReadingRepository repository, IMeterReadingValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public SubmissionResult SubmitReadings(IEnumerable<MeterReading> readings)
        {
            SubmissionResult result = new();

            foreach (var reading in readings)
            {
                if (_validator.Validate(reading))
                {
                    _repository.AddMeterReading(reading);
                    ++result.Successful;
                }
                else
                {
                    ++result.Failed;
                }
            }

            if (result.Successful > 0)
            {
                _repository.SaveChanges();
            }

            return result;
        }
    }
}