#include <QmlNet/qml/QQuickStyle.h>
#include <QQuickStyle>

extern "C" {

Q_DECL_EXPORT void qquickstyle_setFallbackStyle(LPWCSTR style)
{
    QQuickStyle::setFallbackStyle(QString::fromUtf16(style));
}

Q_DECL_EXPORT void qquickstyle_setStyle(LPWCSTR style)
{
    QQuickStyle::setStyle(QString::fromUtf16(style));
}

}
