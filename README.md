Ambiance 1.0.0
==============

Ambiance is a utility to control RGB LED peripherals through your Windows desktop.

## Supported Peripherals
- Devices supported by the [Logitech LED SDK](gaming.logitech.com/en-us/developers), such as the G910 keyboard

## System Requirements
- Microsoft Windows 10 (untested on Windows 8.1 and older)
- Microsoft .NET Framework v4.5.2 or newer
- Logitech Gaming Software (if using a Logitech RGB LED device, such as the G910 keyboard)

## Installation
1. Clone the project to your local machine
2. Build in Release mode using Visual Studio (developed using 2015)
3. Copy bin\Release\Ambaince.exe and lib\<arch>\LogitechLedEnginesWrapper.dll to the desired run location

## Running the Tool
1. Run Ambiance.exe, and ensure that a Logitech SDK version number is displayed.
2. Select the desired LED color by filling in the RGB values (0-255). The color preview box will update as the input changes.
3. The Set Keyboard button will change the lighting of the keyboard to the current RGB values.
  - Note: The Logitech SDK doesn't provide a mechanism for setting the G910's G-Keys or G logo, so these keys should be disabled if a single color is desired.
4. Checking the "Pick Color from Background Image" box will disable the RGB input boxes and use the Windows registry to pick the color. Uncheck the box to re-enable user-defined colors.
