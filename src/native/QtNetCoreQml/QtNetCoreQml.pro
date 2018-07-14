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

SOURCES += qguiapplication_helpers.cpp \
    net_qml_register_type.cpp \
    net_qml_activate_signal.cpp \
    net_qml_meta.cpp \
    net_qml_value.cpp \
    net_qml_value_type.cpp \
    net_type_info.cpp \
    net_instance.cpp \
    net_type_info_method.cpp \
    net_type_info_property.cpp \
    net_type_info_manager.cpp \
    net_variant.cpp \
    net_test_helper.cpp \
    net_test_string_interop.cpp

HEADERS += qtnetcoreqml_global.h \
    qguiapplication_helpers.h \
    net_qml_register_type.h \
    net_qml_meta.h \
    net_qml_value.h \
    net_qml_value_type.h \
    net_type_info.h \
    net_instance.h \
    net_type_info_method.h \
    net_type_info_property.h \
    net_type_info_manager.h \
    net_variant.h \
    net_test_helper.h \
    net_test_string_interop.h \
    net_qml_activate_signal.h

target.path = $$(PREFIX)
INSTALLS += target
