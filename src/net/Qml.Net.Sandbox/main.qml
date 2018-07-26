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
			interval: 5000; running: true; repeat: true
			onTriggered: {
			    console.log('calling function with callback')
			    var task = test.Test(function() {
			        console.log('in callback, going to call AnotherMethod')
			        test.AnotherMethod()
			        console.log('done calling AnotherMethod')
			    })
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
