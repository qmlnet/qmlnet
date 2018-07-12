import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0

Item {
    property alias textField1: textField1
    property alias button1: button1
	property alias button2: button2
    property alias textFieldMessage: textFieldMessage
    property alias buttonSendMessage: buttonSendMessage

    RowLayout {
        id: firstRow
        anchors.horizontalCenter: parent.horizontalCenter
        anchors.topMargin: 20
        anchors.top: parent.top

        TextField {
            id: textField1
            placeholderText: qsTr("Text Field")
        }

        Button {
            id: button1
            text: qsTr("Press Me")
        }

        Button {
            id: button2
            text: qsTr("Press Me wait for result")
        }
    }
    RowLayout {
        id: secondRow
        anchors.horizontalCenter: parent.horizontalCenter
        anchors.topMargin: 20
        anchors.top: firstRow.bottom

        TextField {
            id: textFieldMessage
            placeholderText: qsTr("Message")
        }

        Button {
            id: buttonSendMessage
            text: qsTr("Send")
        }
    }
}
