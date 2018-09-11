QT += quick
CONFIG += c++11

DEFINES += QT_DEPRECATED_WARNINGS

SOURCES += \
        main.cpp

RESOURCES += qml.qrc

DEFINES += "NET_ROOT=\"\\\"$$PWD/../net-output\\\"\""

include (../../../src/native/QmlNet/Hosting.pri)
