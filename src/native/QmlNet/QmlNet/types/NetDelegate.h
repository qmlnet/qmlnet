#ifndef NETDELEGATE_H
#define NETDELEGATE_H

#include <QmlNet.h>
#include <QSharedPointer>

class NetDelegate
{
public:
    NetDelegate(NetGCHandle* gcHandle);
    ~NetDelegate();
    NetGCHandle* getGCHandle();
private:
    NetGCHandle* gcHandle;
};

struct NetDelegateContainer {
    QSharedPointer<NetDelegate> delegate;
};

#endif // NETDELEGATE_H
