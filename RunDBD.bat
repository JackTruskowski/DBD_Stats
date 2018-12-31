::Syncs log files and then runs the DBD application so that previous log files aren't overwritten
::before they can be saved

::TODO: Figure out a way to not have to run a separate launcher for the application
::@author Jack Truskowski

::Sync
call cpy.bat settings.ini

::Grab the path from the ini file
Set ExPath = ""
for /f "delims=" %%a in ('findstr /i "^ExecutablePath=" settings.ini') do set ExPath="%%a"
Set ExPath=%ExPath:~16,-1%

::Launch the game
start /d "%ExPath%" DeadByDaylight.url
