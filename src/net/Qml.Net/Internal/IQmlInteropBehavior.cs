using System;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal
{
    internal interface IQmlInteropBehavior
    {
        bool IsApplicableFor(Type type);

        void OnNetTypeInfoCreated(NetTypeInfo netTypeInfo, Type forType);

        void OnObjectEntersNative(object instance, UInt64 objectId);

        void OnObjectLeavesNative(object instance, UInt64 objectId);
    }
}
