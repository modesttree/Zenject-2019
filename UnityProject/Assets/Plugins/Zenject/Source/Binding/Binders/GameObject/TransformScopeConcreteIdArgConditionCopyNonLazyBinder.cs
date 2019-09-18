#if !NOT_UNITY3D

using System;
using UnityEngine;

namespace Zenject
{
    [NoReflectionBaking]
    public class TransformScopeConcreteIdArgConditionCopyNonLazyBinder : ActiveGameObjectScopeConcreteIdArgConditionCopyNonLazyBinder
    {
        public TransformScopeConcreteIdArgConditionCopyNonLazyBinder(
            BindInfo bindInfo,
            GameObjectCreationParameters gameObjectInfo)
            : base(bindInfo, gameObjectInfo)
        {
        }

        public ActiveGameObjectScopeConcreteIdArgConditionCopyNonLazyBinder UnderTransform(Transform parent)
        {
            GameObjectInfo.ParentTransform = parent;
            return this;
        }

        public ActiveGameObjectScopeConcreteIdArgConditionCopyNonLazyBinder UnderTransform(Func<InjectContext, Transform> parentGetter)
        {
            GameObjectInfo.ParentTransformGetter = parentGetter;
            return this;
        }

        public ActiveGameObjectScopeConcreteIdArgConditionCopyNonLazyBinder UnderTransformGroup(string transformGroupname)
        {
            GameObjectInfo.GroupName = transformGroupname;
            return this;
        }
    }
}

#endif
