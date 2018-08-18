#include <QmlNet/types/NetTypeArrayFacade.h>
#include <QmlNet/types/NetTypeArrayFacadeArray.h>
#include <QmlNet/types/NetTypeInfo.h>

NetTypeArrayFacade::NetTypeArrayFacade()
{

}

QSharedPointer<NetTypeArrayFacade> NetTypeArrayFacade::fromType(QSharedPointer<NetTypeInfo> type)
{
    if(type->isArray()) {
        QSharedPointer<NetTypeArrayFacade_Array> facade = QSharedPointer<NetTypeArrayFacade_Array>(new NetTypeArrayFacade_Array(type));
        if(facade->isIncomplete()) {
            return nullptr;
        }
        return facade.dynamicCast<NetTypeArrayFacade>();
    }

    return nullptr;
}


uint NetTypeArrayFacade::getLength(QSharedPointer<NetReference>)
{
    return 0;
}

QSharedPointer<NetVariant> NetTypeArrayFacade::getIndexed(QSharedPointer<NetReference>, int)
{
    return nullptr;
}

void NetTypeArrayFacade::setIndexed(QSharedPointer<NetReference>, int, QSharedPointer<NetVariant>)
{

}
