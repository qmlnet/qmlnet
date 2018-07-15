#include "NetVariantList.h"

NetVariantList::NetVariantList()
{

}

NetVariantList::~NetVariantList()
{

}

int NetVariantList::count() {
    return variants.size();
}

void NetVariantList::add(QSharedPointer<NetVariant> variant) {
    variants.append(variant);
}

QSharedPointer<NetVariant> NetVariantList::get(int index) {
    if(index < 0) return QSharedPointer<NetVariant>(NULL);
    if(index >= variants.length()) return QSharedPointer<NetVariant>(NULL);
    return variants.at(index);
}

void NetVariantList::remove(int index) {
    variants.removeAt(index);;
}

void NetVariantList::clear() {
    variants.clear();
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

int net_variant_list_count(NetVariantListContainer* container) {
    return container->list->count();
}

void net_variant_list_add(NetVariantListContainer* container, NetVariantContainer* variant) {
    container->list->add(variant->variant);
}

NetVariantContainer* net_variant_list_get(NetVariantListContainer* container, int index){
    QSharedPointer<NetVariant> variant = container->list->get(index);
    if(variant == NULL) return NULL;
    NetVariantContainer* result = new NetVariantContainer();
    result->variant = variant;
    return result;
}

void net_variant_list_remove(NetVariantListContainer* container, int index) {
    container->list->remove(index);
}

void net_variant_list_clear(NetVariantListContainer* container) {
    container->list->clear();
}

}
