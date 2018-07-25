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
			    var t = {
			        test: false
			    }
			    var task = test.Test(t)
			    //Net.await(task, function() {
			    //    console.log('callback called!')
			    //})
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
