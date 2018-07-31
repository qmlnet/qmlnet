QT += gui qml qml-private core-private quickcontrols2

INCLUDEPATH += $$PWD

HEADERS += $$PWD/QmlNet.h

include (QmlNet/types/types.pri)
include (QmlNet/qml/qml.pri)

SOURCES += \
    $$PWD/QmlNet.cpp
