#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    public class ZenjectBinding : MonoBehaviour
    {
        [Tooltip("The component to add to the Zenject container")]
        [SerializeField]
        Component _component = null;

        [Tooltip("This value will determine what container the component gets added to.  Note that this value is optional.  If unset, the component will be bound on the most 'local' composition root.  In most cases this will be the SceneCompositionRoot, unless this component is underneath a FacadeCompositionRoot, in which case it will bind to that instead by default.  You can also override this default by providing the CompositionRoot directly.  This can be useful if you want to bind something that is inside a FacadeCompositionRoot to the SceneCompositionRoot container.")]
        [SerializeField]
        CompositionRoot _compositionRoot;

        [Tooltip("This value is used to determine how to bind this component.  When set to 'Self' is equivalent to calling Container.ToInstance inside an installer. When set to 'AllInterfaces' this is equivalent to calling 'Container.BindAllInterfaces<MyMonoBehaviour>().ToInstance', and similarly for AllInterfacesAndSelf")]
        [SerializeField]
        BindTypes _bindType = BindTypes.Self;

        public CompositionRoot CompositionRoot
        {
            get
            {
                return _compositionRoot;
            }
        }

        public Component Component
        {
            get
            {
                return _component;
            }
        }

        public BindTypes BindType
        {
            get
            {
                return _bindType;
            }
        }

        public enum BindTypes
        {
            Self,
            AllInterfaces,
            AllInterfacesAndSelf,
        }
    }
}

#endif

