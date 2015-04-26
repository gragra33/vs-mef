﻿// This file originated from System.ComponentModel.Composition.dll

namespace Microsoft.VisualStudio.Composition
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    internal static partial class CollectionServices
    {
        private static readonly ConstructorInfo CollectionOfObjectCtor = typeof(CollectionOfObject<>).GetConstructors()[0];
        private static readonly Dictionary<Type, Func<object, ICollection<object>>> CachedCollectionWrapperFactories = new Dictionary<Type, Func<object, ICollection<object>>>();

        internal static ICollection<object> GetCollectionWrapper(Type itemType, object collectionObject)
        {
            Requires.NotNull(itemType, "itemType");
            Requires.NotNull(collectionObject, "collectionObject");

            var underlyingItemType = itemType.UnderlyingSystemType;

            if (underlyingItemType == typeof(object))
            {
                return (ICollection<object>)collectionObject;
            }

            // Most common .Net collections implement IList as well so for those
            // cases we can optimize the wrapping instead of using reflection to create
            // a generic type.
            if (typeof(IList).IsAssignableFrom(collectionObject.GetType()))
            {
                return new CollectionOfObjectList((IList)collectionObject);
            }

            Func<object, ICollection<object>> factory;
            lock (CachedCollectionWrapperFactories)
            {
                CachedCollectionWrapperFactories.TryGetValue(underlyingItemType, out factory);
            }

            if (factory == null)
            {
                Type collectionType = typeof(CollectionOfObject<>).MakeGenericType(underlyingItemType);
                var ctor = (ConstructorInfo)MethodBase.GetMethodFromHandle(CollectionOfObjectCtor.MethodHandle, collectionType.TypeHandle);

                factory = collection =>
                {
                    using (var args = ArrayRental<object>.Get(1))
                    {
                        args.Value[0] = collection;
                        return (ICollection<object>)ctor.Invoke(args.Value);
                    }
                };

                lock (CachedCollectionWrapperFactories)
                {
                    CachedCollectionWrapperFactories[underlyingItemType] = factory;
                }
            }

            return factory(collectionObject);
        }

        private class CollectionOfObjectList : ICollection<object>
        {
            private readonly IList list;

            public CollectionOfObjectList(IList list)
            {
                this.list = list;
            }

            public int Count
            {
                get { throw Assumes.NotReachable(); }
            }

            public bool IsReadOnly
            {
                get { return this.list.IsReadOnly; }
            }

            public void Add(object item)
            {
                this.list.Add(item);
            }

            public void Clear()
            {
                this.list.Clear();
            }

            public bool Contains(object item)
            {
                throw Assumes.NotReachable();
            }

            public void CopyTo(object[] array, int arrayIndex)
            {
                throw Assumes.NotReachable();
            }

            public bool Remove(object item)
            {
                throw Assumes.NotReachable();
            }

            public IEnumerator<object> GetEnumerator()
            {
                throw Assumes.NotReachable();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw Assumes.NotReachable();
            }
        }

        private class CollectionOfObject<T> : ICollection<object>
        {
            private readonly ICollection<T> collectionOfT;

            public CollectionOfObject(object collectionOfT)
            {
                this.collectionOfT = (ICollection<T>)collectionOfT;
            }

            public int Count
            {
                get { throw Assumes.NotReachable(); }
            }

            public bool IsReadOnly
            {
                get { return this.collectionOfT.IsReadOnly; }
            }

            public void Add(object item)
            {
                this.collectionOfT.Add((T)item);
            }

            public void Clear()
            {
                this.collectionOfT.Clear();
            }

            public bool Contains(object item)
            {
                throw Assumes.NotReachable();
            }

            public void CopyTo(object[] array, int arrayIndex)
            {
                throw Assumes.NotReachable();
            }

            public bool Remove(object item)
            {
                throw Assumes.NotReachable();
            }

            public IEnumerator<object> GetEnumerator()
            {
                throw Assumes.NotReachable();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw Assumes.NotReachable();
            }
        }
    }
}
