#include "NetVariantList.h"

NetVariantList::NetVariantList()
{

}

NetVariantList::~NetVariantList()
{

}

extern "C" {

NetVariantListContainer* net_variant_list_create() {
    NetVariantListContainer* result = new NetVariantListContainer();
    result->list = QSharedPointer<NetVariantList>(new NetVariantList());
    return result;
}

void net_variant_list_destroy(NetVariantListContainer* container) {
    delete container;
}

}
