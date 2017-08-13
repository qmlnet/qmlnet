#ifndef NET_INSTANCE_H
#define NET_INSTANCE_H

class NetInstance
{
public:
    NetInstance();
    void SetValue(void* value);
    void* GetValue();
private:
    void* value;
};

#endif // NET_INSTANCE_H
