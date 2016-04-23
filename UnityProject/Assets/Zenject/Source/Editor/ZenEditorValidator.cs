#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree.Util;
using UnityEngine;
using ModestTree;
using Zenject.Internal;
using UnityEditor;

#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

namespace Zenject
{
    public static class ZenEditorValidator
    {
        static string GetActiveScene()
        {
#if UNITY_5_3_OR_NEWER
            return EditorSceneManager.GetActiveScene().path;
#else
            return EditorApplication.currentScene;
#endif
        }

        public static void ValidateCurrentSceneThenPlay()
        {
            if (ValidateCurrentScene())
            {
                EditorApplication.isPlaying = true;
            }
        }

        // This can be called by build scripts using batch mode unity for continuous integration testing
        // This will exit with an error code for whether validation passed or not
        public static void ValidateAllScenesFromScript()
        {
            ValidateAllActiveScenes(true);
        }

        public static bool ValidateAllActiveScenes()
        {
            return ValidateAllActiveScenes(false);
        }

        public static bool ValidateAllActiveScenes(bool exitAfter)
        {
            return ValidateScenes(UnityEditorUtil.GetAllActiveScenePaths(), exitAfter);
        }

        public static void ValidateScenesFromScript()
        {
            var sceneNames = UnityEditorUtil.GetArgument("scenes").Split(',');

            Assert.That(sceneNames.Length > 0);

            ValidateScenes(
                sceneNames.Select(x => UnityEditorUtil.GetScenePath(x)).ToList(), true);
        }

        public static bool ValidateScenes(List<string> scenePaths, bool exitAfter)
        {
            var startScene = GetActiveScene();

            var failedScenes = new List<string>();

            foreach (var scenePath in scenePaths)
            {
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);

                Log.Trace("Validating scene '{0}'...", sceneName);

                ZenEditorUtil.OpenScene(scenePath);

                if (!ValidateCurrentScene())
                {
                    Log.Error("Failed to validate scene '{0}'", sceneName);
                    failedScenes.Add(sceneName);
                }
            }

            if (!exitAfter && !string.IsNullOrEmpty(startScene))
            {
                ZenEditorUtil.OpenScene(startScene);
            }

            if (failedScenes.IsEmpty())
            {
                Log.Trace("Successfully validated all {0} scenes", scenePaths.Count);

                if (exitAfter)
                {
                    EditorApplication.Exit(0);
                }

                return true;
            }
            else
            {
                Log.Error("Validated {0}/{1} scenes. Failed to validate the following: {2}",
                    scenePaths.Count - failedScenes.Count, scenePaths.Count, failedScenes.Join(", "));

                if (exitAfter)
                {
                    EditorApplication.Exit(1);
                }

                return false;
            }
        }

        static List<ZenjectResolveException> ValidateDecoratorCompRoot(SceneDecoratorCompositionRoot decoratorCompRoot, int maxErrors)
        {
            var sceneName = decoratorCompRoot.SceneName;
            var scenePath = UnityEditorUtil.GetScenePath(sceneName);

            if (scenePath == null)
            {
                return new List<ZenjectResolveException>()
                {
                    new ZenjectResolveException(
                        "Could not find scene path for decorated scene '{0}'".Fmt(sceneName)),
                };
            }

            var rootObjectsBefore = UnityUtil.GetRootGameObjects();

            ZenEditorUtil.OpenSceneAdditive(scenePath);

            var newRootObjects = UnityUtil.GetRootGameObjects().Except(rootObjectsBefore);

            // Use finally to ensure we clean up the data added from OpenSceneAdditive
            try
            {
                var previousBeforeInstallHook = SceneCompositionRoot.BeforeInstallHooks;
                SceneCompositionRoot.BeforeInstallHooks = (container) =>
                {
                    if (previousBeforeInstallHook != null)
                    {
                        previousBeforeInstallHook(container);
                    }

                    decoratorCompRoot.AddPreBindings(container);
                };

                var previousAfterInstallHook = SceneCompositionRoot.AfterInstallHooks;
                SceneCompositionRoot.AfterInstallHooks = (container) =>
                {
                    decoratorCompRoot.AddPostBindings(container);

                    if (previousAfterInstallHook != null)
                    {
                        previousAfterInstallHook(container);
                    }
                };

                var compRoot = newRootObjects.SelectMany(x => x.GetComponentsInChildren<SceneCompositionRoot>()).OnlyOrDefault();

                if (compRoot != null)
                {
                    return ValidateScene(compRoot, maxErrors);
                }

                var newDecoratorCompRoot = newRootObjects.SelectMany(x => x.GetComponentsInChildren<SceneDecoratorCompositionRoot>()).OnlyOrDefault();

                if (newDecoratorCompRoot != null)
                {
                    return ValidateDecoratorCompRoot(newDecoratorCompRoot, maxErrors);
                }

                return new List<ZenjectResolveException>()
                {
                    new ZenjectResolveException(
                        "Could not find composition root for decorated scene '{0}'".Fmt(sceneName)),
                };
            }
            finally
            {
#if UNITY_5_3_OR_NEWER
                EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByPath(scenePath), true);
#else
                foreach (var newObject in newRootObjects)
                {
                    GameObject.DestroyImmediate(newObject);
                }
#endif
            }
        }

        public static bool ValidateCurrentScene()
        {
            var startTime = DateTime.Now;
            // Only show a few to avoid spamming the log too much
            var resolveErrors = GetCurrentSceneValidationErrors(10).ToList();

            foreach (var error in resolveErrors)
            {
                Log.ErrorException(error);
            }

            var secondsElapsed = (DateTime.Now - startTime).Milliseconds / 1000.0f;

            if (resolveErrors.Any())
            {
                Log.Error("Validation Completed With Errors, Took {0:0.00} Seconds.", secondsElapsed);
                return false;
            }

            Log.Info("Validation Completed Successfully, Took {0:0.00} Seconds.", secondsElapsed);
            return true;
        }

        static List<ZenjectResolveException> GetCurrentSceneValidationErrors(int maxErrors)
        {
            var compRoot = GameObject.FindObjectsOfType<SceneCompositionRoot>().OnlyOrDefault();

            if (compRoot != null)
            {
                return ValidateScene(compRoot, maxErrors);
            }

            var decoratorCompRoot = GameObject.FindObjectsOfType<SceneDecoratorCompositionRoot>().OnlyOrDefault();

            if (decoratorCompRoot != null)
            {
                return ValidateDecoratorCompRoot(decoratorCompRoot, maxErrors);
            }

            return new List<ZenjectResolveException>()
            {
                new ZenjectResolveException("Unable to find unique composition root in current scene"),
            };
        }

        public static List<ZenjectResolveException> ValidateScene(SceneCompositionRoot compRoot, int maxErrors)
        {
            GlobalCompositionRoot globalCompRoot = null;

            try
            {
                globalCompRoot = GlobalCompositionRoot.InstantiateNewRoot();

                // Need to do this here even though this is also done in ZenValidator
                // so these errors are caught for DLL builds
                ValidateCompositionRootInstallers(globalCompRoot);
                ValidateCompositionRootInstallers(compRoot);

                foreach (var facadeRoot in GameObject.FindObjectsOfType<FacadeCompositionRoot>())
                {
                    ValidateCompositionRootInstallers(facadeRoot);
                }

                return ZenValidator.ValidateScene(compRoot, globalCompRoot).Take(maxErrors).ToList();
            }
            finally
            {
                if (globalCompRoot != null)
                {
                    GameObject.DestroyImmediate(globalCompRoot.gameObject);
                }
            }
        }

        public static void ValidateCompositionRootInstallers(CompositionRoot compRoot)
        {
            foreach (var installer in compRoot.Installers)
            {
                Assert.That(PrefabUtility.GetPrefabType(installer.gameObject) != PrefabType.Prefab,
                    "Found prefab with name '{0}' in the Installer property of CompositionRoot '{1}'.  You should use the property 'InstallerPrefabs' for this instead.", installer.name, compRoot.name);
            }

            foreach (var installer in compRoot.InstallerPrefabs)
            {
                Assert.That(PrefabUtility.GetPrefabType(installer.gameObject) == PrefabType.Prefab,
                    "Found non-prefab with name '{0}' in the InstallerPrefabs property of CompositionRoot '{1}'.  You should use the property 'Installer' for this instead", installer.name, compRoot.name);
            }
        }
    }
}
#endif



