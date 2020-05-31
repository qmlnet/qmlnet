
#include <QmlNet/qml/NetTranslator.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNetUtilities.h>

NetTranslator::NetTranslator(NetGCHandle *callbackDelegate, TranslateCallback callback, QObject *parent) : _callbackDelegate(callbackDelegate), _callback(callback), QTranslator(parent) {
}

bool NetTranslator::isEmpty() const {
    // We only install this translator if C# has callbacks, so
    // it is never considered to be empty
    return false;
}

QString NetTranslator::translate(const char *context, const char *sourceText, const char *disambiguation, int n) const {
    auto contextLength = context ? static_cast<int>(strlen(context)) : -1;
    auto sourceTextLength = sourceText ? static_cast<int>(strlen(sourceText)) : -1;
    auto disambiguationLength = disambiguation ? static_cast<int>(strlen(disambiguation)) : -1;

    auto str = _callback(context, contextLength, sourceText, sourceTextLength, disambiguation, disambiguationLength, n);
    return takeStringFromDotNet(str);
}

bool NetDataTranslator::load(QByteArray data, const QString &directory) {
    _data = std::move(data);

    if (!QTranslator::load(reinterpret_cast<const uchar*>(_data.constData()), _data.size(), directory)) {
        _data = QByteArray(); // Do not keep the data copy alive unnecessarily
        return false;
    }

    return true;
}
