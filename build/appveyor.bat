call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars64.bat"
set PATH=%PATH%;C:\Qt\5.11.2\msvc2017_64\bin
set PATH=%PATH%;C:\Qt\Tools\QtCreator\bin

set PATH=%PATH%;%APPVEYOR_BUILD_FOLDER%\src\native\output


dotnet run -p ./build/scripts/Build.csproj -- ci