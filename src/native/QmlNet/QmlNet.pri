QT += gui qml qml-private core-private quickcontrols2 webengine

CONFIG(enable-webengine) {
    QT += webengine
    DEFINES += QMLNET_WEBENGINE
}

INCLUDEPATH += $$PWD

HEADERS += $$PWD/QmlNet.h \
    $$PWD/QmlNetUtilities.h

include (QmlNet/types/types.pri)
include (QmlNet/qml/qml.pri)

SOURCES += \
    $$PWD/QmlNet.cpp \
    $$PWD/QmlNetUtilities.cpp
