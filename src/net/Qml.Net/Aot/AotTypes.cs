using System;
using System.Collections.Generic;

namespace Qml.Net.Aot
{
    public static class AotTypes
    {
        private static Dictionary<int, Type> _aotTypes = new Dictionary<int, Type>();
        private static Dictionary<Type, int> _aotTypesReversed = new Dictionary<Type, int>();
        
        public static void AddAotType(int aotTypeId, Type type)
        {
            lock (_aotTypes)
            {
                if (_aotTypes.ContainsKey(aotTypeId))
                {
                    throw new Exception("Already registered aot type id");
                }

                _aotTypes[aotTypeId] = type;
                _aotTypesReversed[type] = aotTypeId;
            }
        }

        public static bool TryGetAotTypeId(Type type, out int aotTypeId)
        {
            lock (_aotTypes)
            {
                return _aotTypesReversed.TryGetValue(type, out aotTypeId);
            }
        }
    }
}