#if !NOT_UNITY3D

namespace Zenject
{
    [NoReflectionBaking]
    public class ActiveGameObjectScopeConcreteIdArgConditionCopyNonLazyBinder : ScopeConcreteIdArgConditionCopyNonLazyBinder
    {
        protected GameObjectCreationParameters GameObjectInfo { get; }

        public ActiveGameObjectScopeConcreteIdArgConditionCopyNonLazyBinder(BindInfo bindInfo, GameObjectCreationParameters gameObjectInfo) :
            base(bindInfo)
        {
            GameObjectInfo = gameObjectInfo;
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder IsActiveSelf(bool isActive)
        {
            GameObjectInfo.IsActiveSelf = isActive;
            return this;
        }
    }
}

#endif