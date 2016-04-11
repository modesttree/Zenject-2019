#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject.Internal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Zenject
{
    public abstract class CompositionRoot : MonoBehaviour
    {
        [FormerlySerializedAs("OnlyInjectWhenActive")]
        [Tooltip("When true, inactive objects will not have their members injected")]
        [SerializeField]
        bool _onlyInjectWhenActive = false;

        [FormerlySerializedAs("Installers")]
        [SerializeField]
        MonoInstaller[] _installers = new MonoInstaller[0];

        [SerializeField]
        MonoInstaller[] _installerPrefabs = new MonoInstaller[0];

        public abstract IDependencyRoot DependencyRoot
        {
            get;
        }

        public bool OnlyInjectWhenActive
        {
            get
            {
                return _onlyInjectWhenActive;
            }
            protected set
            {
                _onlyInjectWhenActive = value;
            }
        }

        public IEnumerable<MonoInstaller> Installers
        {
            get
            {
                return _installers;
            }
        }

        public IEnumerable<MonoInstaller> InstallerPrefabs
        {
            get
            {
                return _installerPrefabs;
            }
        }

        protected void SetInstallers(MonoInstaller[] installers)
        {
            _installers = installers;
        }

        void CheckInstallerPrefabTypes()
        {
#if UNITY_EDITOR
            foreach (var installer in _installers)
            {
                Assert.That(PrefabUtility.GetPrefabType(installer.gameObject) != PrefabType.Prefab,
                    "Found prefab with name '{0}' in the Installer property of CompositionRoot '{1}'.  You should use the property 'InstallerPrefabs' for this instead.", installer.name, this.name);
            }
#endif

            foreach (var installerPrefab in _installerPrefabs)
            {
                Assert.IsNotNull(installerPrefab, "Found null prefab in CompositionRoot");

#if UNITY_EDITOR
                Assert.That(PrefabUtility.GetPrefabType(installerPrefab.gameObject) == PrefabType.Prefab,
                    "Found non-prefab with name '{0}' in the InstallerPrefabs property of CompositionRoot '{1}'.  You should use the property 'Installer' for this instead",
                    installerPrefab.name, this.name);
#endif
                Assert.That(installerPrefab.GetComponent<MonoInstaller>() != null,
                    "Expected to find component with type 'MonoInstaller' on given installer prefab '{0}'", installerPrefab.name);
            }
        }

        protected void InstallInstallers(DiContainer container)
        {
            InstallInstallers(container, new Dictionary<Type, List<TypeValuePair>>());
        }

        // We pass in the container here instead of using our own for validation to work
        protected void InstallInstallers(
            DiContainer container, Dictionary<Type, List<TypeValuePair>> extraArgsMap)
        {
            CheckInstallerPrefabTypes();

            var newGameObjects = new List<GameObject>();
            var allInstallers = _installers.ToList();

            try
            {
                foreach (var installerPrefab in _installerPrefabs)
                {
                    Assert.IsNotNull(installerPrefab);

                    var installerGameObject = GameObject.Instantiate(installerPrefab.gameObject);

                    newGameObjects.Add(installerGameObject);

                    installerGameObject.transform.SetParent(this.transform, false);
                    var installer = installerGameObject.GetComponent<MonoInstaller>();

                    Assert.IsNotNull(installer);

                    allInstallers.Add(installer);
                }

                foreach (var installer in allInstallers)
                {
                    List<TypeValuePair> extraArgs;

                    if (extraArgsMap.TryGetValue(installer.GetType(), out extraArgs))
                    {
                        extraArgsMap.Remove(installer.GetType());
                        container.InstallExplicit(installer, extraArgs);
                    }
                    else
                    {
                        container.Install(installer);
                    }
                }
            }
            finally
            {
                if (container.IsValidating)
                {
                    foreach (var gameObject in newGameObjects)
                    {
                        GameObject.DestroyImmediate(gameObject);
                    }
                }
            }
        }

        protected void InstallSceneBindings(DiContainer container)
        {
            foreach (var autoBinding in GetInjectableComponents().OfType<ZenjectBinding>())
            {
                if (autoBinding == null)
                {
                    continue;
                }

                if (autoBinding.CompositionRoot == null)
                {
                    InstallAutoBinding(container, autoBinding);
                }
            }

            foreach (var autoBinding in GameObject.FindObjectsOfType<ZenjectBinding>())
            {
                if (autoBinding == null)
                {
                    continue;
                }

                if (autoBinding.CompositionRoot == this)
                {
                    InstallAutoBinding(container, autoBinding);
                }
            }
        }

        protected void InstallAutoBinding(
            DiContainer container, ZenjectBinding autoBinding)
        {
            var component = autoBinding.Component;
            var bindType = autoBinding.BindType;

            if (component == null)
            {
                return;
            }

            switch (bindType)
            {
                case ZenjectBinding.BindTypes.Self:
                {
                    container.Bind(component.GetType()).ToInstance(component);
                    break;
                }
                case ZenjectBinding.BindTypes.AllInterfaces:
                {
                    container.BindAllInterfaces(component.GetType()).ToInstance(component);
                    break;
                }
                case ZenjectBinding.BindTypes.AllInterfacesAndSelf:
                {
                    container.BindAllInterfacesAndSelf(component.GetType()).ToInstance(component);
                    break;
                }
                default:
                {
                    Assert.Throw();
                    break;
                }
            }
        }

        public abstract IEnumerable<Component> GetInjectableComponents();

        public static IEnumerable<Component> GetInjectableComponents(
            GameObject gameObject, bool onlyInjectWhenActive)
        {
            foreach (var component in ZenUtilInternal.GetInjectableComponentsBottomUp(
                gameObject, true, !onlyInjectWhenActive))
            {
                if (component == null)
                {
                    // This warning about fiBackupSceneStorage appears in normal cases so just ignore
                    // Not sure what it is
                    if (gameObject.name != "fiBackupSceneStorage")
                    {
                        Log.Warn("Zenject: Found null component on game object '{0}'.  Possible missing script.", gameObject.name);
                    }
                    continue;
                }

                if (component.GetType().DerivesFrom<MonoInstaller>())
                {
                    // Do not inject on installers since these are always injected before they are installed
                    continue;
                }

                yield return component;
            }
        }
    }
}

#endif
