using System.Device.Gpio;
using System.Device.I2c;
using HumidityFanControl.Config;
using HumidityFanControl.Data;
using HumidityFanControl.Hardware;
using HumidityFanControl.Models;
using HumidityFanControl.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// load configs
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var fanSettings = configuration.GetSection("FanControl").Get<FanControlSettings>();
var sensorSettings = configuration.GetSection("Sensor").Get<SensorSettings>();

//setup db, create if doesnt exist
using var context = new HfcContext();
context.Database.Migrate();

using var i2cDevice = I2cDevice.Create(new I2cConnectionSettings(sensorSettings.I2cBusId,sensorSettings.I2cAddress));
using var gpio = new GpioController();

var sensor = new Htu21dSensorReader(i2cDevice);
var relay = new RelayController(gpio, fanSettings.RelayGpioPin);
var logger = new DataLogger(context);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("\n🛑 Ctrl+C pressed. Exiting...");
    e.Cancel = true;
    cts.Cancel();
};

var service = new FanControlService(sensor, relay, logger, TimeSpan.FromMilliseconds(fanSettings.ReadIntervalMs),fanSettings.HumidityThreshold,cts.Token);
await service.RunAsync();