# Qt/Qml support for .NET/.NET Core on Linux/OSX/Windows

This is a work-in-progress to bridge .NET to Qml in a seamless way.

# The idea

## Define a .NET type (POCO)

```c#
public class QmlType
{
    /// <summary>
    /// Properties are exposed to QML.
    /// </summary>
    public string StringProperty { get; set; }

    /// <summary>
    /// Events get exposed as signals.
    /// </summary>
    public event Action<string> CustomEvent { get; set; }

    /// <summary>
    /// Methods can return .NET types.
    /// The returned type can be invoked from QML (properties/methods/events/etc).
    /// </summary>
    /// <returns></returns>
    public QmlType CreateNetObject()
    {
        return new QmlType();
    }

    /// <summary>
    /// QML can pass .NET types to .NET methods.
    /// </summary>
    /// <param name="parameter"></param>
    public void TestMethod(QmlType parameter)
    {
    }

    /// <summary>
    /// QML can also pass QML/C++ objects that can be invoked from .NET
    /// </summary>
    /// <param name="qObject"></param>
    public void TestMethodWithQObject(dynamic qObject)
    {
        string result = qObject.PropertyDefinedInCpp;
        qObject.MethodDefinedInCpp(result);
    }
}
```

## Register your new type with QML.

```c#
using (var app = new QGuiApplication(r))
{
    using (var engine = new QQmlApplicationEngine())
    {
        // Register our new type to be used in QML
        QQmlApplicationEngine.RegisterType<QmlType>("test", 1, 1);
        engine.loadFile("main.qml");
        return app.exec();
    }
}
```

## Using the .NET type in QML

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
          test.CustomEvent()
          // We can read/set properties
          console.log(test.StringProperty)
          test.StringProperty = "New value!"
          // We can return .NET types (even ones not registered with QML).
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
