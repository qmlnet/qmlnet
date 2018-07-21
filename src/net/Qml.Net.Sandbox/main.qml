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
			id: signalTimer
			interval: 1000; running: true; repeat: true
			onTriggered: {
                var o = test.GetSharedInstance()
                o.testSignal.connect(function(message) {
                    console.log("Signal was raised: " + message)
					signalTimer.running = false
                })
                var o2 = test.GetSharedInstance()
                o2.testSignal("Hello")
			}
		}
		Timer {
		    id: instanceCheckTimer
			property var instanceRef: null
			interval: 1000; running: true; repeat: true
			onTriggered: {
				testInstances.GarbageCollect()
				gc()
				switch(testInstances.State) {
					case 0:
						console.log("Creating two QML references")
						var ref1 = testInstances.GetInstance()
						var ref2 = testInstances.GetInstance()

						ref1 = null
						ref2 = null
					
						console.log("Created and deleted two references on QML side. IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.State++
						break
					case 1:
						testInstances.DeleteInstance()
						console.log("Deleting .Net references. IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.State++
						break
					case 2:
						if(!testInstances.IsInstanceAlive()) {
							console.log("Yeah! Instance has been released!");
							testInstances.State++
						}
						break
					case 3:
						testInstances.CreateNewInstance()
						console.log("Created new .Net Instance. IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.State++
						break
					case 4:
						instanceRef = testInstances.GetInstance()
						var secondLocalRef = testInstances.GetInstance()

						console.log("Created two QML refs. One scoped and one long living. IsAlive = " + testInstances.IsInstanceAlive())

						testInstances.State++
						//secondLocalRef will be freed here
						break
					case 5:
						instanceRef.Log("Long living QML ref still works!")
						testInstances.State++
						break
					case 6:
						instanceRef.Log("Long living QML ref still works!")
						testInstances.State++
						break;
					case 7:
						console.log("Deleting long living QML ref. IsAlive = " + testInstances.IsInstanceAlive())
						instanceRef = null
						testInstances.State++
						break;
					case 8:
						console.assert(testInstances.IsInstanceAlive(), ".Net object IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.State++
						break;
					case 9:
						console.assert(testInstances.IsInstanceAlive(), ".Net object IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.State++
						break;
					case 10:
						console.log("Releasing .Net instance a second time. IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.DeleteInstance()
						testInstances.State++
						break;
					case 11:
						if(!testInstances.IsInstanceAlive()) {
							console.log("Yeah! Instance has been released a second time!");
							testInstances.State++
						}
						break;
					case 12:
						testInstances.CreateNewInstance()
						console.log("Created new .Net Instance. IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.State++
						break
					case 13:
						instanceRef = testInstances.GetInstance()
						testInstances.DeleteInstance()
						console.log("a QML ref and deleted the .Net ref. IsAlive = " + testInstances.IsInstanceAlive())

						testInstances.State++
						break
					case 14:
						console.assert(testInstances.IsInstanceAlive(), ".Net object IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.State++
						break;
					case 15:
						console.assert(testInstances.IsInstanceAlive(), ".Net object IsAlive = " + testInstances.IsInstanceAlive())
						testInstances.State++
						break;
					case 16:
						console.log("Releasing last QML ref. IsAlive = " + testInstances.IsInstanceAlive())
						instanceRef = null
						testInstances.State++
						break;
					case 17:
						if(!testInstances.IsInstanceAlive()) {
							console.log("Yeah! Instance has been released a third time!");
							testInstances.State++
						}
						break;
					default:
						instanceCheckTimer.running = false
						break;
				}
			}
		}
	}
    
	Image {
        source: "Images/placeholder.png"
    }

	TestQmlImport {
		id: test
	}

	TestQmlInstanceHandling {
		id: testInstances
	}
}
