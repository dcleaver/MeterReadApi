using MeterReadApi.Parsers;
using MeterReadApi.Persistence;
using MeterReadApi.Processors;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MeterReadController : ControllerBase
    {
        private readonly IMeterReadingParser _meterReadingParser;
        private readonly IMeterReadingProcessor _processor;
        private readonly IAccountsSeeder _seeder;
        private readonly IMeterReadingRepository _repository;

        public MeterReadController(IMeterReadingParser meterReadingParser, IMeterReadingProcessor processor, IAccountsSeeder seeder, IMeterReadingRepository repository)
        {
            _meterReadingParser = meterReadingParser;
            _processor = processor;
            _seeder = seeder;
            _repository = repository;
        }

        [HttpPost("seed-database")]
        public IActionResult SeedDatabase(IFormFile file)
        {
            using Stream csvFile = file.OpenReadStream();
            _seeder.SeedAccounts(csvFile);

            return Ok();
        }

        [HttpPost("meter-reading-uploads")]
        public IActionResult GetEmployeeCSV(IFormFile file)
        {
            using Stream csvFile = file.OpenReadStream();
            ParseResult parseResult = _meterReadingParser.Parse(csvFile);
            SubmissionResult submitResult = _processor.SubmitReadings(parseResult.MeterReadings);

            submitResult.Failed += parseResult.InvalidCount;

            return Ok(submitResult);
        }

        [HttpGet("get-accounts")]
        public IActionResult GetAccounts()
        {
            return Ok(_repository.GetAccounts());
        }

        [HttpGet("get-readings")]
        public IActionResult GetReadings(int accountId)
        {
            return Ok(_repository.GetReadings(accountId));
        }
    }
}
