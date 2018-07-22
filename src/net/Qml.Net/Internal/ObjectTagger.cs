using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qml.Net.Internal
{
    internal class ObjectId : IDisposable
    {
        public ObjectId()
            : this(0)
        {
        }

        private ObjectId(UInt64 id)
        {
            Id = id;
        }

        public UInt64 Id { get; private set; }

        public static implicit operator UInt64(ObjectId oId)
        {
            return oId.Id;
        }

        public static ObjectId CreateNew()
        {
            return new ObjectId(TakeNextFreeId());
        }

        #region Id management

        //set in unit tests
        private static UInt64 MaxId = UInt64.MaxValue - 1;

        //used in unit tests
        private static void Reset()
        {
            MaxId = UInt64.MaxValue - 1;
            NextId = 1;
            UsedIds.Clear();
        }

        private static UInt64 NextId = 1;
        private static HashSet<UInt64> UsedIds = new HashSet<UInt64>();

        private static UInt64 TakeNextFreeId()
        {
            UInt64 nextId = NextId;
            UsedIds.Add(nextId);
            NextId = CalculateNextFreeId(nextId);
            return nextId;
        }

        private static UInt64 CalculateNextFreeId(UInt64 nextId)
        {
            bool firstPass = true;
            while (UsedIds.Contains(nextId))
            {
                if(nextId >= MaxId)
                {
                    if(!firstPass)
                    {
                        throw new Exception("Too many object ids in use!");
                    }
                    nextId = 1;
                    firstPass = false;
                }
                else
                {
                    nextId++;
                }
            }
            return nextId;
        }

        private static void FreeId(UInt64 id)
        {
            UsedIds.Remove(id);
        }

        public void Dispose()
        {
            FreeId(Id);
        }

        ~ObjectId()
        {
            FreeId(Id);
        }

        #endregion
    }

    internal static class ObjectTagger
    {
        private static readonly ConditionalWeakTable<object, ObjectId> ObjectIdRefs = new ConditionalWeakTable<object, ObjectId>();

        public static UInt64 GetOrCreateTag(this object obj)
        {
            var result = GetTag(obj);
            if(result.HasValue)
            {
                return result.Value;
            }
            var newObjId = ObjectId.CreateNew();
            ObjectIdRefs.Add(obj, newObjId);
            return newObjId;
        }

        public static UInt64? GetTag(this object obj)
        {
            if (ObjectIdRefs.TryGetValue(obj, out var objId))
            {
                return objId;
            }
            return null;
        }
    }
}
