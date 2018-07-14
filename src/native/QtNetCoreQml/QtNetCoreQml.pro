#-------------------------------------------------
#
# Project created by QtCreator 2017-05-31T14:08:15
#
#-------------------------------------------------

QT       += gui qml core-private

CONFIG += c++11
CONFIG += plugin

TARGET = QtNetCoreQml
TEMPLATE = lib

DEFINES += QTNETCOREQML_LIBRARY
DEFINES += QT_DEPRECATED_WARNINGS

SOURCES += net_type_info.cpp \
    net_type_info_method.cpp \
    net_type_info_property.cpp \
    net_type_info_manager.cpp

HEADERS += qtnetcoreqml_global.h \
    net_type_info.h \
    net_type_info_method.h \
    net_type_info_property.h \
    net_type_info_manager.h

target.path = $$(PREFIX)
INSTALLS += target
