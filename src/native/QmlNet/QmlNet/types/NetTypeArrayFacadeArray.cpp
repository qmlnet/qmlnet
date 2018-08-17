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

    if(_lengthProperty == nullptr) {
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
