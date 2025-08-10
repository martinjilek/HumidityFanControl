using System.Device.Gpio;
using System.Device.I2c;
using HumidityFanControl.Data;
using HumidityFanControl.Hardware;
using HumidityFanControl.Models;
using HumidityFanControl.Services;
using Microsoft.EntityFrameworkCore;

const byte HTU21D_ADDR = 0x40;

const int RELAY_GPIO = 17;
const int DELAY_MS = 2500;

//setup db, create if doesnt exist
using var context = new HfcContext();
context.Database.Migrate();

using var i2cDevice = I2cDevice.Create(new I2cConnectionSettings(1, HTU21D_ADDR));
using var gpio = new GpioController();

var sensor = new Htu21dSensorReader(i2cDevice);
var relay = new RelayController(gpio, RELAY_GPIO);
var logger = new DataLogger(context);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("\n🛑 Ctrl+C pressed. Exiting...");
    e.Cancel = true;
    cts.Cancel();
};

var service = new FanControlService(sensor, relay, logger, TimeSpan.FromMilliseconds(DELAY_MS), cts.Token);
await service.RunAsync();