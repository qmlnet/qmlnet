SET BATCHDIR=%~dp0
if exist %BATCHDIR%build rd /q /s %BATCHDIR%build
if exist %BATCHDIR%output rd /q /s %BATCHDIR%output
mkdir %BATCHDIR%build
mkdir %BATCHDIR%output


cd %BATCHDIR%build
set PREFIX=%BATCHDIR%output
qmake ../QmlNet

jom
jom install