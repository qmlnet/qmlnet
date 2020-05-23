#include <QmlNet/qml/QQuickStyle.h>
#include <QQuickStyle>

extern "C" {

Q_DECL_EXPORT void qquickstyle_setFallbackStyle(const QChar* style)
{
    QQuickStyle::setFallbackStyle(QString(style));
}

Q_DECL_EXPORT void qquickstyle_setStyle(const QChar* style)
{
    QQuickStyle::setStyle(QString(style));
}

}
