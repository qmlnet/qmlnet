call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars64.bat"

SET BATCHDIR=%~dp0
set PATH=%PATH%;D:\Qt\Tools\QtCreator\bin
set PATH=%PATH%;D:\Qt\5.11.1\msvc2017_64\bin

if exist %BATCHDIR%\build rd /q /s %BATCHDIR%\build
if exist %BATCHDIR%\output rd /q /s %BATCHDIR%\output
mkdir %BATCHDIR%\build
mkdir %BATCHDIR%\output


cd %BATCHDIR%\build
set PREFIX=%BATCHDIR%\output
qmake ../QmlNet

jom
jom install