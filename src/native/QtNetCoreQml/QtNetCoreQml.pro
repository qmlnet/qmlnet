#-------------------------------------------------
#
# Project created by QtCreator 2017-05-31T14:08:15
#
#-------------------------------------------------

QT       += gui qml

TARGET = QtNetCoreQml
TEMPLATE = lib

DEFINES += QTNETCOREQML_LIBRARY
DEFINES += QT_DEPRECATED_WARNINGS

SOURCES += qguiapplication_helpers.cpp \
    QGuiApplication_wrap.cxx

HEADERS += qguiapplication_helpers.h \
    qtnetcoreqml_global.h

unix {
    target.path = /usr/lib
    INSTALLS += target
}
