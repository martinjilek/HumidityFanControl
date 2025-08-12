using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HumidityFanControl.Data.Models;
public class HfcContext : DbContext
{
    public DbSet<SensorLog> Logs { get; set; }

    public string DbPath { get; }

    public HfcContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "sqlite.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}