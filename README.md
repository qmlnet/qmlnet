# Qml.Net 


A Qml integration with .NET

[![Qml.Net](https://img.shields.io/nuget/v/Qml.Net.svg?style=flat-square&label=Qml.Net)](http://www.nuget.org/packages/Qml.Net/)
[![Build status](https://travis-ci.com/pauldotknopf/Qml.Net.svg?branch=master)](https://travis-ci.com/pauldotknopf/Qml.Net) [![Build status](https://ci.appveyor.com/api/projects/status/0ob29turkjslh61j/branch/master?svg=true)](https://ci.appveyor.com/project/pauldotknopf/qml-net)

Supported platforms/runtimes:
* Runtimes:
  * .NET Framework
  * .NET Core
  * Mono
* Operating systems
  * Linux
  * OSX
  * Windows

## The idea

**Define a .NET type (POCO)**

```c#
[Signal("CustomSignal", NetVariantType.String)] // You can define signals that Qml can listen to.
public class QmlType
{
    /// <summary>
    /// Properties are exposed to Qml.
    /// </summary>
    [NotifySignal("StringPropertyChanged")] // For Qml binding/MVVM.
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
        this.ActivateSignal("CustomSignal", message)
    }
}
```

**Register your new type with Qml.**

```c#
using (var app = new QGuiApplication(r))
{
    using (var engine = new QQmlApplicationEngine())
    {
        // Register our new type to be used in Qml
        QQmlApplicationEngine.RegisterType<QmlType>("test", 1, 1);
        engine.loadFile("main.qml");
        return app.exec();
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
          console.log(test.StringProperty)
          test.StringPropertyChanged.connect(function() {
              console.log("The property was changed!")
          })
          test.StringProperty = "New value!"
          
          // We can return .NET types (even ones not registered with Qml)
          var netObject = test.CreateNetObject();
          
          // All properties/methods/signals can be invoked on "netObject"
          // We can also pass the .NET object back to .NET
          netObject.TestMethod(netObject)
          
          // We can invoke async tasks that have continuation on the UI thread
          var task = netObject.TestAsync()
          // And we can await the task
          Net.await(task, function(result) {
              // With the result!
              console.log(result)
          })
          
          // We can trigger signals from .NET
          test.CustomSignal.connect(function(message) {
              console.log("message: " + message)
          })
          test.ActivateCustomSignal("test message!")
      }
      function testHandler(message) {
          console.log("Message - " message)
      }
    }
}
```

# Getting started

## Step 1: Install NuGet package

```bash
dotnet add package Qml.Net
```

## Step 2: Compile the native bindings

*The native libraries aren't shipped with the NuGet package considering the complexity of a Qt/Qml installation, but there are discussions to do so (see [here](https://github.com/pauldotknopf/Qml.Net/issues/33))*

1. Install Qt (at least 5.9) and Qt Creator.
2. Build and deploy ```src/native/QmlNet/QmlNet.pro```.
3. Massage your ```PATH```/```LD_LIBRARY_PATH```/```DYLD_LIBRARY_PATH``` to contain the path that you've deployed the native ```QmlNet``` library.

# Currently implemented

- [x] Support for all the basic Qml types and the back-and-forth between them (```DateTime```, ```string```, etc).
- [x] Reading/setting properties on .NET objects.
- [x] Invoking methods on .NET obejcts.
- [x] Declaring and activating signals on .NET objects.
- [x] ```async``` and ```await``` with support for awaiting and getting the result from Qml.

# Not implemented (but planned)

- [ ] Compiling Qml resource files and bundling them within .NET.
- [ ] Passing dynamic javascript objects to .NET as ```dynamic```. They will be either a live mutable instance, or as a JSON serialized snapshot of the object.
- [ ] Passing ```QObject``` types to .NET with support for interacting with signals/slots/properties on them.
- [ ] .NET Events to signals
- [ ] Custom V8 type that looks like an array, but wraps a .NET ```IList<T>``` instance, for modification of list in Qml, and performance.
- [ ] General perf improvements (particularly with reflection).
- [ ] NuGet/MyGet feed.
