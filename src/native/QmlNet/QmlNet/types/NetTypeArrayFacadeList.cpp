#include <QmlNet/types/NetTypeArrayFacadeList.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QmlNet/types/Callbacks.h>

NetTypeArrayFacade_List::NetTypeArrayFacade_List(const QSharedPointer<NetTypeInfo>& type) :
    _isIncomplete(false)
{
    for(int x = 0; x < type->getPropertyCount(); x++) {
        QSharedPointer<NetPropertyInfo> property = type->getProperty(x);
        if(property->getPropertyName().compare("Count") == 0) {
            _lengthProperty = property;
        } else if(property->getPropertyName().compare("Item") == 0) {
            _itemProperty = property;
        }
    }

    for(int x = 0; x < type->getLocalMethodCount(); x++) {
        QSharedPointer<NetMethodInfo> method = type->getLocalMethodInfo(x);
        if(method->getMethodName().compare("RemoveAt") == 0) {
            _removeAtMethod = method;
        } else if(method->getMethodName().compare("Add") == 0) {
            _addMethod = method;
        }
    }

    if(_lengthProperty == nullptr ||
        _itemProperty == nullptr ||
        _removeAtMethod == nullptr ||
        _addMethod == nullptr) {
        _isIncomplete = true;
        return;
    }
}

bool NetTypeArrayFacade_List::isIncomplete()
{
    return _isIncomplete;
}

bool NetTypeArrayFacade_List::isFixed()
{
    return false;
}

uint NetTypeArrayFacade_List::getLength(const QSharedPointer<NetReference>& reference)
{
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    QmlNet::readProperty(_lengthProperty, reference, nullptr, result);
    return static_cast<uint>(result->getInt());
}

QSharedPointer<NetVariant> NetTypeArrayFacade_List::getIndexed(const QSharedPointer<NetReference>& reference, uint index)
{
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    QSharedPointer<NetVariant> indexParameter = QSharedPointer<NetVariant>(new NetVariant());
    indexParameter->setInt(qint32(index));
    QmlNet::readProperty(_itemProperty, reference, indexParameter, result);
    return result;
}

void NetTypeArrayFacade_List::setIndexed(const QSharedPointer<NetReference>& reference, uint index, const QSharedPointer<NetVariant>& value)
{
    QSharedPointer<NetVariant> indexParameter = QSharedPointer<NetVariant>(new NetVariant());
    indexParameter->setInt(qint32(index));
    QmlNet::writeProperty(_itemProperty, reference, indexParameter, value);
}

void NetTypeArrayFacade_List::push(const QSharedPointer<NetReference>& reference, const QSharedPointer<NetVariant>& value)
{
    QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());
    parameters->add(value);
    QmlNet::invokeNetMethod(_addMethod, reference, parameters, nullptr);
}

QSharedPointer<NetVariant> NetTypeArrayFacade_List::pop(const QSharedPointer<NetReference>& reference)
{
    uint length = getLength(reference);
    QSharedPointer<NetVariant> item = getIndexed(reference, length - 1);
    deleteAt(reference, length - 1);
    return item;
}

void NetTypeArrayFacade_List::deleteAt(const QSharedPointer<NetReference>& reference, uint index)
{
    QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());
    QSharedPointer<NetVariant> parameter = QSharedPointer<NetVariant>(new NetVariant());
    parameter->setInt(static_cast<int>(index));
    parameters->add(parameter);
    QmlNet::invokeNetMethod(_removeAtMethod, reference, parameters, nullptr);
}
