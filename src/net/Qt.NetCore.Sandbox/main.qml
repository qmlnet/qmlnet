import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0
import test 1.0

ApplicationWindow {
    visible: true
    width: 640
    height: 480
    title: qsTr("Hello World")

	Item {
		Timer {
			interval: 10; running: true; repeat: true
			onTriggered: {
				var par = test.Create()
                test.TestMethod(par)
                gc()
			}
		}
	}

	TestQmlImport {
		id: test
	}
}
