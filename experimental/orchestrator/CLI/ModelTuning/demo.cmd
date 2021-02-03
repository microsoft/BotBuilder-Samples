@echo off
@echo ORCHESTRATOR EVALUATION DEMO

@rem set SEED for different test sets
set SEED=common
set BLU=generated\%SEED%.blu 
set LUFILE=%SEED%.lu
@rem test file contains sample utterances that are not in main LU file.
set TESTFILE=%SEED%.test.lu
@rem proper test
@rem set TESTFILE=%SEED%.test.lu


set LUISKEY=
set LUISAPP=
set LUISHOST=

set SKIPLUIS=0
set QUERYRUN=0

if "%LUISKEY%" == "" (
  @echo Skipping comparison with LUIS. Fill in LUIS info to compare results.
  set SKIPLUIS=1
)

@rem set QUERY to run a single utterance test
set QUERY="what is the american declaration of independence?"

if "%1" == "qonly" (
  set QUERYRUN=1
  goto QUERYONLY
)

@rem model folder needs to be downloaded only once.
if "%1" == "getmodel" (
  if EXIST model rd /s /q model
)

@echo cleaning folders
if EXIST report (
rd /s /q report && md report
)
if EXIST generated (
  rd /s /q generated && md generated
)


@rem Only need to retrieve model once
IF NOT EXIST .\model (
@rem see bf orchestrator:basemodel:get --help to get the non-default model 
@rem see available models via bf orchestrator:basemodel:list
  @echo getting base model
  md model
  call bf orchestrator:basemodel:get  --out model
)
@echo Create orchestrator snapshot .blu file
call bf orchestrator:create --model model --out ./generated --in %LUFILE%

@echo running orchestrator test to generate a report (see report folder) 
call bf orchestrator:test --in %BLU% --model ./model  --out report --test %TESTFILE%

if "%SKIPLUIS%" == "0" (
  @echo running LUIS test...
  call bf luis:test --subscriptionKey %LUISKEY% --endpoint %LUISHOST% --appId %LUISAPP% --in %TESTFILE% --out report/luisresult.txt
)

:QUERYONLY
@rem Illustrates how to query for only a single utterance. Edit %QUERY% above.
if "%QUERYRUN%" == "1" (

  echo Orchestrator single utterance query:
  echo bf orchestrator:query --in %BLU% --model model --query %QUERY%
  call bf orchestrator:query --in %BLU% --model model --query %QUERY%


  if "%SKIPLUIS%" == "0" (
    echo LUIS single utterance query:
    echo bf luis:application:query --appId LUISAPP --endpoint LUISHOST --subscriptionKey LUISKEY --query %QUERY%
    call bf luis:application:query --appId %LUISAPP% --endpoint %LUISHOST% --subscriptionKey %LUISKEY% --query %QUERY%
  )
)

:DONE

