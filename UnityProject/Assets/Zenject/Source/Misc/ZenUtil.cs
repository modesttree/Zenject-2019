using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif
using UnityEngine;
#endif

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class ZenUtil
    {
        // Due to the way that Unity overrides the Equals operator,
        // normal null checks such as (x == null) do not always work as
        // expected
        // In those cases you can use this function which will also
        // work with non-unity objects
        public static bool IsNull(System.Object obj)
        {
            return obj == null || obj.Equals(null);
        }

#if !ZEN_NOT_UNITY3D
        public static void LoadScene(string levelName)
        {
            LoadSceneInternal(levelName, false, null, null);
        }

        public static void LoadScene(string levelName, Action<DiContainer> preBindings)
        {
            LoadSceneInternal(levelName, false, preBindings, null);
        }

        public static void LoadScene(
            string levelName, Action<DiContainer> preBindings, Action<DiContainer> postBindings)
        {
            LoadSceneInternal(levelName, false, preBindings, postBindings);
        }

        public static void LoadSceneAdditive(string levelName)
        {
            LoadSceneInternal(levelName, true, null, null);
        }

        public static void LoadSceneAdditive(string levelName, Action<DiContainer> preBindings)
        {
            LoadSceneInternal(levelName, true, preBindings, null);
        }

        public static void LoadSceneAdditive(
            string levelName, Action<DiContainer> preBindings, Action<DiContainer> postBindings)
        {
            LoadSceneInternal(levelName, true, preBindings, postBindings);
        }

        static void UnityLoadScene(string levelName, bool isAdditive)
        {
#if UNITY_5_3
            SceneManager.LoadScene(levelName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
#else
            if (isAdditive)
            {
                Application.LoadLevelAdditive(levelName);
            }
            else
            {
                Application.LoadLevel(levelName);
            }
#endif
        }

        static void LoadSceneInternal(
            string levelName, bool isAdditive, Action<DiContainer> preBindings, Action<DiContainer> postBindings)
        {
            if (preBindings != null)
            {
                SceneCompositionRoot.BeforeInstallHooks += preBindings;
            }

            if (postBindings != null)
            {
                SceneCompositionRoot.AfterInstallHooks += postBindings;
            }

            Assert.That(Application.CanStreamedLevelBeLoaded(levelName), "Unable to load level '{0}'", levelName);

            Log.Debug("Starting to load scene '{0}'", levelName);
            UnityLoadScene(levelName, isAdditive);
            Log.Debug("Finished loading scene '{0}'", levelName);
        }

        // This method can be used to load the given scene and perform injection on its contents
        // Note that the scene we're loading can have [Inject] flags however it should not have
        // its own composition root
        public static IEnumerator LoadSceneAdditiveWithContainer(
            string levelName, DiContainer parentContainer)
        {
            var rootObjectsBeforeLoad = UnityUtil.GetRootGameObjects();

            UnityLoadScene(levelName, true);

            // Wait one frame for objects to be added to the scene heirarchy
            yield return null;

            var rootObjectsAfterLoad = UnityUtil.GetRootGameObjects();

            foreach (var newObject in rootObjectsAfterLoad.Except(rootObjectsBeforeLoad))
            {
                Assert.That(newObject.GetComponent<SceneCompositionRoot>() == null,
                    "LoadSceneAdditiveWithContainer does not expect a container to exist in the loaded scene");

                parentContainer.InjectGameObject(newObject);
            }
        }

#if UNITY_5_3
        public static IEnumerator LoadSceneAdditiveWithContainerAsync(string levelName, DiContainer parentContainer)
        {
            return LoadSceneAdditiveWithContainerAsync(levelName, parentContainer, null);
        }

        // This method can be used to load the given scene and perform injection on its contents
        // Note that the scene we're loading can have [Inject] flags however it should not have
        // its own composition root.
        // This Method uses LoadSceneAsync and works in unity from 5.3 and upward.
        // After the loading finishes and the callback parameter is set the callback gets called.
        public static IEnumerator LoadSceneAdditiveWithContainerAsync(string levelName, DiContainer parentContainer,
            Action<Scene> callback)
        {
            var asyncOp = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

            yield return asyncOp;

            var scene = SceneManager.GetSceneByName(levelName);

            if (scene.IsValid())
            {
                foreach (var go in scene.GetRootGameObjects())
                {
                    parentContainer.InjectGameObject(go);
                }

                if (callback != null)
                {
                    callback(scene);
                }
            }
        }
#endif
#endif
    }
}
