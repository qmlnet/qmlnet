#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <QtNetCoreQml.h>
#include <QList>
#include <QString>
#include <QSharedPointer>

class NetTypeInfo {
public:
    NetTypeInfo(QString fullTypeName);
    ~NetTypeInfo();

    QString getFullTypeName();

private:
    QString _fullTypeName;
};

#ifdef __cplusplus
extern "C" {
#endif

struct NetTypeInfoContainer {
    QSharedPointer<NetTypeInfo> netTypeInfo;
};

NetTypeInfoContainer* type_info_create(LPWSTR fullTypeName);
void type_info_destroy(NetTypeInfoContainer* netTypeInfo);
LPWSTR type_info_getFullTypeName(NetTypeInfoContainer* netTypeInfo);

#ifdef __cplusplus
}
#endif

#endif // NET_TYPE_INFO_H
