using System.Collections.Generic;
using System.Linq;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif
using ModestTree.Util;
using Zenject;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using ModestTree;

namespace Zenject
{
    [CustomEditor(typeof(SceneDecoratorCompositionRoot))]
    public class SceneDecoratorCompositionRootEditor : UnityEditor.Editor
    {
        List<ReorderableList> _propLists;

        public virtual void OnEnable()
        {
            _propLists = new List<ReorderableList>();

            var names = new string[]
            {
                "DecoratorInstallers",
                "PreInstallers",
                "PostInstallers"
            };

            var descriptions = new string[]
            {
                "",
                "",
                ""
            };

            Assert.IsEqual(descriptions.Length, names.Length);

            var infos = Enumerable.Range(0, names.Length).Select(i => new { Name = names[i], Description = descriptions[i] }).ToList();

            foreach (var info in infos)
            {
                var prop = serializedObject.FindProperty(info.Name);

                ReorderableList reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true);
                _propLists.Add(reorderableList);

                var closedName = info.Name;
                var closedDesc = info.Description;

                reorderableList.drawHeaderCallback += rect =>
                {
                    GUI.Label(rect,
                        new GUIContent(closedName, closedDesc));
                };

                reorderableList.drawElementCallback += (rect, index, active, focused) =>
                {
                    rect.width -= 40;
                    rect.x += 20;
                    EditorGUI.PropertyField(rect, prop.GetArrayElementAtIndex(index), GUIContent.none, true);
                };
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (Application.isPlaying)
            {
                GUI.enabled = false;
            }

            GUILayout.Space(5);

            var binder = target as SceneDecoratorCompositionRoot;

            EditorGUILayout.BeginHorizontal();
            {
                binder.SceneName = EditorGUILayout.TextField("Decorated Scene", binder.SceneName);

                GUILayout.Space(10);

                if (GUILayout.Button("Open", GUILayout.MaxWidth(40)))
                {
                    EditorApplication.delayCall += () =>
                    {
                        var scenePath = UnityEditorUtil.TryGetScenePath(binder.SceneName);

                        if (scenePath == null)
                        {
                            EditorUtility.DisplayDialog("Error",
                                "Could not find scene with name '{0}'.  Is it added to your build settings?".Fmt(binder.SceneName), "Ok");
                        }
                        else
                        {
#if UNITY_5_3_OR_NEWER
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
#else
                            if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
#endif
                            {
                                ZenEditorUtil.OpenScene(scenePath);

                                var compRoot = ZenEditorUtil.TryGetSceneCompositionRoot();

                                if (compRoot != null)
                                {
                                    Selection.activeGameObject = compRoot.gameObject;
                                }
                                else
                                {
                                    var decoratorCompRoot = ZenEditorUtil.TryGetSceneDecoratorCompositionRoot();

                                    if (decoratorCompRoot != null)
                                    {
                                        Selection.activeGameObject = decoratorCompRoot.gameObject;
                                    }
                                }
                            }
                        }
                    };
                }
            }
            EditorGUILayout.EndHorizontal();

            foreach (var list in _propLists)
            {
                list.DoLayoutList();
            }

            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
