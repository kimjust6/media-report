@ECHO OFF

REM Build the javascripts
CALL yarn --cwd MediaReport/client/ build
IF %errorlevel% NEQ 0 EXIT /B %errorlevel%

IF NOT EXIST MediaReport\ClientResources (md src\MediaReport\ClientResources)

copy MediaReport\client\dist\assets\index.js MediaReport\ClientResources
copy MediaReport\client\dist\assets\index.js.map MediaReport\ClientResources

EXIT /B %ERRORLEVEL%