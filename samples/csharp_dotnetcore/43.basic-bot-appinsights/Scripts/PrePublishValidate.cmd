 @echo off
 findstr /r \"\s*\" *.bot | findstr /C:"\"appId\"" > nul 2>&1 
 IF %errorlevel% EQU 0 echo Warning: appId is not specified in bot configuration.
 
 findstr /r \"\s*\" *.bot | findstr /C:"\"appPassword\"" > nul 2>&1 
 IF %errorlevel% EQU 0 echo Warning: appPassword not specified in bot configuration.
 
 findstr /r \"\s*\" appsettings.json | findstr /C:"\"botFilePath\"" > nul 2>&1 
 IF %errorlevel% EQU 0 goto NoBotFilePath

 findstr /r \"\s*\" appsettings.json | findstr /C:"\"botFileSecret\"" > nul 2>&1 
 IF %errorlevel% EQU 0 echo Warning: botFileSecret not specified in appsettings.json.
 
 findstr /r \"type\" *.bot | findstr /C:"\"luis\"" > nul 2>&1
 IF %errorlevel% EQU 1 GOTO BadLuis
 IF %errorlevel% EQU 0 GOTO End
 
 :NoBotFilePath
 echo ERROR: The "botFilePath" missing in appsettings.json. See the README.md for more information on how to use Msbot.
 exit /B 1

 :BadLuis
 echo ERROR: Luis not configured. See the README.md for more information on how to use Msbot.
 exit /B 3
 
 :End
 echo Bot configuration check complete.
 exit /B 0