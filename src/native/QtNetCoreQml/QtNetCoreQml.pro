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
    swig.cpp \
    net_qml_register_type.cpp \
    net_qml_meta.cpp \
    net_qml_value.cpp \
    net_qml_value_type.cpp \
    net_type_info.cpp \
    net_instance.cpp

HEADERS += qtnetcoreqml_global.h \
    qguiapplication_helpers.h \
    swig.h \
    net_qml_register_type.h \
    net_qml_meta.h \
    net_qml_value.h \
    net_qml_value_type.h \
    net_type_info.h \
    net_instance.h

unix {
    target.path = /usr/lib
    INSTALLS += target
}

DISTFILES += \
    swig/QtNetCoreQml.i \
    swig/NetInvoker.i \
    swig/NetTypeInfo.i \
    swig/QCoreApplication.i \
    swig/QGuiApplication.i \
    swig/QQmlApplicationEngine.i \
    swig/QQmlRegisterType.i
