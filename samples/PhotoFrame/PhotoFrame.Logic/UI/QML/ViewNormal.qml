import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0

Item {
    anchors.fill: parent
    property var viewModel
    Image {
        anchors.fill: parent
        fillMode: Image.PreserveAspectFit
        id: imgPhoto
        source: viewModel.imageUri
    }
}
