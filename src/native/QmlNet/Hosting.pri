INCLUDEPATH += $$PWD

HEADERS += $$PWD/Hosting/coreclrhost.h \
    $$PWD/Hosting/CoreHost.h

SOURCES += \
    $$PWD/Hosting/CoreHost.cpp

unix {
    LIBS += -ldl
}

# These settings are needed to get symbols
# for the current running process.
macx {
    # nothing needed for OSX
}
unix:!macx {
    QMAKE_LFLAGS += -fPIC -rdynamic
}
win32 {
    QMAKE_LFLAGS += /FIXED:NO
}
