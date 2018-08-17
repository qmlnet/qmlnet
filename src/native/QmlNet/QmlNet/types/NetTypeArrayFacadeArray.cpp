#include <QmlNet/types/NetTypeArrayFacadeArray.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QmlNet/types/Callbacks.h>

NetTypeArrayFacade_Array::NetTypeArrayFacade_Array(QSharedPointer<NetTypeInfo> type) :
    _isIncomplete(false)
{
    for(int x = 0; x < type->getPropertyCount(); x++) {
        QSharedPointer<NetPropertyInfo> property = type->getProperty(x);
        if(property->getPropertyName().compare("Length") == 0) {
            _lengthProperty = property;
        }
    }

    for(int x = 0; x < type->getMethodCount(); x++) {
        QSharedPointer<NetMethodInfo> method = type->getMethodInfo(x);
        if(method->getMethodName().compare("Get") == 0) {
            _getIndexed = method;
        } else if(method->getMethodName().compare("Set") == 0) {
            _setIndexed = method;
        }
    }

    if(_lengthProperty == nullptr ||
        _getIndexed == nullptr ||
        _setIndexed == nullptr) {
        _isIncomplete = true;
        return;
    }
}

bool NetTypeArrayFacade_Array::isIncomplete()
{
    return _isIncomplete;
}

int NetTypeArrayFacade_Array::getLength(QSharedPointer<NetReference> reference)
{
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    readProperty(_lengthProperty, reference, result);
    return result->getInt();
}

QSharedPointer<NetVariant> NetTypeArrayFacade_Array::getIndexed(QSharedPointer<NetReference> reference, int index)
{
    QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());
    QSharedPointer<NetVariant> parameter = QSharedPointer<NetVariant>(new NetVariant());
    parameter->setInt(static_cast<int>(index));
    parameters->add(parameter);
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    invokeNetMethod(_getIndexed, reference, parameters, result);
    return result;
}

void NetTypeArrayFacade_Array::setIndexed(QSharedPointer<NetReference> reference, int index, QSharedPointer<NetVariant> value)
{
    QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());
    QSharedPointer<NetVariant> parameter = QSharedPointer<NetVariant>(new NetVariant());
    parameter->setInt(static_cast<int>(index));
    parameters->add(parameter);
    parameters->add(value);
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    invokeNetMethod(_setIndexed, reference, parameters, result);
}
