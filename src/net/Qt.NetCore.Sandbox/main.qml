import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0
import test 1.0
import testInstances 1.0

ApplicationWindow {
    visible: true
    width: 640
    height: 480
    title: qsTr("Hello World")
	Item {
		Timer {
			interval: 1000; running: true; repeat: true
			onTriggered: {
                var o = test.GetSharedInstance()
                o.testSignal.connect(function(message) {
                    console.log("Signal was raised: " + message)
                })
                var o2 = test.GetSharedInstance()
                o2.testSignal("Hello")
			}
		}
		Timer {
		    id: instanceCheckTimer
			interval: 1000; running: true; repeat: true
			onTriggered: {
				if(testInstances.State == 0) {
					var ref1 = testInstances.GetInstance()
					var ref2 = testInstances.GetInstance()

					ref1 = null
					ref2 = null
					
					console.log("Created and deleted two references on QML side. IsAlive = " + testInstances.IsInstanceAlive())
					testInstances.State = 1
				} else if(testInstances.State == 1) {
					testInstances.DeleteInstances()
					console.log("Deleting .Net references. IsAlive = " + testInstances.IsInstanceAlive())
					testInstances.State = 2
				} else if(testInstances.State == 2) {
					if(testInstances.IsInstanceAlive()) {
						console.log("Yeah! Instance has been released!");
						instanceCheckTimer.running = false
					}
				}
			}
		}
	}

	TestQmlImport {
		id: test
	}

	TestQmlInstanceHandling {
		id: testInstances
	}
}
