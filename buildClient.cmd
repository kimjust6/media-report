@ECHO OFF

REM Build the javascripts
CALL yarn --cwd MediaReport/client/ build
IF %errorlevel% NEQ 0 EXIT /B %errorlevel%

IF NOT EXIST MediaReport\ClientResources (md MediaReport\ClientResources)

copy MediaReport\client\dist\assets\index.js MediaReport\ClientResources\index.js
copy MediaReport\client\dist\assets\index.js.map MediaReport\ClientResources\index.js.map

EXIT /B %ERRORLEVEL%