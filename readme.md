# Humidity fan control
This is program controls a fan based on humidity levels inside the room.

## Requirements
- Raspberry pi with GPIO pins
- Linux OS
- Humidity and temperature sensor GY-213V-HTU21D
- 3.3V Relay module
- A fan
- Internet connection (not required)

## Settings
- Rename the appsettings.json.dist to appsettings.json
    - Edit the settings to desired values
        - HumidityThreshold: Humidity level to turn on the fan
        - ReadIntervalMs: How often to check the humidity level
        - Sensor.I2cBusId & Sensor.I2cAddress: see https://www.raspberrypi.com/documentation/computers/raspberry-pi.html#i2c
        - WeatherData:
            - enable: Whether to fetch weather data from the internet (requires connection), using https://open-meteo.com/en/docs
            - lat: your current latitude
            - lon: your current longitude
        - Schedule: Set scheduled times when the fan is allowed to run, if other conditions are met
            - If nothing is specified, the schedule is set to reasonable default (9 - 19h every day)
            - enable: Whether to enable schedules control (false => reasonable default schedule is used)
            - Default - default schedule of all days not specified
            - Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday - specific days
              - StartTime: Time since which the fan is allowed to run
              - EndTime: Time until which the fan is allowed to run
        - FanTimingSettings:
            - MinOnTimeMinutes: Minimum time the fan will run once turned on in minutes
            - MaxOnTimeMinutes: Maximum time the fan can run continuously in minutes
            - CooldownTimeMinutes: Time the fan must stay off before it can be turned on again in minutes