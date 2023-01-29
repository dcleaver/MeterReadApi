using CsvHelper;
using MeterReadApi.Models;
using System.Globalization;

namespace MeterReadApi.Parsers
{

    public class CsvMeterReadingParser : IMeterReadingParser
    {
        private class CsvMeterReading
        {
            public string AccountId { get; set; }

            public string MeterReadingDateTime { get; set; }

            public string MeterReadValue { get; set; }
        }

        public ParseResult Parse(Stream contents)
        {
            using StreamReader reader = new StreamReader(contents);
            using IReader csvReader = new CsvReader(reader, CultureInfo.GetCultureInfo("en-GB"));

            ParseResult result = new();

            if (!csvReader.Read())
            {
                return result;
            }

            if (!csvReader.ReadHeader())
            {
                return result;
            }

            while (csvReader.Read())
            {
                try
                {
                    CsvMeterReading reading = csvReader.GetRecord<CsvMeterReading>()!;

                    MeterReading meterReading = new()
                    {
                        AccountId = int.Parse(reading.AccountId),
                        MeterReadingDateTime = DateTimeOffset.Parse(reading.MeterReadingDateTime),
                        MeterReadValue = int.Parse(reading.MeterReadValue)
                    };

                    result.MeterReadings.Add(meterReading);
                }
                catch (Exception)
                {
                    ++result.InvalidCount;
                }
            };

            return result;
        }
    }
}