#nullable enable

using System;
using System.Runtime.Loader;

namespace PaintDotNet.Effects
{
    internal sealed class ScriptAssemblyLoadContext : AssemblyLoadContext
    {
        public ScriptAssemblyLoadContext(string? name, bool isCollectible = true)
            : base(name, isCollectible)
        {
        }
    }
}
