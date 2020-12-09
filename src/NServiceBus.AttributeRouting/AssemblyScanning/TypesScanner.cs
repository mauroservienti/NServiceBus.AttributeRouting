using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NServiceBus.AttributeRouting.AssemblyScanning
{
    class TypesScanner
    {
        public static IEnumerable<Type> ScanMessageTypes(AssemblyScannerConfiguration coreScannerConfiguration, Conventions conventions)
        {
            var excludedTypes = GetExcludedTypes(coreScannerConfiguration).ToImmutableHashSet();
            var excludedAssemblies = GetExcludedAssemblies(coreScannerConfiguration).ToImmutableHashSet();

            var scanner = new InternalAssemblyScanner();
            scanner.AddAssemblyFilter(fullPath=> excludedAssemblies.Contains(Path.GetFileName(fullPath))
                ? InternalAssemblyScanner.FilterResults.Exclude
                : InternalAssemblyScanner.FilterResults.Include);

            var assemblies = scanner.Scan().ToList();
            var messageTypes = assemblies.SelectMany(a=>a.GetTypes())
                .Where(t => !excludedTypes.Contains(t) && (conventions.IsCommandType(t) || conventions.IsMessageType(t)))
                .ToList();

            return messageTypes;
        }

        static List<Type> GetExcludedTypes(AssemblyScannerConfiguration configuration)
        {
            var property = typeof(AssemblyScannerConfiguration).GetProperty("ExcludedTypes",
                BindingFlags.Instance | BindingFlags.NonPublic);

            return (List<Type>)property?.GetValue(configuration, null);
        }

        static List<string> GetExcludedAssemblies(AssemblyScannerConfiguration configuration)
        {
            var property = typeof(AssemblyScannerConfiguration).GetProperty("ExcludedAssemblies",
                BindingFlags.Instance | BindingFlags.NonPublic);

            return (List<string>)property?.GetValue(configuration, null);
        }
    }
}