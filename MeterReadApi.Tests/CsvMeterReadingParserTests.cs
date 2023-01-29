using MeterReadApi.Models;
using MeterReadApi.Parsers;
using System.Text;

namespace MeterReadApi.Tests
{
    public class CsvMeterReadingParserTests
    {
        private CsvMeterReadingParser _parser;

        private const string CsvHeader = "AccountId,MeterReadingDateTime,MeterReadValue,";
        private const string ExampleReading1 = "1234,01/05/2020 16:42,56179,";
        private const string ExampleReading2 = "56789,01/01/2021 06:25,234,";
        private const string ExampleReading3 = "65432,28/09/2022 12:12,3721,";

        public CsvMeterReadingParserTests()
        {
            _parser = new CsvMeterReadingParser();
        }

        [Theory]
        [InlineData("")]
        [InlineData(CsvHeader)]
        public void GivenEmptyContents_WhenParse_ThenZeroReadingsReturned(string contents)
        {
            ParseResult parseResult = _parser.Parse(new MemoryStream(Encoding.ASCII.GetBytes(contents)));

            Assert.Empty(parseResult.MeterReadings);
            Assert.Equal(0, parseResult.InvalidCount);
        }

        [Fact]
        public void GivenSingleEntry_WhenParse_ThenEntryReturned()
        {
            string contents = $"{CsvHeader}{Environment.NewLine}{ExampleReading1}";

            ParseResult result = _parser.Parse(new MemoryStream(Encoding.ASCII.GetBytes(contents)));
            IList<MeterReading> readings = result.MeterReadings;

            Assert.Single(readings);
            CheckReading(readings.First(), 1234, DateTimeOffset.Parse("01/05/2020 16:42"), 56179);
        }

        [Fact]
        public void GivenMultipleEntries_WhenParse_ThenEntriesReturned()
        {
            string contents = $"{CsvHeader}{Environment.NewLine}" +
                $"{ExampleReading1}{Environment.NewLine}" +
                $"{ExampleReading2}{Environment.NewLine}" +
                $"{ExampleReading3}";

            ParseResult result = _parser.Parse(new MemoryStream(Encoding.ASCII.GetBytes(contents)));

            IList<MeterReading> readings = result.MeterReadings;
            Assert.Equal(3, readings.Count());
            Assert.Equal(0, result.InvalidCount);

            CheckReading(readings[0], 1234, DateTimeOffset.Parse("01/05/2020 16:42"), 56179);
            CheckReading(readings[1], 56789, DateTimeOffset.Parse("01/01/2021 06:25"), 234);
            CheckReading(readings[2], 65432, DateTimeOffset.Parse("28/09/2022 12:12"), 3721);
        }

        [Fact]
        public void GivenUnparsableEntry_WhenParse_ThenOnlyValidEntriesReturned()
        {
            string contents = $"{CsvHeader}{Environment.NewLine}" +
                $"{ExampleReading1}{Environment.NewLine}" +
                $"65432,76/26/2022 12:12,asdf,{ Environment.NewLine}" +
                $"{ExampleReading3}";

            ParseResult result = _parser.Parse(new MemoryStream(Encoding.ASCII.GetBytes(contents)));

            IList<MeterReading> readings = result.MeterReadings;
            Assert.Equal(2, readings.Count());
            Assert.Equal(1, result.InvalidCount);

            CheckReading(readings[0], 1234, DateTimeOffset.Parse("01/05/2020 16:42"), 56179);
            CheckReading(readings[1], 65432, DateTimeOffset.Parse("28/09/2022 12:12"), 3721);
        }

        private static void CheckReading(MeterReading actualReading, int expectedAccountId, DateTimeOffset expectedReadDate, int expectedReading)
        {
            Assert.Equal(expectedAccountId, actualReading.AccountId);
            Assert.Equal(expectedReadDate, actualReading.MeterReadingDateTime);
            Assert.Equal(expectedReading, actualReading.MeterReadValue);
        }
    }
}