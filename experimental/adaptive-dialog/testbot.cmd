@echo off
if "%1" EQU "" goto simple
dotnet %~dp0TestBot\Microsoft.Bot.Builder.TestBot.Json.dll --root %~f1
goto done

:simple
dotnet %~dp0TestBot\Microsoft.Bot.Builder.TestBot.Json.dll

:done
