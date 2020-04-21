<p align="center"><a href="https://github.com/qmlnet/qmlnet" rel="nofollow"><img src="https://qmlnet.github.io/qmlnet.png" width="150"></a></p>

<div style="text-align: center;">

[![Qml.Net](https://img.shields.io/nuget/v/Qml.Net.svg?style=flat&label=Qml.Net)](http://www.nuget.org/packages/Qml.Net/) [![Build status](https://travis-ci.com/qmlnet/qmlnet.svg?branch=develop)](https://travis-ci.com/qmlnet/qmlnet) [![Build status](https://ci.appveyor.com/api/projects/status/l0hh7ranqawj682y/branch/develop?svg=true)](https://ci.appveyor.com/project/pauldotknopf/qmlnet/) [![Gitter](https://img.shields.io/gitter/room/qmlnet/Lobby.svg?style=flat)](https://gitter.im/qmlnet/Lobby) [![All Contributors](https://img.shields.io/badge/all_contributors-8-orange.svg)](#contributors) [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://paypal.me/pauldotknopf)

</div>
------------------------

A Qt/Qml integration with .NET

Supported platforms/runtimes:
* Runtimes:
  * .NET Framework
  * .NET Core
  * Mono
* Operating systems
  * Linux
  * OSX
  * Windows

# First look

<img src="https://github.com/pauldotknopf/Qml.Net.Examples/blob/master/assets/features.gif" alt="" style="max-width:100%;" width="200">

# Documentation

https://qmlnet.github.io/

# Getting started

```bash
dotnet add package Qml.Net
dotnet add package Qml.Net.WindowsBinaries
dotnet add package Qml.Net.OSXBinaries
dotnet add package Qml.Net.LinuxBinaries
```  

**Note for Linux users**: Package `libc6-dev` is required to be installed because it contains `libdl.so` that needed to work

# Examples
Checkout the [examples](https://github.com/qmlnet/qmlnet-examples) on how to do many things with Qml.Net.

# Quick overview

**Define a .NET type (POCO)**

```c#
//QmlType.cs
using Qml.Net;
using System.Threading.Tasks;

namespace QmlQuickOverview
{
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
            return "async result!";
        }
        
        /// <summary>
        /// Qml can also pass Qml/C++ objects that can be invoked from .NET
        /// </summary>
        /// <param name="qObject"></param>
        public void TestMethodWithQObject(dynamic o)
        {
            string result = o.propertyDefinedInCpp;
            o.methodDefinedInCpp(result);
            
            // You can also listen to signals on QObjects.
            var qObject = o as INetQObject;
            var handler = qObject.AttachSignal("signalName", parameters => {
                // parameters is a list of arguements passed to the signal.
            });
            handler.Dispose(); // When you are done listening to signal.
            
            // You can also listen to when a property changes (notify signal).
            handler = qObject.AttachNotifySignal("property", parameters => {
                // parameters is a list of arguements passed to the signal.
            });
            handler.Dispose(); // When you are done listening to signal.
        }
        
        /// <summary>
        /// .NET can activate signals to send notifications to Qml.
        /// </summary>
        public void ActivateCustomSignal(string message)
        {
            this.ActivateSignal("customSignal", message);
        }
    }
}

```

**Register your new type with Qml.**

```c#
//QmlExample.cs
using Qml.Net;
using Qml.Net.Runtimes;

namespace QmlQuickOverview
{
    class QmlExample
    {
        static int Main(string[] args)
        {
            RuntimeManager.DiscoverOrDownloadSuitableQtRuntime();

            using (var app = new QGuiApplication(args))
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    // Register our new type to be used in Qml
                    Qml.Net.Qml.RegisterType<QmlType>("test", 1, 1);
                    engine.Load("Main.qml");
                    return app.Exec();
                }
            }
        }
    }
}

```

**Use the .NET type in Qml**

```js
//Main.qml
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
- [x] Dynamically compiled delegates for increased performance.
- [x] Passing ```QObject``` types to .NET with support for interacting with signals/slots/properties on them.

There aren't really any important features missing that are needed for prime-time. This product is currently used on embedded devices in the medical industry.

## Contributors ✨

Thanks goes to these wonderful people!

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore -->
<table>
  <tr>
    <td align="center"><a href="http://www.devmil.de"><img src="https://avatars1.githubusercontent.com/u/6693130?v=4" width="100px;" alt="Michael Lamers"/><br /><sub><b>Michael Lamers</b></sub></a><br /><a href="https://github.com/qmlnet/qmlnet/commits?author=devmil" title="Code">💻</a></td>
    <td align="center"><a href="https://ymueller.de"><img src="https://avatars0.githubusercontent.com/u/2760830?v=4" width="100px;" alt="TripleWhy"/><br /><sub><b>TripleWhy</b></sub></a><br /><a href="https://github.com/qmlnet/qmlnet/commits?author=TripleWhy" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/MaxMommersteeg"><img src="https://avatars3.githubusercontent.com/u/9657173?v=4" width="100px;" alt="Max"/><br /><sub><b>Max</b></sub></a><br /><a href="https://github.com/qmlnet/qmlnet/commits?author=MaxMommersteeg" title="Code">💻</a> <a href="https://github.com/qmlnet/qmlnet/commits?author=MaxMommersteeg" title="Documentation">📖</a> <a href="#financial-MaxMommersteeg" title="Financial">💵</a></td>
    <td align="center"><a href="https://github.com/geigertom"><img src="https://avatars0.githubusercontent.com/u/19152463?v=4" width="100px;" alt="geigertom"/><br /><sub><b>geigertom</b></sub></a><br /><a href="https://github.com/qmlnet/qmlnet/commits?author=geigertom" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/jamesdavila"><img src="https://avatars0.githubusercontent.com/u/1946041?v=4" width="100px;" alt="James Davila"/><br /><sub><b>James Davila</b></sub></a><br /><a href="https://github.com/qmlnet/qmlnet/commits?author=jamesdavila" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/afillebrown"><img src="https://avatars2.githubusercontent.com/u/38264913?v=4" width="100px;" alt="Andy Fillebrown"/><br /><sub><b>Andy Fillebrown</b></sub></a><br /><a href="https://github.com/qmlnet/qmlnet/commits?author=afillebrown" title="Code">💻</a></td>
    <td align="center"><a href="https://linkedin.com/in/vadimperetokin"><img src="https://avatars1.githubusercontent.com/u/110988?v=4" width="100px;" alt="Vadim Peretokin"/><br /><sub><b>Vadim Peretokin</b></sub></a><br /><a href="https://github.com/qmlnet/qmlnet/commits?author=vadi2" title="Documentation">📖</a></td>
  </tr>
  <tr>
    <td align="center"><a href="https://github.com/Juhlinus"><img src="https://avatars0.githubusercontent.com/u/12988164?v=4" width="100px;" alt="Linus Juhlin"/><br /><sub><b>Linus Juhlin</b></sub></a><br /><a href="https://github.com/qmlnet/qmlnet/commits?author=Juhlinus" title="Documentation">📖</a></td>
  </tr>
</table>

<!-- ALL-CONTRIBUTORS-LIST:END -->
