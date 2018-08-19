HEADERS += \
    $$PWD/QGuiApplication.h \
    $$PWD/QQmlApplicationEngine.h \
    $$PWD/NetVariant.h \
    $$PWD/NetValue.h \
    $$PWD/NetValueMetaObject.h \
    $$PWD/NetValueType.h \
    $$PWD/NetVariantList.h \
    $$PWD/NetTestHelper.h \
    $$PWD/QResource.h \
    $$PWD/JsNetObject.h \
    $$PWD/NetJsValue.h \
    $$PWD/QQuickStyle.h \
    $$PWD/NetValueMetaObjectPacker.h \
    $$PWD/QCommon.h \
    $$PWD/JsNetArray.h

SOURCES += \
    $$PWD/QGuiApplication.cpp \
    $$PWD/QQmlApplicationEngine.cpp \
    $$PWD/NetVariant.cpp \
    $$PWD/NetValue.cpp \
    $$PWD/NetValueMetaObject.cpp \
    $$PWD/NetValueType.cpp \
    $$PWD/NetVariantList.cpp \
    $$PWD/NetTestHelper.cpp \
    $$PWD/QResource.cpp \
    $$PWD/JsNetObject.cpp \
    $$PWD/NetJsValue.cpp \
    $$PWD/QQuickStyle.cpp \
    $$PWD/NetValueMetaObjectPacker.cpp \
    $$PWD/QCommon.cpp

versionAtLeast(QT_VERSION, 5.11.0) {
    message('511')
    SOURCES += $$PWD/JsNetArray511.cpp
} else {
    message('510')
    SOURCES += $$PWD/JsNetArray510.cpp
}
