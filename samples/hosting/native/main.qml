import QtQuick 2.9
import QtQuick.Controls 2.2
import test 1.0

ApplicationWindow {
    visible: true
    width: 640
    height: 480
    title: qsTr("Tabs")

    SwipeView {
        id: swipeView
        anchors.fill: parent
        currentIndex: tabBar.currentIndex

        Page1Form {
        }

        Page2Form {
        }
    }

    footer: TabBar {
        id: tabBar
        currentIndex: swipeView.currentIndex

        TabButton {
            text: qsTr("Page 1")
        }
        TabButton {
            text: qsTr("Page 2")
        }
    }

    Timer {
        interval: 500; running: true; repeat: true
        onTriggered: {
            test.testMethod()
        }
    }

    TestObject {
        id: test
        Component.onCompleted: {
            // Call .NET
            test.testMethod()
        }
    }
}
