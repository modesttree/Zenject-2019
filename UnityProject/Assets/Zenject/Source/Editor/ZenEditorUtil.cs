#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;
using ModestTree;
using Zenject.Internal;

#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

namespace Zenject
{
    public static class ZenEditorUtil
    {
        public static SceneDecoratorCompositionRoot TryGetSceneDecoratorCompositionRoot()
        {
            return GameObject.FindObjectsOfType<SceneDecoratorCompositionRoot>().OnlyOrDefault();
        }

        public static SceneCompositionRoot TryGetSceneCompositionRoot()
        {
            return GameObject.FindObjectsOfType<SceneCompositionRoot>().OnlyOrDefault();
        }

        public static void OpenScene(string scenePath)
        {
#if UNITY_5_3_OR_NEWER
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
#else
            EditorApplication.OpenScene(scenePath);
#endif
        }

        public static void OpenSceneAdditive(string scenePath)
        {
#if UNITY_5_3_OR_NEWER
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
#else
            EditorApplication.OpenSceneAdditive(scenePath);
#endif
        }
    }
}
#endif
