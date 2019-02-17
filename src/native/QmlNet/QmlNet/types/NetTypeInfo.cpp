#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/types/NetSignalInfo.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/types/NetTypeArrayFacade.h>
#include <QmlNetUtilities.h>
#include <QMutex>
#include <utility>

using namespace QmlNet;

static int nextTypeId = 1;
Q_GLOBAL_STATIC(QMutex, typeIdMutex);

NetTypeInfo::NetTypeInfo(QString fullTypeName) :
    metaObject(nullptr),
    _fullTypeName(std::move(fullTypeName)),
    _variantType(NetVariantTypeEnum_Invalid),
    _isArray(false),
    _isList(false),
    _arrayFacadeLoaded(false),
    _lazyLoaded(false),
    _isLoading(false)
{
    typeIdMutex->lock();
    _id = nextTypeId;
    nextTypeId++;
    typeIdMutex->unlock();
}


NetTypeInfo::~NetTypeInfo() = default;

int NetTypeInfo::getId()
{
    return _id;
}

QString NetTypeInfo::getFullTypeName() {
    return _fullTypeName;
}

QString NetTypeInfo::getBaseType() const
{
    return _baseType;
}

void NetTypeInfo::setBaseType(const QString& baseType)
{
    _baseType = baseType;
}

QString NetTypeInfo::getClassName() {
    return _className;
}

void NetTypeInfo::setClassName(QString className) {
    _className = std::move(className);
}

NetVariantTypeEnum NetTypeInfo::getPrefVariantType() {
    return _variantType;
}

void NetTypeInfo::setPrefVariantType(NetVariantTypeEnum variantType) {
    _variantType = variantType;
}

bool NetTypeInfo::isArray()
{
    return _isArray;
}

void NetTypeInfo::setIsArray(bool isArray)
{
    _isArray = isArray;
}

bool NetTypeInfo::isList()
{
    return _isList;
}

void NetTypeInfo::setIsList(bool isList)
{
    _isList = isList;
}

void NetTypeInfo::addMethod(const QSharedPointer<NetMethodInfo>& methodInfo) {
    _methods.append(methodInfo);
    if(methodInfo->isStatic()) {
        _methodsStatic.append(methodInfo);
    } else {
        _methodsLocal.append(methodInfo);
    }
}

int NetTypeInfo::getMethodCount() {
    return _methods.size();
}

QSharedPointer<NetMethodInfo> NetTypeInfo::getMethodInfo(int index) {
    if(index < 0) return QSharedPointer<NetMethodInfo>(nullptr);
    if(index >= _methods.length()) return QSharedPointer<NetMethodInfo>(nullptr);
    return _methods.at(index);
}

int NetTypeInfo::getLocalMethodCount()
{
    return _methodsLocal.size();
}

QSharedPointer<NetMethodInfo> NetTypeInfo::getLocalMethodInfo(int index)
{
    if(index < 0) return QSharedPointer<NetMethodInfo>(nullptr);
    if(index >= _methodsLocal.length()) return QSharedPointer<NetMethodInfo>(nullptr);
    return _methodsLocal.at(index);
}

int NetTypeInfo::getStaticMethodCount()
{
    return _methodsStatic.size();
}

QSharedPointer<NetMethodInfo> NetTypeInfo::getStaticMethodInfo(int index)
{
    if(index < 0) return QSharedPointer<NetMethodInfo>(nullptr);
    if(index >= _methodsStatic.length()) return QSharedPointer<NetMethodInfo>(nullptr);
    return _methodsStatic.at(index);
}

void NetTypeInfo::addProperty(const QSharedPointer<NetPropertyInfo>& property) {
    _properties.append(property);
}

int NetTypeInfo::getPropertyCount() {
    return _properties.size();
}

QSharedPointer<NetPropertyInfo> NetTypeInfo::getProperty(int index) {
    if(index < 0) return QSharedPointer<NetPropertyInfo>(nullptr);
    if(index >= _properties.length()) return QSharedPointer<NetPropertyInfo>(nullptr);
    return _properties.at(index);
}

void NetTypeInfo::addSignal(const QSharedPointer<NetSignalInfo>& signal) {
    _signals.append(signal);
}

int NetTypeInfo::getSignalCount() {
    return _signals.size();
}

QSharedPointer<NetSignalInfo> NetTypeInfo::getSignal(int index) {
    if(index < 0) return QSharedPointer<NetSignalInfo>(nullptr);
    if(index >= _signals.size()) return QSharedPointer<NetSignalInfo>(nullptr);
    return _signals.at(index);
}

QSharedPointer<NetTypeArrayFacade> NetTypeInfo::getArrayFacade()
{
    if(_arrayFacadeLoaded) {
        return _arrayFacade;
    }
    ensureLoaded();
    _arrayFacade = NetTypeArrayFacade::fromType(sharedFromThis());
    _arrayFacadeLoaded = true;
    return _arrayFacade;
}

bool NetTypeInfo::isLoaded() {
    return _lazyLoaded;
}

bool NetTypeInfo::isLoading() {
    return _isLoading;
}

void NetTypeInfo::ensureLoaded() {
    if (_lazyLoaded) {
        return;
    }
    if(_isLoading) {
        // Prevent recursion
        qFatal("Recursion detected on type loading for type: %s", qPrintable(getFullTypeName()));
    }
    _isLoading = true;
    loadTypeInfo(sharedFromThis());
    _isLoading = false;
    _lazyLoaded = true;
}

extern "C" {

static_assert (std::is_pointer<LPWSTR>::value, "Check fromUtf16 calls below.");
static_assert (!std::is_pointer<std::remove_pointer<LPWSTR>::type>::value, "Check fromUtf16 calls below.");
static_assert (sizeof(std::remove_pointer<LPWSTR>::type) == sizeof(ushort), "Check fromUtf16 calls below.");

Q_DECL_EXPORT NetTypeInfoContainer* type_info_create(LPWSTR fullTypeName) {
    NetTypeInfoContainer* result = new NetTypeInfoContainer();
    result->netTypeInfo = QSharedPointer<NetTypeInfo>(new NetTypeInfo(QString::fromUtf16(static_cast<const char16_t*>(fullTypeName))));
    return result;
}

Q_DECL_EXPORT void type_info_destroy(NetTypeInfoContainer* netTypeInfo) {
    delete netTypeInfo;
    netTypeInfo = nullptr;
}

Q_DECL_EXPORT int type_info_getId(NetTypeInfoContainer* netTypeInfo)
{
    return netTypeInfo->netTypeInfo->getId();
}

Q_DECL_EXPORT QmlNetStringContainer* type_info_getFullTypeName(NetTypeInfoContainer* netTypeInfo) {
    QString result = netTypeInfo->netTypeInfo->getFullTypeName();
    return createString(result);
}

Q_DECL_EXPORT QmlNetStringContainer* type_info_getBaseType(NetTypeInfoContainer* netTypeInfo)
{
    auto result = netTypeInfo->netTypeInfo->getBaseType();
    return createString(result);
}

Q_DECL_EXPORT void type_info_setBaseType(NetTypeInfoContainer* netTypeInfo, LPWCSTR baseType)
{
    if(baseType == nullptr) {
        netTypeInfo->netTypeInfo->setBaseType(QString());
    } else {
        netTypeInfo->netTypeInfo->setBaseType(QString::fromUtf16(baseType));
    }
}

Q_DECL_EXPORT QmlNetStringContainer* type_info_getClassName(NetTypeInfoContainer* netTypeInfo) {
    QString result = netTypeInfo->netTypeInfo->getClassName();
    return createString(result);
}

Q_DECL_EXPORT void type_info_setClassName(NetTypeInfoContainer* netTypeInfo, LPWSTR className) {
    netTypeInfo->netTypeInfo->setClassName(QString::fromUtf16(static_cast<const char16_t*>(className)));
}

Q_DECL_EXPORT NetVariantTypeEnum type_info_getPrefVariantType(NetTypeInfoContainer* netTypeInfo) {
    return netTypeInfo->netTypeInfo->getPrefVariantType();
}

Q_DECL_EXPORT void type_info_setPrefVariantType(NetTypeInfoContainer* netTypeInfo, NetVariantTypeEnum variantType) {
    netTypeInfo->netTypeInfo->setPrefVariantType(variantType);
}

Q_DECL_EXPORT uchar type_info_setIsArray(NetTypeInfoContainer* netTypeInfo)
{
    if(netTypeInfo->netTypeInfo->isArray()) {
        return 1;
    } else {
        return 0;
    }
}

Q_DECL_EXPORT void type_info_getIsArray(NetTypeInfoContainer* netTypeInfo, uchar isArray)
{
    netTypeInfo->netTypeInfo->setIsArray(isArray == 1);
}

Q_DECL_EXPORT uchar type_info_setIsList(NetTypeInfoContainer* netTypeInfo)
{
    if(netTypeInfo->netTypeInfo->isList()) {
        return 1;
    } else {
        return 0;
    }
}

Q_DECL_EXPORT void type_info_getIsList(NetTypeInfoContainer* netTypeInfo, uchar isList)
{
    netTypeInfo->netTypeInfo->setIsList(isList == 1);
}

Q_DECL_EXPORT void type_info_addMethod(NetTypeInfoContainer* netTypeInfo, NetMethodInfoContainer* methodInfo) {
    netTypeInfo->netTypeInfo->addMethod(methodInfo->method);
}

Q_DECL_EXPORT int type_info_getMethodCount(NetTypeInfoContainer* container) {
    return container->netTypeInfo->getMethodCount();
}

Q_DECL_EXPORT NetMethodInfoContainer* type_info_getMethodInfo(NetTypeInfoContainer* container, int index) {
    QSharedPointer<NetMethodInfo> methodInfo = container->netTypeInfo->getMethodInfo(index);
    if(methodInfo == nullptr) {
        return nullptr;
    }
    NetMethodInfoContainer* result = new NetMethodInfoContainer();
    result->method = methodInfo;
    return result;
}

Q_DECL_EXPORT int type_info_getLocalMethodCount(NetTypeInfoContainer* container)
{
    return container->netTypeInfo->getLocalMethodCount();
}

Q_DECL_EXPORT NetMethodInfoContainer* type_info_getLocalMethodInfo(NetTypeInfoContainer* container, int index) {
    QSharedPointer<NetMethodInfo> methodInfo = container->netTypeInfo->getLocalMethodInfo(index);
    if(methodInfo == nullptr) {
        return nullptr;
    }
    NetMethodInfoContainer* result = new NetMethodInfoContainer();
    result->method = methodInfo;
    return result;
}

Q_DECL_EXPORT int type_info_getStaticMethodCount(NetTypeInfoContainer* container)
{
    return container->netTypeInfo->getStaticMethodCount();
}

Q_DECL_EXPORT NetMethodInfoContainer* type_info_getStaticMethodInfo(NetTypeInfoContainer* container, int index) {
    QSharedPointer<NetMethodInfo> methodInfo = container->netTypeInfo->getStaticMethodInfo(index);
    if(methodInfo == nullptr) {
        return nullptr;
    }
    NetMethodInfoContainer* result = new NetMethodInfoContainer();
    result->method = methodInfo;
    return result;
}

Q_DECL_EXPORT void type_info_addProperty(NetTypeInfoContainer* container, NetPropertyInfoContainer* propertyContainer) {
    container->netTypeInfo->addProperty(propertyContainer->property);
}

Q_DECL_EXPORT int type_info_getPropertyCount(NetTypeInfoContainer* container) {
    return container->netTypeInfo->getPropertyCount();
}

Q_DECL_EXPORT NetPropertyInfoContainer* type_info_getProperty(NetTypeInfoContainer* container, int index) {
    QSharedPointer<NetPropertyInfo> property = container->netTypeInfo->getProperty(index);
    if(property == nullptr) {
        return nullptr;
    }
    NetPropertyInfoContainer* result = new NetPropertyInfoContainer();
    result->property = property;
    return result;
}

Q_DECL_EXPORT void type_info_addSignal(NetTypeInfoContainer* container, NetSignalInfoContainer* signalContainer) {
    container->netTypeInfo->addSignal(signalContainer->signal);
}

Q_DECL_EXPORT int type_info_getSignalCount(NetTypeInfoContainer* container) {
    return container->netTypeInfo->getSignalCount();
}

Q_DECL_EXPORT NetSignalInfoContainer* type_info_getSignal(NetTypeInfoContainer* container, int index) {
    QSharedPointer<NetSignalInfo> signal = container->netTypeInfo->getSignal(index);
    if(signal == nullptr) {
        return nullptr;
    }
    NetSignalInfoContainer* result = new NetSignalInfoContainer();
    result->signal = signal;
    return result;
}

Q_DECL_EXPORT uchar type_info_isLoaded(NetTypeInfoContainer* container) {
    if(container->netTypeInfo->isLoaded()) {
        return 1;
    } else {
        return 0;
    }
}

Q_DECL_EXPORT uchar type_info_isLoading(NetTypeInfoContainer* container) {
    if(container->netTypeInfo->isLoading()) {
        return 1;
    } else {
        return 0;
    }
}

Q_DECL_EXPORT void type_info_ensureLoaded(NetTypeInfoContainer* container) {
    container->netTypeInfo->ensureLoaded();
}

}
