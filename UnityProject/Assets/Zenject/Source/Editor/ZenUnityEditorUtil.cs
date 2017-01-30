#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ModestTree;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public static class ZenUnityEditorUtil
    {
        // Don't use this
        public static void ValidateCurrentSceneSetup()
        {
            Assert.That(!ProjectContext.HasInstance);
            ProjectContext.ValidateOnNextRun = true;

            foreach (var sceneContext in GetAllSceneContexts())
            {
                try
                {
                    sceneContext.Validate();
                }
                catch (Exception e)
                {
                    // Add a bit more context
                    throw new ZenjectException(
                        "Scene '{0}' Failed Validation!".Fmt(sceneContext.gameObject.scene.name), e);
                }
            }
        }

        // Don't use this
        public static int ValidateAllActiveScenes()
        {
            var activeScenePaths = UnityEditor.EditorBuildSettings.scenes.Where(x => x.enabled)
                .Select(x => x.path).ToList();

            foreach (var scenePath in activeScenePaths)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                ValidateCurrentSceneSetup();
            }

            return activeScenePaths.Count;
        }

        // Don't use this
        public static void RunCurrentSceneSetup()
        {
            Assert.That(!ProjectContext.HasInstance);

            foreach (var sceneContext in GetAllSceneContexts())
            {
                try
                {
                    sceneContext.Run();
                }
                catch (Exception e)
                {
                    // Add a bit more context
                    throw new ZenjectException(
                        "Scene '{0}' Failed To Start!".Fmt(sceneContext.gameObject.scene.name), e);
                }
            }
        }

        static IEnumerable<SceneContext> GetAllSceneContexts()
        {
            var decoratedSceneNames = new List<string>();

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);

                var sceneContexts = scene.GetRootGameObjects()
                    .SelectMany(x => x.GetComponentsInChildren<SceneContext>()).ToList();

                var decoratorContexts = scene.GetRootGameObjects()
                    .SelectMany(x => x.GetComponentsInChildren<SceneDecoratorContext>()).ToList();

                if (!sceneContexts.IsEmpty())
                {
                    Assert.That(decoratorContexts.IsEmpty(),
                        "Found both SceneDecoratorContext and SceneContext in the same scene '{0}'.  This is not allowed", scene.name);

                    Assert.That(sceneContexts.IsLength(1),
                        "Found multiple SceneContexts in scene '{0}'.  Expected a maximum of one.", scene.name);

                    var context = sceneContexts[0];

                    decoratedSceneNames.RemoveAll(x => context.ContractNames.Contains(x));

                    yield return context;
                }
                else if (!decoratorContexts.IsEmpty())
                {
                    Assert.That(decoratorContexts.IsLength(1),
                        "Found multiple SceneDecoratorContexts in scene '{0}'.  Expected a maximum of one.", scene.name);

                    var context = decoratorContexts[0];

                    Assert.That(!string.IsNullOrEmpty(context.DecoratedContractName),
                        "Missing Decorated Contract Name on SceneDecoratorContext in scene '{0}'", scene.name);

                    decoratedSceneNames.Add(context.DecoratedContractName);
                }
            }

            Assert.That(decoratedSceneNames.IsEmpty(),
                "Found decorator scenes without a corresponding scene to decorator.  Missing scene contracts: {0}", decoratedSceneNames.Join(", "));
        }

        public static string ConvertFullAbsolutePathToAssetPath(string fullPath)
        {
            return "Assets/" + Path.GetFullPath(fullPath)
                .Remove(0, Path.GetFullPath(Application.dataPath).Length + 1)
                .Replace("\\", "/");
        }

        public static string TryGetSelectedFilePathInProjectsTab()
        {
            return GetSelectedFilePathsInProjectsTab().OnlyOrDefault();
        }

        public static List<string> GetSelectedFilePathsInProjectsTab()
        {
            return GetSelectedPathsInProjectsTab()
                .Where(x => File.Exists(x)).ToList();
        }

        public static List<string> GetSelectedPathsInProjectsTab()
        {
            var paths = new List<string>();

            UnityEngine.Object[] selectedAssets = Selection.GetFiltered(
                typeof(UnityEngine.Object), SelectionMode.Assets);

            foreach (var item in selectedAssets)
            {
                var relativePath = AssetDatabase.GetAssetPath(item);

                if (!string.IsNullOrEmpty(relativePath))
                {
                    var fullPath = Path.GetFullPath(Path.Combine(
                        Application.dataPath, Path.Combine("..", relativePath)));

                    paths.Add(fullPath);
                }
            }

            return paths;
        }

        // Note that the path is relative to the Assets folder
        public static List<string> GetSelectedFolderPathsInProjectsTab()
        {
            return GetSelectedPathsInProjectsTab()
                .Where(x => Directory.Exists(x)).ToList();
        }

        // Returns the best guess directory in projects pane
        // Useful when adding to Assets -> Create context menu
        // Returns null if it can't find one
        // Note that the path is relative to the Assets folder for use in AssetDatabase.GenerateUniqueAssetPath etc.
        public static string TryGetSelectedFolderPathInProjectsTab()
        {
            return GetSelectedFolderPathsInProjectsTab().OnlyOrDefault();
        }
    }
}

#endif
