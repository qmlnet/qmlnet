#ifndef NETVARIANTLIST_H
#define NETVARIANTLIST_H

#include <QmlNet.h>
#include <QmlNet/qml/NetVariant.h>
#include <QSharedPointer>

class NetVariantList
{
public:
    NetVariantList();
    ~NetVariantList();
    int count();
    void add(const QSharedPointer<NetVariant>& variant);
    QSharedPointer<NetVariant> get(int index);
    void remove(int index);
    void clear();
    QString debugDisplay();
private:
    QList<QSharedPointer<NetVariant>> variants;
};

struct NetVariantListContainer {
    QSharedPointer<NetVariantList> list;
};

#endif // NETVARIANTLIST_H
