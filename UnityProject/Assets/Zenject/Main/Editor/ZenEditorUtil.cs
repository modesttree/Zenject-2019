using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using UnityEditor;
using UnityEngine;

namespace ModestTree.Zenject
{
    public static class ZenEditorUtil
    {
        public static DiContainer GetContainerForCurrentScene()
        {
            var roots = GameObject.FindObjectsOfType<CompositionRoot>();

            if (roots.IsEmpty())
            {
                throw new ZenjectException(
                    "Unable to find CompositionRoot in current scene.");
            }

            if (roots.Length > 1)
            {
                throw new ZenjectException(
                    "Found multiple CompositionRoot objects.  Not sure which one to use");
            }

            return roots[0].Container;
        }

        public static void OutputObjectGraphForCurrentScene(
            DiContainer container, IEnumerable<Type> ignoreTypes, IEnumerable<Type> contractTypes)
        {
            string dotFilePath = EditorUtility.SaveFilePanel("Choose the path to export the object graph", "", "ObjectGraph", "dot");

            if (!dotFilePath.IsEmpty())
            {
                ObjectGraphVisualizer.OutputObjectGraphToFile(
                    container, dotFilePath, ignoreTypes, contractTypes);

                var dotExecPath = EditorPrefs.GetString("Zenject.GraphVizDotExePath", "");

                if (dotExecPath.IsEmpty() || !File.Exists(dotExecPath))
                {
                    EditorUtility.DisplayDialog(
                        "GraphViz", "Unable to locate GraphViz.  Please select the graphviz 'dot.exe' file which can be found at [GraphVizInstallDirectory]/bin/dot.exe.  If you do not have GraphViz you can download it at http://www.graphviz.org", "Ok");

                    dotExecPath = EditorUtility.OpenFilePanel("Please select dot.exe from GraphViz bin directory", "", "exe");

                    EditorPrefs.SetString("Zenject.GraphVizDotExePath", dotExecPath);
                }

                if (!dotExecPath.IsEmpty())
                {
                    RunDotExe(dotExecPath, dotFilePath);
                }
            }
        }

        static void RunDotExe(string dotExePath, string dotFileInputPath)
        {
            var outputDir = Path.GetDirectoryName(dotFileInputPath);
            var fileBaseName = Path.GetFileNameWithoutExtension(dotFileInputPath);

            var proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = dotExePath;
            proc.StartInfo.Arguments = "-Tpng {0}.dot -o{0}.png".With(fileBaseName);
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WorkingDirectory = outputDir;

            proc.Start();
            proc.WaitForExit();

            var errorMessage = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (errorMessage.IsEmpty())
            {
                EditorUtility.DisplayDialog(
                    "Success!", "Successfully created files {0}.dot and {0}.png".With(fileBaseName), "Ok");
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Error", "Error occurred while generating {0}.png".With(fileBaseName), "Ok");

                Log.Error("Zenject error: Failure during object graph creation: " + errorMessage);

                // Do we care about STDOUT?
                //var outputMessage = proc.StandardOutput.ReadToEnd();
                //Log.Error("outputMessage = " + outputMessage);
            }

        }
    }
}
