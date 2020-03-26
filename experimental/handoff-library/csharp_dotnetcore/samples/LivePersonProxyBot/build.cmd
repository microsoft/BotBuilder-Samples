@echo off
setlocal

set DEPLOYMENT_SOURCE=
set DEPLOYMENT_TARGET=%~dp0%.

copy %~dp0wwwroot\default.htm  %~dp0app_offline.htm

if exist ..\wwwroot\deploy.cmd (
  pushd ..\wwwroot
  call deploy.cmd
  popd
)

del %~dp0%\app_offline.htm

endlocal
