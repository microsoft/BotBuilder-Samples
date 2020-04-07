@echo off
setlocal
if "%REPOS%" EQU "" goto help
if not exist "%1" goto help
if "%2" EQU "" goto help

set indir=%1
set outdir=%2

set dobuild=yes
if "%3" NEQ "" set dobuild=%3

echo Copying and publishing generated dialog and optionally building LUIS
echo Input: %indir% 
echo Output: %outdir%
echo lubuild: %dobuild%

pushd %indir%
if "%dobuild%" NEQ "yes" goto copy
rd /s %outdir%

:copy
xcopy /s * %outdir%

if "%dobuild%" NEQ "yes" goto pop
if "%LUIS_AUTHORING_KEY%" EQU "" goto help
cd %outdir%luis
FOR %%i IN (%outdir:~0,-1%) DO set botname=%%~nxi
bf luis:build --in . --authoringKey %LUIS_AUTHORING_KEY% --botName=%botname% --dialog --suffix %USERNAME% --out .

:pop
popd
goto done

:help
echo genCopy [input] [output] [lubuild=yes]
echo Copies input to output and publishes to LUIS if lubuild=yes
echo Environment variable REPOS should also be set to the root of your github repos
echo and LUIS_AUTHORING_KEY to your LUIS authoring key

:done