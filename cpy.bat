::A wee batch file to copy over unique log files for stats
::Takes the path to ini file as a command line arg
::Author: Jack Truskowski

SET mm=%date:~4,2%
SET dd=%date:~7,2%
SET yy=%date:~12,2%
SET hh=%time:~0,2%
SET min=%time:~3,2%
SET ss=%time:~6,2%
::SET datapath=C:\Users\trusk\OneDrive\Desktop\DbdStats\data\
::SET logfilepath=C:\Users\trusk\AppData\Local\DeadByDaylight\Saved\Logs\DeadByDaylight.log

::Read from ini
Set datapath = ""
for /f "delims=" %%a in ('findstr /i "^DataPath=" %1') do set datapath="%%a"
Set datapath=%datapath:~10,-1%

Set logfilepath = ""
for /f "delims=" %%a in ('findstr /i "^LogPath=" %1') do set logfilepath="%%a"
Set logfilepath=%logfilepath:~9,-1%

echo %datapath%
echo %logfilepath%

::for each file in the destination directory, check if the log file contents match
FOR %%f in ("%datapath%"*.log) DO (
    C:\Windows\system32\cmd.exe /C fc /b "%%f" "%logfilepath%" > nul
    IF NOT errorlevel 1 (
      ::this file already exists in target directory, exit without copying
      echo file already exists
      EXIT /b
    )	
)

::unique file, copy it over
echo unique file
C:\Windows\system32\cmd.exe /C ECHO F|xcopy /i /S "%logfilepath%" "%datapath%"dbd_%mm%%dd%%yy%_%hh%%min%%ss%.log

