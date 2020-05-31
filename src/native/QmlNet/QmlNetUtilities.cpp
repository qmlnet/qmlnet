#include <QmlNetUtilities.h>

extern "C" {

QmlNetStringContainer* createString(const QString& value)
{
    if(value.isNull()) {
        return nullptr;
    }

    QmlNetStringContainer* result = new QmlNetStringContainer();
    result->container = new QString(value);
    result->data = result->container->utf16();

    return result;
}

void freeString(QmlNetStringContainer* container)
{
    if(container) {
        
            delete container->container;
        
        delete container;
    }
}

}

// According to the Microsoft documentation, the string returned from unmanaged
// must be allocated using CoTaskMemAlloc, which doesn't exist on Linux ofcourse.
// This issue says that malloc should be used instead: https://github.com/dotnet/runtime/issues/10748
#ifdef Q_OS_WIN
#include <combaseapi.h>
#define interop_malloc CoTaskMemAlloc
#define interop_free CoTaskMemFree
#else
#define interop_malloc malloc
#define interop_free free
#endif

QChar *returnStringToDotNet(const QString &str)
{
    static_assert(sizeof(QChar) == 2, "QChar must be 2-byte UTF-16");

    if(str.isNull()) {
        return nullptr;
    }

    auto len = str.length();
    auto result = reinterpret_cast<QChar *>(interop_malloc((len + 1) * sizeof(QChar)));

    memcpy(result, str.utf16(), len * sizeof(QChar));
    result[len] = 0;

    return result;
}

QString takeStringFromDotNet(QChar *str)
{
    if (!str) {
        return QString::null;
    }

    QString result(reinterpret_cast<QChar*>(str));

    interop_free(str);

    return result;
}
