using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qml.Net.Internal
{
    public class ObjectId : IDisposable
    {
        public ObjectId()
            : this(0)
        {
        }

        private ObjectId(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; private set; }

        public static implicit operator ulong(ObjectId oId)
        {
            return oId.Id;
        }

        public static ObjectId CreateNew()
        {
            return new ObjectId(TakeNextFreeId());
        }

        #region Id management

        private static ulong NextId = 1;
        private static HashSet<ulong> UsedIds = new HashSet<ulong>();

        private static ulong TakeNextFreeId()
        {
            ulong nextId = NextId;
            UsedIds.Add(nextId);
            NextId = CalculateNextFreeId(nextId);
            return nextId;
        }

        private static ulong CalculateNextFreeId(ulong nextId)
        {
            bool firstPass = true;
            while (UsedIds.Contains(nextId))
            {
                if(nextId == ulong.MaxValue)
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

        private static void FreeId(ulong id)
        {
            UsedIds.Remove(id);
        }

        public void Dispose()
        {
            FreeId(Id);
        }

        #endregion
    }

    public static class ObjectTagger
    {
        private static readonly ConditionalWeakTable<object, ObjectId> ObjectIdRefs = new ConditionalWeakTable<object, ObjectId>();

        public static ulong GetOrCreateTag(this object obj)
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

        public static ulong? GetTag(this object obj)
        {
            if (ObjectIdRefs.TryGetValue(obj, out var objId))
            {
                return objId;
            }
            return null;
        }
    }
}
