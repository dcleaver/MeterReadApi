using MeterReadApi.Models;

namespace MeterReadApi.Parsers
{
    public interface IMeterReadingParser
    {
        ParseResult Parse(Stream contents);
    }
}