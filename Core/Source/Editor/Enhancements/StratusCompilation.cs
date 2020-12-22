using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace Stratus.Editor
{
    [InitializeOnLoad]
    public static class StratusCompilation
    {
        static StratusCompilation()
        {
            CompilationPipeline.compilationFinished += OnAssemblyCompilationFinished;
            AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
        }

        private static void OnAssemblyReload()
        {
        }

        private static void OnAssemblyCompilationFinished(object obj)
        {
            
        }
    }

}