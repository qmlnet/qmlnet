#-------------------------------------------------
#
# Project created by QtCreator 2017-05-31T14:08:15
#
#-------------------------------------------------

QT       += gui qml qml-private core-private

CONFIG += c++11
CONFIG += plugin

TARGET = QmlNet
TEMPLATE = lib

DEFINES += QMLNET_LIBRARY
DEFINES += QT_DEPRECATED_WARNINGS

include(QmlNet.pri)

target.path = $$(PREFIX)
INSTALLS += target

win32 {
    qtlibs.path = $$(PREFIX)
    qtlibs.files = $$[QT_INSTALL_BINS]/Qt5Core.dll \
        $$[QT_INSTALL_BINS]/Qt5Gui.dll \
        $$[QT_INSTALL_BINS]/Qt5Network.dll \
        $$[QT_INSTALL_BINS]/Qt5Qml.dll \
        $$[QT_INSTALL_BINS]/Qt5Quick.dll \
        $$[QT_INSTALL_BINS]/Qt5QuickControls2.dll \
        $$[QT_INSTALL_BINS]/Qt5QuickTemplates2.dll
    INSTALLS += qtlibs

    qtplugins.path = $$(PREFIX)/plugins
    qtplugins.files = $$[QT_INSTALL_PLUGINS]/*
    INSTALLS += qtplugins

    qtqml.path = $$(PREFIX)/qml
    qtqml.files = $$[QT_INSTALL_QML]/*
    INSTALLS += qtqml
}

macx {
    # See here: https://stackoverflow.com/questions/51638447/c-sharp-pinvoke-returning-invalid-wrong-bool-value-only-when-native-code-compil
    CONFIG += debug

    qtlibs.path = $$(PREFIX)/lib
    qtlibs.files = $$[QT_INSTALL_LIBS]/*
    INSTALLS += qtlibs

    qtplugins.path = $$(PREFIX)/plugins
    qtplugins.files = $$[QT_INSTALL_PLUGINS]/*
    INSTALLS += qtplugins

    qtqml.path = $$(PREFIX)/qml
    qtqml.files = $$[QT_INSTALL_QML]/*
    INSTALLS += qtqml

    QMAKE_RPATHDIR += @loader_path/lib
}

unix:!macx {
    qtlibs.path = $$(PREFIX)/lib
    qtlibs.files = $$[QT_INSTALL_LIBS]/*
    INSTALLS += qtlibs

    qtplugins.path = $$(PREFIX)/plugins
    qtplugins.files = $$[QT_INSTALL_PLUGINS]/*
    INSTALLS += qtplugins

    qtqml.path = $$(PREFIX)/qml
    qtqml.files = $$[QT_INSTALL_QML]/*
    INSTALLS += qtqml

    QMAKE_RPATHDIR = $ORIGIN/lib
}
