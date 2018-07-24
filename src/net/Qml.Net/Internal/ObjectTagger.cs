using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qml.Net.Internal
{
    internal class ObjectId : IDisposable
    {
        private ObjectTagger _Tagger;

        internal ObjectId(ObjectTagger tagger)
            : this(0, tagger)
        {
        }

        internal ObjectId(UInt64 id, ObjectTagger tagger)
        {
            Id = id;
            _Tagger = tagger;
        }

        internal UInt64 Id { get; private set; }

        public static implicit operator UInt64(ObjectId oId)
        {
            return oId.Id;
        }

        public void Dispose()
        {
            if (_Tagger != null)
            {
                _Tagger.FreeId(Id);
            }
        }

        ~ObjectId()
        {
            if(_Tagger != null)
            {
                _Tagger.FreeId(Id);
            }
        }
    }

    internal class ObjectTagger
    {
        internal static ObjectTagger Default { get; private set; } = new ObjectTagger();

        private readonly ConditionalWeakTable<object, ObjectId> ObjectIdRefs = new ConditionalWeakTable<object, ObjectId>();
        private UInt64 _MaxId = UInt64.MaxValue - 1;

        internal ObjectTagger(UInt64 maxId = UInt64.MaxValue - 1)
        {
            _MaxId = maxId;
        }

        internal UInt64 GetOrCreateTag(object obj)
        {
            var result = GetTag(obj);
            if (result.HasValue)
            {
                return result.Value;
            }
            var newObjId = CreateNewObjectId();
            ObjectIdRefs.Add(obj, newObjId);
            return newObjId;
        }

        internal UInt64? GetTag(object obj)
        {
            if (ObjectIdRefs.TryGetValue(obj, out var objId))
            {
                return objId;
            }
            return null;
        }

        private ObjectId CreateNewObjectId()
        {
            return new ObjectId(TakeNextFreeId(), this);
        }

        #region Id management

        private UInt64 NextId = 1;
        private HashSet<UInt64> UsedIds = new HashSet<UInt64>();

        private UInt64 TakeNextFreeId()
        {
            lock (this)
            {
                UInt64 nextId = NextId;
                UsedIds.Add(nextId);
                NextId = CalculateNextFreeId(nextId);
                return nextId;
            }
        }

        private UInt64 CalculateNextFreeId(UInt64 nextId)
        {
            bool firstPass = true;
            while (UsedIds.Contains(nextId))
            {
                if (nextId >= _MaxId)
                {
                    if (!firstPass)
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

        internal void FreeId(UInt64 id)
        {
            lock (this)
            {
                UsedIds.Remove(id);
            }
        }

        #endregion
    }

    internal static class ObjectTaggerExtension
    {
        public static UInt64 GetOrCreateTag(this object obj)
        {
            return ObjectTagger.Default.GetOrCreateTag(obj);
        }

        public static UInt64? GetTag(this object obj)
        {
            return ObjectTagger.Default.GetTag(obj);
        }
    }
}
