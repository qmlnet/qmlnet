#include <QmlNet/types/NetTypeArrayFacade.h>
#include <QmlNet/types/NetTypeArrayFacadeArray.h>
#include <QmlNet/types/NetTypeArrayFacadeList.h>
#include <QmlNet/types/NetTypeInfo.h>

NetTypeArrayFacade::NetTypeArrayFacade() = default;

QSharedPointer<NetTypeArrayFacade> NetTypeArrayFacade::fromType(const QSharedPointer<NetTypeInfo>& type)
{
    if(type->isArray()) {
        QSharedPointer<NetTypeArrayFacade_Array> facade = QSharedPointer<NetTypeArrayFacade_Array>(new NetTypeArrayFacade_Array(type));
        if(facade->isIncomplete()) {
            return nullptr;
        }
        return facade.staticCast<NetTypeArrayFacade>();
    }

    if(type->isList()) {
        QSharedPointer<NetTypeArrayFacade_List> facade = QSharedPointer<NetTypeArrayFacade_List>(new NetTypeArrayFacade_List(type));
        if(facade->isIncomplete()) {
            return nullptr;
        }
        return facade.staticCast<NetTypeArrayFacade>();
    }

    return nullptr;
}

bool NetTypeArrayFacade::isFixed()
{
    return false;
}

uint NetTypeArrayFacade::getLength(const QSharedPointer<NetReference>&)
{
    return 0;
}

QSharedPointer<NetVariant> NetTypeArrayFacade::getIndexed(const QSharedPointer<NetReference>&, uint)
{
    return nullptr;
}

void NetTypeArrayFacade::setIndexed(const QSharedPointer<NetReference>&, uint, const QSharedPointer<NetVariant>&)
{

}

void NetTypeArrayFacade::push(const QSharedPointer<NetReference>&, const QSharedPointer<NetVariant>&)
{

}

QSharedPointer<NetVariant> NetTypeArrayFacade::pop(const QSharedPointer<NetReference>&)
{
    return nullptr;
}

void NetTypeArrayFacade::deleteAt(const QSharedPointer<NetReference>&, uint)
{

}
