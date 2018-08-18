#include <QmlNet/types/NetTypeArrayFacadeList.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QmlNet/types/Callbacks.h>

NetTypeArrayFacade_List::NetTypeArrayFacade_List(QSharedPointer<NetTypeInfo> type) :
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
        }
    }

    if(_lengthProperty == nullptr ||
        _itemProperty == nullptr ||
        _removeAtMethod == nullptr) {
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

uint NetTypeArrayFacade_List::getLength(QSharedPointer<NetReference> reference)
{
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    readProperty(_lengthProperty, reference, nullptr, result);
    return static_cast<uint>(result->getInt());
}

QSharedPointer<NetVariant> NetTypeArrayFacade_List::getIndexed(QSharedPointer<NetReference> reference, uint index)
{
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    QSharedPointer<NetVariant> indexParameter = QSharedPointer<NetVariant>(new NetVariant());
    indexParameter->setInt(index);
    readProperty(_itemProperty, reference, indexParameter, result);
    return result;
}

void NetTypeArrayFacade_List::setIndexed(QSharedPointer<NetReference> reference, uint index, QSharedPointer<NetVariant> value)
{
    QSharedPointer<NetVariant> indexParameter = QSharedPointer<NetVariant>(new NetVariant());
    indexParameter->setInt(static_cast<int>(index));
    writeProperty(_itemProperty, reference, indexParameter, value);
}

QSharedPointer<NetVariant> NetTypeArrayFacade_List::pop(QSharedPointer<NetReference> reference)
{
    uint length = getLength(reference);
    QSharedPointer<NetVariant> item = getIndexed(reference, length - 1);
    deleteAt(reference, length - 1);
    return item;
}

void NetTypeArrayFacade_List::deleteAt(QSharedPointer<NetReference> reference, uint index)
{
    QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());
    QSharedPointer<NetVariant> parameter = QSharedPointer<NetVariant>(new NetVariant());
    parameter->setInt(static_cast<int>(index));
    parameters->add(parameter);
    invokeNetMethod(_removeAtMethod, reference, parameters, nullptr);
}
