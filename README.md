# Qml.Net 


A Qml integration with .NET

[![Qml.Net](https://img.shields.io/nuget/v/Qml.Net.svg?style=flat&label=Qml.Net)](http://www.nuget.org/packages/Qml.Net/)
[![Build status](https://travis-ci.com/qmlnet/qmlnet.svg?branch=develop)](https://travis-ci.com/qmlnet/qmlnet) [![Build status](https://ci.appveyor.com/api/projects/status/l0hh7ranqawj682y/branch/develop?svg=true)](https://ci.appveyor.com/project/pauldotknopf/qmlnet/branch/develop)
[![Gitter chat](https://img.shields.io/gitter/room/qmlnet/Lobby.svg?style=flat)](https://gitter.im/qmlnet/Lobby)

Supported platforms/runtimes:
* Runtimes:
  * .NET Framework
  * .NET Core
  * Mono
* Operating systems
  * Linux
  * OSX
  * Windows

# Getting started

```bash
dotnet add package Qml.Net
```

**Windows**

```bash
dotnet add package Qml.Net.WindowsBinaries
```

**OSX**

```bash
dotnet add package Qml.Net.OSXBinaries
```

**Linux**

```bash
dotnet add package Qml.Net.LinuxBinaries
```

Checkout the [examples](https://github.com/pauldotknopf/Qml.Net.Examples) for some inspiration.

# Quick overview

**Define a .NET type (POCO)**

```c#
[Signal("customSignal", NetVariantType.String)] // You can define signals that Qml can listen to.
public class QmlType
{
    /// <summary>
    /// Properties are exposed to Qml.
    /// </summary>
    [NotifySignal("stringPropertyChanged")] // For Qml binding/MVVM.
    public string StringProperty { get; set; }

    /// <summary>
    /// Methods can return .NET types.
    /// The returned type can be invoked from Qml (properties/methods/events/etc).
    /// </summary>
    /// <returns></returns>
    public QmlType CreateNetObject()
    {
        return new QmlType();
    }

    /// <summary>
    /// Qml can pass .NET types to .NET methods.
    /// </summary>
    /// <param name="parameter"></param>
    public void TestMethod(QmlType parameter)
    {
    }

    /// <summary>
    /// Qml can also pass Qml/C++ objects that can be invoked from .NET
    /// </summary>
    /// <param name="qObject"></param>
    public void TestMethodWithQObject(dynamic qObject)
    {
        string result = qObject.PropertyDefinedInCpp;
        qObject.MethodDefinedInCpp(result);
    }
    
    /// <summary>
    /// Async methods can be invoked with continuations happening on Qt's main thread.
    /// </summary>
    public async Task<string> TestAsync()
    {
        // On the UI thread
        await Task.Run(() =>
        {
            // On the background thread
        });
        // On the UI thread
        return "async result!"
    }
    
    /// <summary>
    /// .NET can activate signals to send notifications to Qml.
    /// </summary>
    public void ActivateCustomSignal(string message)
    {
        this.ActivateSignal("customSignal", message)
    }
}
```

**Register your new type with Qml.**

```c#
using (var app = new QGuiApplication(args))
{
    using (var engine = new QQmlApplicationEngine())
    {
        // Register our new type to be used in Qml
        QQmlApplicationEngine.RegisterType<QmlType>("test", 1, 1);
        engine.Load("main.qml");
        return app.Exec();
    }
}
```

**Use the .NET type in Qml**

```js
import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0
import test 1.1

ApplicationWindow {
    visible: true
    width: 640
    height: 480
    title: qsTr("Hello World")

    QmlType {
      id: test
      Component.onCompleted: function() {
          // We can read/set properties
          console.log(test.stringProperty)
          test.stringPropertyChanged.connect(function() {
              console.log("The property was changed!")
          })
          test.stringProperty = "New value!"
          
          // We can return .NET types (even ones not registered with Qml)
          var netObject = test.createNetObject();
          
          // All properties/methods/signals can be invoked on "netObject"
          // We can also pass the .NET object back to .NET
          netObject.testMethod(netObject)
          
          // We can invoke async tasks that have continuation on the UI thread
          var task = netObject.testAsync()
          // And we can await the task
          Net.await(task, function(result) {
              // With the result!
              console.log(result)
          })
          
          // We can trigger signals from .NET
          test.customSignal.connect(function(message) {
              console.log("message: " + message)
          })
          test.activateCustomSignal("test message!")
      }
      function testHandler(message) {
          console.log("Message - " + message)
      }
    }
}
```

# Currently implemented

- [x] Support for all the basic Qml types and the back-and-forth between them (```DateTime```, ```string```, etc).
- [x] Reading/setting properties on .NET objects.
- [x] Invoking methods on .NET obejcts.
- [x] Declaring and activating signals on .NET objects.
- [x] ```async``` and ```await``` with support for awaiting and getting the result from Qml.
- [x] Passing dynamic javascript objects to .NET as ```dynamic```.
- [x] Custom V8 type that looks like an array, but wraps a .NET ```IList<T>``` instance, for modification of list in Qml, and performance.

# Not implemented (but planned)

- [ ] Compiling Qml resource files and bundling them within .NET.
- [ ] Passing ```QObject``` types to .NET with support for interacting with signals/slots/properties on them.
- [ ] .NET Events to signals
- [ ] General perf improvements (particularly with reflection).
- [ ] Qml debugger for VS and VS Code.
- [ ] Yocto meta-layer for .NET Core and Qml.Net (for embedded Linux development).
