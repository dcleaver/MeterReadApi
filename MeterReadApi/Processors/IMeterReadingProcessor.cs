using MeterReadApi.Models;

namespace MeterReadApi.Processors
{
    public interface IMeterReadingProcessor
    {
        SubmissionResult SubmitReadings(IEnumerable<MeterReading> readings);
    }
}