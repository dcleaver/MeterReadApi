using MeterReadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadApi.Persistence
{
    public class MeterReadingContext : DbContext
    {
        public MeterReadingContext(DbContextOptions<MeterReadingContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<MeterReading> MeterReadings { get; set; }
    }
}