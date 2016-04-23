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
    public static class ZenMenuItems
    {
        [MenuItem("Edit/Zenject/Help...")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://github.com/modesttree/zenject");
        }

        [MenuItem("Edit/Zenject/Validate All Active Scenes")]
        public static void ValidateAllActiveScenes()
        {
            ZenEditorValidator.ValidateAllActiveScenes();
        }

        [MenuItem("Edit/Zenject/Validate Current Scene #%v")]
        public static void ValidateCurrentScene()
        {
            ZenEditorValidator.ValidateCurrentScene();
        }

        // Returns true if we should continue
        static bool CheckForExistingCompositionRoot()
        {
            if (ZenEditorUtil.TryGetSceneCompositionRoot() != null)
            {
                var shouldContinue = EditorUtility.DisplayDialog(
                    "Error", "There already exists a SceneCompositionRoot in the scene.  Are you sure you want to add another?", "Yes", "Cancel");
                return shouldContinue;
            }

            return true;
        }

        [MenuItem("GameObject/Zenject/Scene Composition Root", false, 9)]
        public static void CreateSceneCompositionRoot(MenuCommand menuCommand)
        {
            if (CheckForExistingCompositionRoot())
            {
                var root = new GameObject("SceneCompositionRoot").AddComponent<SceneCompositionRoot>();
                Selection.activeGameObject = root.gameObject;
            }
        }

        [MenuItem("GameObject/Zenject/Decorator Composition Root", false, 9)]
        public static void CreateDecoratorCompositionRoot(MenuCommand menuCommand)
        {
            if (CheckForExistingCompositionRoot())
            {
                var root = new GameObject("DecoratorCompositionRoot").AddComponent<SceneDecoratorCompositionRoot>();
                Selection.activeGameObject = root.gameObject;
            }
        }

        [MenuItem("GameObject/Zenject/Facade Composition Root", false, 9)]
        public static void CreateFacadeCompositionRoot(MenuCommand menuCommand)
        {
            var root = new GameObject("FacadeCompositionRoot").AddComponent<FacadeCompositionRoot>();
            Selection.activeGameObject = root.gameObject;
        }

        [MenuItem("Edit/Zenject/Create Global Composition Root")]
        public static void CreateGlobalCompositionRootInDefaultLocation()
        {
            var fullDirPath = Path.Combine(Application.dataPath, "Resources");

            if (!Directory.Exists(fullDirPath))
            {
                Directory.CreateDirectory(fullDirPath);
            }

            CreateGlobalCompositionRootInternal("Assets/Resources");
        }

        [MenuItem("Assets/Create/Zenject/Global Composition Root")]
        public static void CreateGlobalCompositionRoot()
        {
            var dir = UnityEditorUtil.TryGetCurrentDirectoryInProjectsTab();

            if (dir == null)
            {
                EditorUtility.DisplayDialog("Error",
                    "Could not find directory to place the '{0}.prefab' asset.  Please try again by right clicking in the desired folder within the projects pane."
                    .Fmt(GlobalCompositionRoot.GlobalCompRootResourcePath), "Ok");
                return;
            }

            var parentFolderName = Path.GetFileName(dir);

            if (parentFolderName != "Resources")
            {
                EditorUtility.DisplayDialog("Error",
                    "'{0}.prefab' must be placed inside a directory named 'Resources'.  Please try again by right clicking within the Project pane in a valid Resources folder."
                    .Fmt(GlobalCompositionRoot.GlobalCompRootResourcePath), "Ok");
                return;
            }

            CreateGlobalCompositionRootInternal(dir);
        }

        static void CreateGlobalCompositionRootInternal(string dir)
        {
            var prefabPath = (Path.Combine(dir, GlobalCompositionRoot.GlobalCompRootResourcePath) + ".prefab").Replace("\\", "/");
            var emptyPrefab = PrefabUtility.CreateEmptyPrefab(prefabPath);

            var gameObject = new GameObject();

            try
            {
                gameObject.AddComponent<GlobalCompositionRoot>();

                var prefabObj = PrefabUtility.ReplacePrefab(gameObject, emptyPrefab);

                Selection.activeObject = prefabObj;
            }
            finally
            {
                GameObject.DestroyImmediate(gameObject);
            }

            Debug.Log("Created new GlobalCompositionRoot at '{0}'".Fmt(prefabPath));
        }
    }
}
#endif


