@echo off
setlocal


echo record deployment timestamp
date /t >> ..\deployment.log
time /t >> ..\deployment.log
echo ---------------------- >> ..\deployment.log
echo Deployment done

