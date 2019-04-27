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

    int getId();

    QString getFullTypeName();

    QString getBaseType() const;
    void setBaseType(const QString& baseType);

    QString getClassName();
    void setClassName(QString className);

    bool isArray();
    void setIsArray(bool isArray);

    bool isList();
    void setIsList(bool isList);

    bool hasComponentCompleted();
    void setHasComponentCompleted(bool hasComponentCompleted);

    bool hasObjectDestroyed();
    void setHasObjectDestroyed(bool hasObjectDestroyed);

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
    int _id;
    QString _fullTypeName;
    QString _baseType;
    QString _className;
    bool _isArray;
    bool _isList;
    bool _hasComponentCompleted;
    bool _hasObjectDestroyed;
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
