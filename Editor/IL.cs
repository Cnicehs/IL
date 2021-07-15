using ILRuntime.Mono.Cecil;
using ILRuntime.Mono.Cecil.Cil;
using System;
using System.Linq;
using UnityEngine;
public static class Run
{
    [UnityEditor.MenuItem("Inject/Run")]
    public static void Inject()
    {
        AssemblyDefinition assembly = null;
        try
        {
            var readerParameters = new ReaderParameters
            {
                ReadSymbols = true,
                InMemory = true,
            };
            string path = "./Library/ScriptAssemblies/Assembly-CSharp.dll";
            string pdb = System.IO.Path.ChangeExtension(path, ".pdb");
            if (System.IO.File.Exists(pdb))
            {
                readerParameters.SymbolReaderProvider = new PortablePdbReaderProvider();
            }
            assembly = AssemblyDefinition.ReadAssembly(path, readerParameters);
            foreach (var type in assembly.MainModule.Types)
            {
                //Logger.Log(type);
                foreach (var method in type.Methods)
                {
                    if (method.Name == "OnEnable")
                    {
                        var insertPoint = method.Body.Instructions[0];
                        var processor = method.Body.GetILProcessor();
                        //var action = assembly.MainModule.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }));
                        ////var baseIns = assembly.MainModule.ImportReference();

                        var it = typeof(EventCenter).GetProperty("Instance", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy|System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        //var Ins = assembly.MainModule.GetType("EventCenter").BaseType.Resolve().Properties.First((x) =>
                        //{
                        //    return x.Name == "Instance";
                        //});
                        var Ins = assembly.MainModule.ImportReference(it.GetGetMethod());
                        //processor.InsertBefore(insertPoint, processor.Create(OpCodes.Ldstr, "IL Inject"));
                        //processor.InsertBefore(insertPoint, processor.Create(OpCodes.Call, action)); 
                        processor.InsertBefore(insertPoint, processor.Create(OpCodes.Call, Ins));
                        processor.InsertBefore(insertPoint, processor.Create(OpCodes.Ldc_I4_0));
                        processor.InsertBefore(insertPoint, processor.Create(OpCodes.Box,assembly.MainModule.GetType("GG").GetElementType()));
                        foreach (var item in assembly.Modules)
                        {
                            Debug.LogError(item);
                        }
                        var gType = assembly.MainModule.ImportReference(typeof(GameObject).GetConstructor(new Type[] { }));
                        processor.InsertBefore(insertPoint, processor.Create(OpCodes.Newobj, gType));

                        var e = typeof(EventCenter).GetMethods().First((x) =>
                        {
                            return x.Name == "EventTrigger" && x.GetParameters()[0].ParameterType == typeof(Enum)&&x.IsGenericMethod;
                        });
                        e = e.MakeGenericMethod(typeof(GameObject));
                        var ae = assembly.MainModule.ImportReference(e);
                        processor.InsertBefore(insertPoint, processor.Create(OpCodes.Callvirt, ae));
                        //processor.InsertBefore(insertPoint, processor.Create(OpCodes.Callvirt, assembly.MainModule.GetType("UnityEngine.GameObject").GetElementType()));
                        //foreach (var item in method.Body.Instructions)
                        //{
                        //    Logger.Log(item);
                        //}
                    }
                }
            }
            var writerParameters = new WriterParameters { WriteSymbols = true };
            assembly.Write("./Library/ScriptAssemblies/Assembly-CSharp.dll", writerParameters);
        }
        catch(SystemException ex)
        {
            Debug.LogError(ex);
        }
    }
}