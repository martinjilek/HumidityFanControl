using Microsoft.EntityFrameworkCore;

namespace HumidityFanControl.Data.Models;
public class HfcContext : DbContext
{
    public DbSet<SensorLog> Logs { get; set; }

    public HfcContext(DbContextOptions<HfcContext> options)
        : base(options)
    {
        
    }
}