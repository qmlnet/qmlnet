#include <QmlNet.h>
#include <QmlNet/qml/QLocaleInterop.h>
#include <QmlNetUtilities.h>
#include <QLocale>

extern "C" {

Q_DECL_EXPORT QmlNetStringContainer* qlocale_set_default_name(const char* name)
{
    QLocale locale = QLocale(QString(name));
    QLocale::setDefault(locale);
    return createString(locale.name());
}

}
