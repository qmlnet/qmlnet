#ifndef NET_TYPE_INFO_H
#define NET_TYPE_INFO_H

#include <QmlNet.h>
#include <QList>
#include <QString>
#include <QSharedPointer>
#include <QEnableSharedFromThis>

class NetMethodInfo;
class NetPropertyInfo;
class NetSignalInfo;
class NetTypeArrayFacade;

class NetTypeInfo : public QEnableSharedFromThis<NetTypeInfo> {
public:
    NetTypeInfo(QString fullTypeName);
    ~NetTypeInfo();

    QString getFullTypeName();

    QString getClassName();
    void setClassName(QString className);

    NetVariantTypeEnum getPrefVariantType();
    void setPrefVariantType(NetVariantTypeEnum variantType);

    bool isArray();
    void setIsArray(bool isArray);

    bool isList();
    void setIsList(bool isList);

    void addMethod(const QSharedPointer<NetMethodInfo>& methodInfo);
    int getMethodCount();
    QSharedPointer<NetMethodInfo> getMethodInfo(int index);

    int getLocalMethodCount();
    QSharedPointer<NetMethodInfo> getLocalMethodInfo(int index);

    int getStaticMethodCount();
    QSharedPointer<NetMethodInfo> getStaticMethodInfo(int index);

    void addProperty(const QSharedPointer<NetPropertyInfo>& property);
    int getPropertyCount();
    QSharedPointer<NetPropertyInfo> getProperty(int index);

    void addSignal(const QSharedPointer<NetSignalInfo>& signal);
    int getSignalCount();
    QSharedPointer<NetSignalInfo> getSignal(int index);

    QSharedPointer<NetTypeArrayFacade> getArrayFacade();

    bool isLoaded();
    bool isLoading();
    void ensureLoaded();

    QMetaObject* metaObject;

private:
    QString _fullTypeName;
    QString _className;
    NetVariantTypeEnum _variantType;
    bool _isArray;
    bool _isList;
    QList<QSharedPointer<NetMethodInfo>> _methods;
    QList<QSharedPointer<NetMethodInfo>> _methodsLocal;
    QList<QSharedPointer<NetMethodInfo>> _methodsStatic;
    QList<QSharedPointer<NetPropertyInfo>> _properties;
    QList<QSharedPointer<NetSignalInfo>> _signals;
    QSharedPointer<NetTypeArrayFacade> _arrayFacade;
    bool _arrayFacadeLoaded;
    bool _lazyLoaded;
    bool _isLoading;
};

struct Q_DECL_EXPORT NetTypeInfoContainer {
    QSharedPointer<NetTypeInfo> netTypeInfo;
};

#endif // NET_TYPE_INFO_H
