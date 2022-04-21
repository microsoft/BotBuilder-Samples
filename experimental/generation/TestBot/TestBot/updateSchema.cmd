@echo off
rem Update schema files if csproj has changed
if "%1" NEQ "" goto check
echo Usage: updateSchema PROJECT
exit /b

:check
fc /b %1 bin\%1 > nul 2> nul
if "%errorlevel%" EQU "0" goto unchanged
call bf dialog:merge TestBot.csproj
copy %1 bin\%1
exit /b

:unchanged
echo No schema changes to merge