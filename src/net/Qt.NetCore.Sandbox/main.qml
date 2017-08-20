import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0
import test 1.1
import test.io 1.0

ApplicationWindow {
    visible: true
    width: 640
    height: 480
    title: qsTr("Hello World")

	Item {
		Timer {
			interval: 5; running: true; repeat: true
			onTriggered: {
				var newValue = testt.TestMethod("2017-08-20T01:18:25.2592555-04:00")
				console.log("Value is: " + newValue);
				testt.TestMethod(newValue)
			}
		}
	}

	Text {
		id: textt
	}

	TestQmlImport {
		id: testt
	}
}
