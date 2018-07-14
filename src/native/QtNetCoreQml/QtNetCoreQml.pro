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

include(QtNetCoreQml.pri)

target.path = $$(PREFIX)
INSTALLS += target
