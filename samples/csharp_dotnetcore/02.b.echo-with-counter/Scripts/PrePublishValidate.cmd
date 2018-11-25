 @echo off
findstr /r \"\s*\" *.bot | findstr /C:"\"appId\"" > nul 2>&1 
 IF %errorlevel% EQU 0 echo Warning: appId is not specified in bot configuration.
 
 findstr /r \"\s*\" *.bot | findstr /C:"\"appPassword\"" > nul 2>&1 
 IF %errorlevel% EQU 0 echo Warning: appPassword not specified in bot configuration.
  
 :End
 echo Bot configuration check complete.
 exit /B 0