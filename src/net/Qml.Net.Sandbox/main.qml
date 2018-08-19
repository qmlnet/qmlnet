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
			interval: 1000; running: true; repeat: true
			onTriggered: {
			    var netObject = test.getObject()
			    var toJson = Net.serialize(netObject)
			    console.log(toJson)
			    console.log(typeof toJson)
			    toJson = JSON.parse(toJson)
			    console.log(toJson)
                console.log(typeof toJson)
			    console.log(toJson.prop1)
			}
		}
	}
    
	Image {
        source: "Images/placeholder.png"
    }

	TestQmlImport {
		id: test
	}
}
