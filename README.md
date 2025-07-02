# Brightness Control - Fixed Brightness Tool

A Windows system tray application that maintains a fixed screen brightness level, designed specifically for laptops with automatic brightness adjustment that cannot be disabled through standard Windows settings.

## Problem This Solves

Many modern laptops come with ambient light sensors that automatically adjust screen brightness based on surrounding lighting conditions. While this feature can be helpful, it often becomes problematic when:

- **Inconsistent lighting environments** cause constant brightness fluctuations
- **Gaming or media consumption** requires stable brightness levels
- **Professional work** demands consistent display conditions
- **Windows settings don't provide an option** to completely disable auto-brightness
- **OEM software is missing or ineffective** at controlling this behavior
- **Driver-level brightness control** overrides user preferences

This application solves these issues by continuously enforcing your preferred brightness level, effectively overriding any automatic adjustments made by the system.

## Features

✅ **System Tray Integration** - Runs quietly in the background without cluttering your desktop  
✅ **Continuous Brightness Enforcement** - Reapplies your chosen brightness every minute  
✅ **Easy Brightness Control** - Simple slider interface (0-100%)  
✅ **Persistent Settings** - Remembers your brightness preference across reboots  
✅ **Lightweight** - Minimal system resource usage  
✅ **Single Instance** - Prevents multiple copies from running simultaneously  
✅ **No External Dependencies** - Uses built-in Windows WMI for brightness control  

## How It Works

The application uses Windows Management Instrumentation (WMI) to directly communicate with your display's brightness controls, bypassing the automatic adjustment mechanisms. By reapplying your chosen brightness level every minute, it ensures that any automatic changes are quickly reverted to your preferred setting.

## Installation & Usage

### Prerequisites
- Windows 10/11
- .NET Framework 4.7.2 or later
- Administrator privileges (recommended for full functionality)

### Setup
1. Download and extract the application files
2. Run `BrightnessControl.exe`
3. The application will start minimized in your system tray
4. Look for the brightness icon (☀️) in your system tray

### Using the Application
1. **Double-click** the tray icon or **right-click → Show Settings**
2. Adjust the brightness slider to your desired level (0-100%)
3. Close the window - the app continues running in the background
4. Your brightness setting is automatically saved and will be restored on next startup

### System Tray Menu
- **Show Settings** - Opens the brightness control window
- **Exit** - Closes the application completely

## Configuration

The application stores your brightness preference in the Windows Registry at:
```
HKEY_CURRENT_USER\SOFTWARE\BrightnessControl
```

### Adding to Windows Startup (Optional)
To automatically start the application when Windows boots:
1. Press `Win + R`, type `shell:startup`, press Enter
2. Copy the `BrightnessControl.exe` file to this folder
3. The application will now start with Windows

## Troubleshooting

### Brightness Control Not Working
- **Run as Administrator** - Some systems require elevated privileges
- **Check Display Type** - External monitors may not support software brightness control
- **Update Display Drivers** - Ensure your graphics drivers are current

### Application Not Appearing in Tray
- Check if the application is running in Task Manager
- Ensure system tray icons are not hidden in Windows settings
- Try restarting the application

### Brightness Keeps Reverting
- The application reapplies brightness every minute - this is expected behavior
- If brightness changes immediately after adjustment, your laptop's automatic brightness is very aggressive
- Consider running the application as Administrator for better control

## Technical Details

- **Language**: C# .NET Framework
- **Brightness Control Method**: Windows WMI (`WmiMonitorBrightnessMethods`)
- **Update Frequency**: 60 seconds
- **Registry Storage**: `HKEY_CURRENT_USER\SOFTWARE\BrightnessControl`
- **Memory Usage**: ~5-10 MB typical

## Compatibility

### Supported Systems
- Windows 10 (all versions)
- Windows 11 (all versions)
- Most laptop displays with WMI brightness support

### Known Limitations
- External monitors may not respond to software brightness control
- Some gaming laptops with custom display drivers may require specific compatibility modes
- Very old laptops (pre-2010) may not support WMI brightness methods

## Why This Tool Is Necessary

Unlike desktop monitors with physical brightness controls, laptop displays rely on software/driver-level brightness adjustment. When manufacturers implement aggressive automatic brightness features at the hardware or driver level, standard Windows settings often cannot override this behavior. This tool provides a software-based solution that continuously enforces your preferred brightness level, giving you back control over your display.

---

**Need Help?** If you encounter issues, try running the application as Administrator first, as this resolves most compatibility problems with brightness control on modern laptops.