using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Spatial.Roslyn.Tests.ApiCompare
{
    public class Reflector
    {
        private readonly List<Type> lostTypes = new List<Type>();
        private readonly List<EntityChange> allChanges = new List<EntityChange>();

        public Assembly Legacy { get; set; }

        public Assembly Current { get; set; }

        public int RemovedTypes => this.lostTypes.Count;

        public int ChangedTypes => this.allChanges.Count;

        public bool LoadAssemblies(string oldassembly, string newassembly)
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(this.CurrentDomain_ReflectionOnlyAssemblyResolve);

            try
            {
                this.Legacy = Assembly.ReflectionOnlyLoadFrom(oldassembly);
                this.Current = Assembly.ReflectionOnlyLoadFrom(newassembly);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    Console.WriteLine("Failed: " + typeLoadException.Message);
                    foreach (var e in loaderExceptions)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                return false;
            }
        }

        public List<EntityChange> Analyse()
        {
            List<EntityChange> allChanges = new List<EntityChange>();
            var currentTypes = this.Legacy.GetExportedTypes();
            foreach (var oldtype in currentTypes)
            {
                if (oldtype.IsPublic)
                {
                    var newtype = this.Current.GetType(oldtype.FullName);
                    EntityChange change = new EntityChange() { OriginalType = oldtype, ReplacementType = newtype };
                    if (!change.IsRemoved)
                    {
                        change.MethodChanges.AddRange(this.GetChangedCtorList(oldtype, newtype, BindingFlags.Public | BindingFlags.DeclaredOnly));
                        change.MethodChanges.AddRange(this.GetChangedCtorList(oldtype, newtype, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance));
                        change.MethodChanges.AddRange(this.GetChangedMethodList(oldtype, newtype));
                    }

                    allChanges.Add(change);
                }
            }

            return allChanges;
        }

        private List<string> GetChangedMethodList(Type legacy, Type current)
        {
            List<string> methodsigs = new List<string>();
            MethodInfo[] legacyMethods = legacy.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            for (int i = 0; i < legacyMethods.Length; i++)
            {
                var result = this.FindNewMethod(current, legacyMethods[i]);
                if (result == null)
                {
                    methodsigs.Add("[Deleted]" + legacyMethods[i].GetSignature());
                }
                else if (result.IsDefinedReflectOnly(typeof(ObsoleteAttribute)))
                {
                    methodsigs.Add("[Obsolete]" + legacyMethods[i].GetSignature());
                }
                else if (!result.IsPublic)
                {
                    methodsigs.Add("[No Longer Public]" + legacyMethods[i].GetSignature());
                }
                else if (result.IsStatic != legacyMethods[i].IsStatic)
                {
                    methodsigs.Add("[Static Modified]" + legacyMethods[i].GetSignature());
                }
                else if (result.ReturnType.Name != legacyMethods[i].ReturnType.Name)
                {
                    methodsigs.Add("[Return Type Changed]" + legacyMethods[i].GetSignature());
                }
                else if (result.GetSignature() != legacyMethods[i].GetSignature())
                {
                    methodsigs.Add("[Optional Param Changed]" + legacyMethods[i].GetSignature());
                }
            }

            return methodsigs;
        }

        private MethodInfo FindNewMethod(Type current, MethodInfo searchMethod)
        {
            var methods = current.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            List<MethodInfo> possibleMatches = new List<MethodInfo>();
            foreach (var candidate in methods)
            {
                if (searchMethod.FullMethodName() == candidate.FullMethodName())
                {
                    // check for exact match
                    if (searchMethod.GetSignature() == candidate.GetSignature())
                    {
                        return candidate;
                    }

                    if (searchMethod.PartialMethodSig() == candidate.PartialMethodSig())
                    {
                        possibleMatches.Add(candidate);
                    }
                }
            }

            foreach (var match in possibleMatches)
            {
                if (match.IsStatic != searchMethod.IsStatic)
                {
                    continue;
                }

                return match;
            }

            return null;
        }

        private List<string> GetChangedCtorList(Type legacy, Type current, BindingFlags binding)
        {
            List<string> legacysigs = this.GetCtorSignatures(legacy, binding);
            List<string> currentsigs = this.GetCtorSignatures(current, binding);
            List<string> onlyinlegacy = legacysigs.Except(currentsigs).ToList();
            List<string> final = new List<string>();
            foreach (string search in onlyinlegacy)
            {
                if (currentsigs.Contains("[Obsolete]" + search))
                {
                    final.Add("[Obsolete]" + search);
                }
                else
                {
                    final.Add("[Deleted]" + search);
                }
            }

            return final;
        }

        private List<string> GetCtorSignatures(Type type, BindingFlags binding)
        {
            List<string> methodsigs = new List<string>();
            var methods = type.GetConstructors(binding);
            foreach (var method in methods)
            {
                methodsigs.Add(method.GetSignature());
            }

            return methodsigs;
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            // TODO: also search in the local directory where the files were
            return Assembly.ReflectionOnlyLoad(args.Name);
        }
    }
}
