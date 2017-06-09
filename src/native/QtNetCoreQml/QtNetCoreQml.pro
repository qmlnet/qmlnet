#-------------------------------------------------
#
# Project created by QtCreator 2017-05-31T14:08:15
#
#-------------------------------------------------

QT       += gui qml core-private

TARGET = QtNetCoreQml
TEMPLATE = lib

DEFINES += QTNETCOREQML_LIBRARY
DEFINES += QT_DEPRECATED_WARNINGS

SOURCES += qguiapplication_helpers.cpp \
    QtNetCoreQml_wrap.cxx \
    net_qml_register_type.cpp \
    net_qml_meta.cpp \
    net_qml_value.cpp \
    net_qml_value_type.cpp

HEADERS += qguiapplication_helpers.h \
    qtnetcoreqml_global.h \
    net_qml_register_type.h \
    net_qml_meta.h \
    net_qml_value.h \
    net_qml_value_type.h

unix {
    target.path = /usr/lib
    INSTALLS += target
}
