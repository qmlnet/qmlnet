call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars64.bat"
set PATH=%PATH%;C:\Qt5120\Qt\5.12.0\msvc2017_64\bin
set PATH=%PATH%;C:\Qt5120\Qt\Tools\QtCreator\bin

set PATH=%PATH%;%APPVEYOR_BUILD_FOLDER%\src\native\output


dotnet run -p ./build/scripts/Build.csproj -- ci