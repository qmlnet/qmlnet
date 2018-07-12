#include "net_qml_register_type.h"
#include "net_qml_value_type.h"
#include "qtestobject.h"
#include <QQmlApplicationEngine>
#include <map>

#define NET_TYPE_SLOT_COUNT 30

class ITypeRegistrationSlot {
public:
    virtual int qmlRegister(NetTypeInfo* typeInfo, QString &uri, int versionMajor, int versionMinor, QString& qmlName) = 0;
};

template <int N>
class TypeRegistrationSlotInfo : public virtual ITypeRegistrationSlot {
public:
    int qmlRegister(NetTypeInfo* typeInfo, QString &uri, int versionMajor, int versionMinor, QString& qmlName) override {
        NetValueType<N>::init(typeInfo);
        return qmlRegisterType<NetValueType<N>>(uri.toUtf8().data(), versionMajor, versionMinor, qmlName.toUtf8().data());
    }
};

class TypeRegistrationSlots {
public:
    template <int MaxSlotIndex>
    static void initRec() {
        initSlot<MaxSlotIndex>();
        initRec<MaxSlotIndex - 1>();
    }

    template<>
    static void initRec<0>() {
        initSlot<0>();
    }

    static ITypeRegistrationSlot* getSlot(int num) {
        return s_typeRegistrationSlots.at(num);
    }

    static int getSlotCount() {
        return s_typeRegistrationSlots.size();
    }

private:
    static std::map<int, ITypeRegistrationSlot*> s_typeRegistrationSlots;

    template <int N>
    static void initSlot() {
        s_typeRegistrationSlots[N] = new TypeRegistrationSlotInfo<N>();
    }
};

std::map<int, ITypeRegistrationSlot*> TypeRegistrationSlots::s_typeRegistrationSlots;

int s_registerCount = 0;
bool s_initialized = false;

void ensureInitialized() {
    if(s_initialized) {
        return;
    }

    TypeRegistrationSlots::initRec<NET_TYPE_SLOT_COUNT - 1>();

    s_initialized = true;
}

int registerNetType(QString &netType, QString &uri, int versionMajor, int versionMinor, QString& qmlName)
{
    ensureInitialized();
    if(!NetTypeInfoManager::isValidType(netType.toUtf8().data()))
        return -1;

    s_registerCount++;
    if(s_registerCount >= TypeRegistrationSlots::getSlotCount()) {
        return -1;
    }

    NetTypeInfo* typeInfo = NetTypeInfoManager::GetTypeInfo(netType.toUtf8().data());

    auto slot = TypeRegistrationSlots::getSlot(s_registerCount);
    return slot->qmlRegister(typeInfo, uri, versionMajor, versionMinor, qmlName);
}
