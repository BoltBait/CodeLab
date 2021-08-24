#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace PaintDotNet.Effects
{
    internal sealed class ScriptAssemblyLoadContext : AssemblyLoadContext
    {
        public ScriptAssemblyLoadContext(string? name, bool isCollectible = true)
            : base(name, isCollectible)
        {
        }

        public void NotifyAssemblyLoaded(Assembly assembly)
        {
            SetAssemblyLoadContext(assembly, this);
        }

        // It's important to "staple" the load context onto the Assembly so that the load context doesn't get garbage collected
        // It is still collectible, but only once all instances from Types in the Assembly have been collected.
        // NOTE: I'm not 100% sure this is true/necessary, it's just something I saw while debugging some nasty stuff in 
        //       Paint.NET 4.3. --Rick Brewster
        private static readonly ConditionalWeakTable<Assembly, ScriptAssemblyLoadContext> tagTable = new();

        private static void SetAssemblyLoadContext(Assembly assembly, ScriptAssemblyLoadContext loadContext)
        {
            tagTable.AddOrUpdate(assembly, loadContext);
        }

        private static bool TryGetAssemblyLoadContext(
            Assembly assembly, 
            [NotNullWhen(true)] out ScriptAssemblyLoadContext? loadContext)
        {
            return tagTable.TryGetValue(assembly, out loadContext);
        }
    }
}
