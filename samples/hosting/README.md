# Hosting

## Purpose

Most of the time it is easier to have .NET serve the entry point of your application (```Program.cs```). This approach works consistently across platforms and IDEs.

However, sometimes you need great control over how Qt is initialize on start. Or, you have some C/C++ code that needs to be run before the applications stars. Or, you need to register custom ```QObject``` classes with QML.

Regardless, this approachs allows you to create a Qt project in tradtional way (with .qrc files, linked libs, etc).

## Benefits

* Traditional Qt/QML application with complete control over the generated main executable.
* No need for the ```Qml.Net.*Binaries``` packages since the native components needed for ```Qml.Net``` are linked in the main executable and passed to .NET via magic.

## Running the sample

Running the sample is straight forward. It is the best way to get the gist of how things are working (it's simple, IMO).

* Step 1: Build the .NET entrypoint application.
```
cd net
dotnet build
```
* Step 2: Open ```native/NativeHost.pro``` in Qt Creator and run it.

That's it.

## The interesting parts

Follow phases 1-9 across the following files:
* [native/main.cpp](native/main.cpp)
* [net/Program.cs](net/Program.cs)