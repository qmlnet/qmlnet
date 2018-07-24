using Qml.Net.Internal.Types;
using System;
using System.Collections.Generic;
using System.Text;

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
