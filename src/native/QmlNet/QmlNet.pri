QT += gui qml core-private quickcontrols2 widgets testlib

CONFIG(enable-webengine) {
    QT += webengine
    DEFINES += QMLNET_WEBENGINE
}

CONFIG(qmlnet-trace) {
    DEFINES += QMLNET_TRACE
}

INCLUDEPATH += $$PWD

HEADERS += $$PWD/QmlNet.h \
    $$PWD/QmlNetUtilities.h

include (QmlNet/types/types.pri)
include (QmlNet/qml/qml.pri)

SOURCES += \
    $$PWD/QmlNet.cpp \
    $$PWD/QmlNetUtilities.cpp
