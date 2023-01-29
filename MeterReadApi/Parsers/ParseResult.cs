using MeterReadApi.Models;

namespace MeterReadApi.Parsers
{
    public class ParseResult
    {
        public IList<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();

        public int InvalidCount { get; set; }
    }
}