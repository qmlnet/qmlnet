#ifndef NETTRANSLATOR_H
#define NETTRANSLATOR_H

#include <QTranslator>
#include <QString>
#include <QmlNet.h>
#include <QmlNet/types/NetDelegate.h>

/**
 * Signature of the managed delegate when converted to an unmanaged function pointer.
 * Please note that the returned pointer must be allocated with CoTaskMemAlloc or the
 * cross-platform equivalent (usually malloc). I.e. by using Marshal.StringToCoTaskMemUni.
 */
using TranslateCallback  = QChar* (__cdecl *)(const char *context, int contextLength, const char *sourceText, int sourceTextLength, const char *disambiguation, int disambiguationLength, int n);

class NetTranslator : public QTranslator
{
Q_OBJECT
public:
    NetTranslator(NetGCHandle *callbackDelegate, TranslateCallback callback, QObject *parent = nullptr);

    bool isEmpty() const override;
    QString translate(const char *context, const char *sourceText, const char *disambiguation, int n) const override;

private:
    NetDelegate const _callbackDelegate;
    TranslateCallback const _callback;
};

/**
 * Holds a reference to managed memory and frees it, once the translator is destroyed.
 */
class NetDataTranslator : public QTranslator
{
Q_OBJECT
public:
    NetDataTranslator(QObject *parent = nullptr) : QTranslator(parent) {
    }

    /**
     * Loads the translation data from the given data and keeps
     * a copy around to keep the data alive.
     */
    bool load(QByteArray data, const QString &directory = QString());

private:
    QByteArray _data;
};

#endif // NETTRANSLATOR_H
