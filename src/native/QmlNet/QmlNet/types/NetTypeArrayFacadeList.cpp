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

    if(_lengthProperty == nullptr ||
        _itemProperty == nullptr) {
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

QSharedPointer<NetVariant> NetTypeArrayFacade_List::getIndexed(QSharedPointer<NetReference> reference, int index)
{
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    QSharedPointer<NetVariant> indexParameter = QSharedPointer<NetVariant>(new NetVariant());
    indexParameter->setInt(index);
    readProperty(_itemProperty, reference, indexParameter, result);
    return result;
}

void NetTypeArrayFacade_List::setIndexed(QSharedPointer<NetReference> reference, int index, QSharedPointer<NetVariant> value)
{
    QSharedPointer<NetVariant> indexParameter = QSharedPointer<NetVariant>(new NetVariant());
    indexParameter->setInt(index);
    writeProperty(_itemProperty, reference, indexParameter, value);
}
