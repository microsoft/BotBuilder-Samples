@echo off
setlocal

set DEPLOYMENT_SOURCE=
set IN_PLACE_DEPLOYMENT=1

if exist ..\wwwroot\deploy.cmd (
  pushd ..\wwwroot
  call deploy.cmd
  popd
)

rem kick of build of csproj

echo record deployment timestamp
date /t >> ..\deployment.log
time /t >> ..\deployment.log
echo ---------------------- >> ..\deployment.log
echo Deployment done

endlocal


