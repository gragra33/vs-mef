﻿// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.VisualStudio.Composition.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TypeDesc
    {
        public TypeDesc(TypeRef type, string fullName)
        {
            this.Type = type;
            this.FullName = fullName;
        }

        public TypeRef Type { get; private set; }

        public string FullName { get; private set; }

        public static TypeDesc Get(Type type, Resolver resolver)
        {
            Requires.NotNull(type, nameof(type));
            Requires.NotNull(resolver, nameof(resolver));

            return new TypeDesc(TypeRef.Get(type, resolver), type.FullName);
        }
    }
}
