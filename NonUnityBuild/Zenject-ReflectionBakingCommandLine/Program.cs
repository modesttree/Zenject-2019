using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zenject.ReflectionBaking.Mono.Cecil;
using Zenject.ReflectionBaking.Mono.Collections.Generic;

namespace Zenject.ReflectionBaking
{
    public class Program
    {
        static int Main(string[] args)
        {
            List<string> namespacePatterns;
            List<string> searchDirectories;

            if (!TryParseArgs(args, out namespacePatterns, out searchDirectories))
            {
                Console.WriteLine("Invalid Arguments specified.  Usage: {0} -d \"DLL Search Directory 1\" -d \"DLL Search Directory 2\" -p \"Namespace Regex Pattern 1\" -p \"Namespace Regex Pattern 2\"...", System.AppDomain.CurrentDomain.FriendlyName);
                return 1;
            }

            var resolver = new DefaultAssemblyResolver();

            foreach (var path in searchDirectories)
            {
                resolver.AddSearchDirectory(path);
            }

            var readerParams = new ReaderParameters()
            {
                AssemblyResolver = resolver,
            };

            var writerParams = new WriterParameters()
            {
            };

            foreach (var assemblyPath in searchDirectories.SelectMany(path => Directory.GetFiles(path, "*.dll")))
            {
                var module = ModuleDefinition.ReadModule(assemblyPath, readerParams);

                var assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));

                int numChanges = ReflectionBakingModuleEditor.WeaveAssembly(
                    module, assembly, namespacePatterns);

                if (numChanges > 0)
                {
                    module.Write(assemblyPath, writerParams);
                    Console.WriteLine("Changed {0} types in assembly '{1}'", numChanges, module.Name);
                }
            }

            return 0;
        }

        static bool TryParseArgs(string[] args, out List<string> patterns, out List<string> directories)
        {
            bool patternQueued = false;
            bool directoryQueued = false;

            patterns = new List<string>();
            directories = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-p")
                {
                    if (patternQueued || directoryQueued)
                    {
                        return false;
                    }

                    patternQueued = true;
                }
                else if (args[i] == "-d")
                {
                    if (patternQueued || directoryQueued)
                    {
                        return false;
                    }

                    directoryQueued = true;
                }
                else
                {
                    if (patternQueued)
                    {
                        patterns.Add(args[i]);
                        patternQueued = false;
                    }
                    else if (directoryQueued)
                    {
                        directories.Add(args[i]);
                        directoryQueued = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return !patternQueued && !directoryQueued && directories.Any();
        }
    }
}
