# Qt/Qml support for .NET/.NET Core on Linux/OSX/Windows

This is a work-in-progress to bridge .NET to Qml in a seamless way.

To see what is currently working, check out the [unit tests](https://github.com/pauldotknopf/net-core-qml/tree/master/src/net/Qt.NetCore.Tests). Checkout the outstanding items that need to be done [here](#things-left-to-do).

The intended platforms to support include:

* Runtimes:
  * .NET Framework Full
  * .NET Core
  * Mono
* Operating systems
  * Linux
  * OSX
  * Windows

*As of now, the only focus is on **.NET Core** (Linux/OSX/Window). The other frameworks should theoretically work though. Drop us an issue if you have any problems. When there is enough demand and userbase, I'd be happy to fully bring in other frameworks.*

## The idea

**Define a .NET type (POCO)**

```c#
public class QmlType
{
    /// <summary>
    /// Properties are exposed to Qml.
    /// </summary>
    public string StringProperty { get; set; }

    /// <summary>
    /// Events get exposed as signals.
    /// </summary>
    public event Action<string> CustomEvent { get; set; }

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
    public async Task TestAsync()
    {
        Console.WriteLine("Hello from UI thread!");
        await Task.Run(() =>
        {
            Console.WriteLine("Hello from background thread!");
        });
        Console.WriteLine("Welcome back to the UI thread!");
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
          // Wire up signal event handlers
          test.CustomEvent.connect(testHandler)
          // And invoke them. This will also trigger any event
          // handlers assigned in .NET.
          test.CustomEvent("Event message")
          // We can read/set properties
          console.log(test.StringProperty)
          test.StringProperty = "New value!"
          // We can return .NET types (even ones not registered with Qml).
          var netObject = test.CreateNetObject();
          // All properties/methods/signals can be invoked on "netObject"
          // We can also pass the .NET object back to .NET
          netObject.TestMethod(netObject)
      }
      function testHandler(message) {
          console.log("Message - " message)
      }
    }
}
```

## Building and running

Setting up an environment takes a little effort, but it is easy enough.

1. Install Qt and Qt Creator.
2. Build and deploy ```src/native/QtNetCoreQml/QtNetCoreQml.pro```.
3. Open ```src/net/Qt.NetCore.sln``` and run the sandbox! *Mark sure your ```LD_LIBRARY_PATH``` (etc) is setup for your app to properly find your ```QtNetCoreQml``` installation.*

There will be a proper NuGet/MyGet feed shortly.

Also, there is no plans to ever bundle the native libraries into the NuGet packages considering the complexity of a traditional Qt installation. It will always be recommend/required to install the native ```QtNetCoreQml``` on the OS of your choice.

## Currently implemented

- [x] Support for all the basic Qml types and the back-and-forth between them (```DateTime```, ```string```, etc).
- [x] Reading/setting properties on .NET objects.
- [x] Invoking methods on .NET obejcts.

## Things left to do

- [ ] Passing dynamic javascript objects to .NET as ```dynamic```. They will be either a live mutable instance, or as a JSON serialized snapshot of the object.
- [ ] Passing ```QObject``` types to .NET with support for interacting with signals/slots/properties on them.
- [ ] Cancellable async tasks.
- [ ] Return value from async tasks.
- [ ] ```INotifyPropertyChanged``` support for signal notification of property changes in Qml. This will allow Qml to bind to .NET properties.
- [ ] .NET Events to signals
- [ ] Custom V8 type that looks like an array, but wraps a .NET ```IList<T>``` instance, for modification of list in Qml, and performance.
- [ ] General perf improvements (particularly with reflection).
- [ ] NuGet/MyGet feed.
