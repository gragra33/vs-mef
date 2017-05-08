// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.VisualStudio.Composition.Reflection
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Manages the creation of a cache assembly.
    /// </summary>
    internal class CacheAssemblyBuilder
    {
        /// <summary>
        /// The assembly builder that is constructing the dynamic assembly.
        /// </summary>
        private readonly AssemblyBuilder assemblyBuilder;

        /// <summary>
        /// The module builder for the default module of the <see cref="assemblyBuilder"/>.
        /// This is where the special attribute will be defined.
        /// </summary>
        private readonly ModuleBuilder moduleBuilder;

        /// <summary>
        /// The directory that will contain the cache files.
        /// </summary>
        private readonly string cacheDirectory;

        /// <summary>
        /// Tracks adding skip visibility check attributes to the dynamic assembly.
        /// </summary>
        private readonly SkipClrVisibilityChecks skipVisibilityChecks;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheAssemblyBuilder"/> class.
        /// </summary>
        /// <param name="cacheDirectory">The path to the directory in which the cache assembly should be placed.</param>
        /// <param name="name">The name of the cached assembly (without the file extension).</param>
        internal CacheAssemblyBuilder(string cacheDirectory, string name)
        {
            Requires.NotNullOrEmpty(cacheDirectory, nameof(cacheDirectory));

            this.cacheDirectory = Path.GetFullPath(cacheDirectory); // resolve to absolute path immediately.

            this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(name),
                AssemblyBuilderAccess.RunAndSave,
                this.cacheDirectory);
            string moduleName = name + ".dll";
            this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(
                moduleName, moduleName);
            this.skipVisibilityChecks = new SkipClrVisibilityChecks(this.assemblyBuilder, this.moduleBuilder);
        }

        /// <summary>
        /// Saves the dynamic assembly to disk.
        /// </summary>
        /// <returns>The absolute path to the serialized assembly.</returns>
        internal string Save()
        {
            Directory.CreateDirectory(this.cacheDirectory);
            string fileName = this.assemblyBuilder.GetName().Name + ".dll";
            this.assemblyBuilder.Save(fileName);
            return Path.Combine(this.cacheDirectory, fileName);
        }
    }
}
