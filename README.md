# DBD Stats
An Windows desktop application to automatically track and display stats for Dead By Daylight written in Visual Studio.

*Disclaimer:  Use at your own risk. I take no responsibility if you get banned for using this application. That being said, only log files are used and they are copied over to another location for reading and manipulation, so it shouldn't be a problem.*

---

![screencap](https://github.com/JackTruskowski/DBD_Stats/blob/master/Assets/Screencap.PNG "Application interface")

##### Current Features:

- Automatic parsing and syncing of log files 
- General killer statistic display

##### Planned Features:
- Individual killer stats
- Survivor stats
- Perk stats
- Multiple pages of stats
- Better interface
- Improved detection of files so that editing .ini file is not required

Feel free to contribute! This is the first time I've ever worked in Visual Studio so I probably did a lot of stuff a bad way. Any pull requests are appreciated.

##### Setup + Usage

1. Download the ZIP of the project and unzip
2. In Steam, create a desktop shortcut for Dead By Daylight and rename it DeadByDaylight.url (no spaces)
3. Provide information in settings.ini

   For example, the executable path might look like `C:\Users\USERNAME\Desktop\`, the data path is the path to the `Data\` directory included in this project, and the log path may look something like `C:\Users\USERNAME\AppData\Local\DeadByDaylight\Saved\Logs\DeadByDaylight.log`

4. Open the project in Visual Studio and build it
5. Launch Dead by Daylight by double clicking `RunDBD.bat`. This will first synchronize any current log files and then launch the game so that unsaved log files don't get overwritten.