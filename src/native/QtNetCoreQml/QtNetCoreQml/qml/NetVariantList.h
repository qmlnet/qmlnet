#ifndef NETVARIANTLIST_H
#define NETVARIANTLIST_H

#include <QtNetCoreQml.h>
#include <QtNetCoreQml/qml/NetVariant.h>
#include <QSharedPointer>

class NetVariantList
{
public:
    NetVariantList();
    ~NetVariantList();

    int count();

private:
    QList<QSharedPointer<NetVariant>> variants;
};

struct NetVariantListContainer {
    QSharedPointer<NetVariantList> list;
};

#endif // NETVARIANTLIST_H
