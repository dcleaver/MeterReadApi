using MeterReadApi.Models;

namespace MeterReadApi.Validators
{
    public interface IMeterReadingValidator
    {
        bool Validate(MeterReading reading);
    }
}