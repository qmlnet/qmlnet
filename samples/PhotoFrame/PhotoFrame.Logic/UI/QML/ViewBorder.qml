import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0
import QtGraphicalEffects 1.0

Item {
    anchors.fill: parent
    property var viewModel

    Rectangle {
        anchors.fill: parent
        color: viewModel.borderColor
    }

    Image {
        anchors.fill: parent
        anchors.topMargin: viewModel.borderWidth
        anchors.bottomMargin: viewModel.borderWidth
        anchors.leftMargin: viewModel.borderWidth
        anchors.rightMargin: viewModel.borderWidth
        fillMode: Image.PreserveAspectFit
        id: imgPhoto
        source: viewModel.imageUri
        visible: false
    }

    DropShadow {
        anchors.fill: imgPhoto
        horizontalOffset: 3
        verticalOffset: 3
        radius: 8.0
        samples: 17
        color: "#80000000"
        source: imgPhoto
    }
}
