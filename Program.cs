using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Threading;
using HumidityFanControl.Models;
using Microsoft.EntityFrameworkCore;


// setup sensor
const byte HTU21D_ADDR = 0x40;
const byte TRIGGER_TEMP_MEASURE_HOLD = 0xE3;
const byte TRIGGER_HUMD_MEASURE_HOLD = 0xE5;

// setup relay
const int RELAY_GPIO = 17;
const int DELAY_MS = 2500;
bool relayOn = false;

//setup db, create if doesnt exist
using var context = new HfcContext();
context.Database.Migrate();

// init i2c and gpio
var i2cSettings = new I2cConnectionSettings(1, HTU21D_ADDR);
using var i2cDevice = I2cDevice.Create(i2cSettings);
using var gpio = new GpioController();
gpio.OpenPin(RELAY_GPIO, PinMode.Output);

bool running = true;

Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("\n🛑 Ctrl+C pressed. Exiting...");
    e.Cancel = true;
    running = false;
};

while (running)
{
    double temp = ReadTemperature(i2cDevice);
    double hum = ReadHumidity(i2cDevice);

    Console.WriteLine($"🌡️ Temp: {temp:F2} °C | 💧 Humidity: {hum:F2} %");
    Console.WriteLine($"Setting GPIO {RELAY_GPIO} to {(hum > 60.0 ? "HIGH (ON)" : "LOW (OFF)")}");
    if (hum > 60.0)
    {
        gpio.Write(RELAY_GPIO, PinValue.High); // Relay ON
        relayOn = true;
    }
    else
    {
        gpio.Write(RELAY_GPIO, PinValue.Low); // Relay OFF
        relayOn = false;
    }

    context.Logs.Add(new()
    {
        FanStateOn = relayOn,
        DateCreated = DateTime.Now,
        Humidity = hum,
        Temperature = temp,
    });
    
    await context.SaveChangesAsync();
    Thread.Sleep(DELAY_MS);
}

CleanExit(gpio);


static double ReadTemperature(I2cDevice device)
{
    Span<byte> cmd = stackalloc byte[] { TRIGGER_TEMP_MEASURE_HOLD };
    Span<byte> data = stackalloc byte[3];

    device.Write(cmd);
    Thread.Sleep(50);
    device.Read(data);

    int raw = (data[0] << 8) | (data[1] & 0xFC);
    return -46.85 + 175.72 * raw / 65536.0;
}

static double ReadHumidity(I2cDevice device)
{
    Span<byte> cmd = stackalloc byte[] { TRIGGER_HUMD_MEASURE_HOLD };
    Span<byte> data = stackalloc byte[3];

    device.Write(cmd);
    Thread.Sleep(50);
    device.Read(data);

    int raw = (data[0] << 8) | (data[1] & 0xFC);
    return -6 + 125.0 * raw / 65536.0;
}

static void CleanExit(System.Device.Gpio.GpioController gpio)
{
    Console.WriteLine("✅ Clean exit.");
    gpio.Write(RELAY_GPIO, PinValue.Low); // Ensure relay is off
    gpio.ClosePin(RELAY_GPIO);
}