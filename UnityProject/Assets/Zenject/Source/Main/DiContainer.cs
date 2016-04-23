using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using Zenject.Internal;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    // Responsibilities:
    // - Expose methods to configure object graph via Bind() methods
    // - Build object graphs via Resolve() method
    public class DiContainer : IInstantiator, IResolver, IBinder
    {
#if !ZEN_NOT_UNITY3D
        public const string DefaultParentId = "InstantiateDefaultParent";
#endif

        readonly Dictionary<BindingId, List<ProviderBase>> _providers = new Dictionary<BindingId, List<ProviderBase>>();
        readonly HashSet<Type> _installedInstallers = new HashSet<Type>();
        readonly Stack<Type> _installsInProgress = new Stack<Type>();
        readonly DiContainer _parentContainer;
        readonly Stack<LookupId> _resolvesInProgress = new Stack<LookupId>();
        readonly SingletonProviderCreator _singletonProviderFactory;
        readonly SingletonRegistry _singletonRegistry;
        bool _isValidating;
        bool _isInstalling;
        bool _hasDisplayedInstallWarning;

#if !ZEN_NOT_UNITY3D
        bool _hasLookedUpParent;
        Transform _defaultParent;
#endif

        public DiContainer(bool isValidating)
        {
            _isValidating = isValidating;

            _singletonRegistry = new SingletonRegistry();
            _singletonProviderFactory = new SingletonProviderCreator(this, _singletonRegistry);

            Bind<DiContainer>().ToInstance(this);
            Bind<IBinder>().ToInstance(this);
            Bind<IResolver>().ToInstance(this);
            Bind<IInstantiator>().ToInstance(this);
        }

        public DiContainer()
            : this(false)
        {
        }

        public DiContainer(DiContainer parentContainer, bool isValidating)
            : this(isValidating)
        {
            _parentContainer = parentContainer;
        }

        public DiContainer(DiContainer parentContainer)
            : this(parentContainer, false)
        {
        }

        public SingletonProviderCreator SingletonProviderCreator
        {
            get
            {
                return _singletonProviderFactory;
            }
        }

        public SingletonRegistry SingletonRegistry
        {
            get
            {
                return _singletonRegistry;
            }
        }

#if !ZEN_NOT_UNITY3D
        public Transform DefaultParent
        {
            get
            {
                // We should be able to cache this since we should be able to assume that
                // this property isn't called until after the install phase
                if (!_hasLookedUpParent)
                {
                    _hasLookedUpParent = true;

                    // Use an InjectContext so we can specify local = true
                    // and optional = true
                    var ctx = new InjectContext(
                        this, typeof(Transform), DefaultParentId,
                        true, null, null, "", null, null, null, InjectSources.Local);

                    _defaultParent = Resolve<Transform>(ctx);
                }

                return _defaultParent;
            }
        }
#endif

        public DiContainer ParentContainer
        {
            get
            {
                return _parentContainer;
            }
        }

        public bool ChecksForCircularDependencies
        {
            get
            {
#if ZEN_MULTITHREADING
                // When multithreading is supported we can't use a static field to track the lookup
                // TODO: We could look at the inject context though
                return false;
#else
                return true;
#endif
            }
        }

        public IEnumerable<Type> InstalledInstallers
        {
            get
            {
                return _installedInstallers;
            }
        }

        // See comment in IBinder.cs for description
        public bool IsValidating
        {
            get
            {
                return _isValidating;
            }
            set
            {
                _isValidating = value;
            }
        }

        // When this is true, it will log warnings when Resolve or Instantiate
        // methods are called
        // Used to ensure that Resolve and Instantiate methods are not called
        // during bind phase.  This is important since Resolve and Instantiate
        // make use of the bindings, so if the bindings are not complete then
        // unexpected behaviour can occur
        public bool IsInstalling
        {
            get
            {
                return _isInstalling;
            }
            set
            {
                _isInstalling = value;
            }
        }

        public IEnumerable<BindingId> AllContracts
        {
            get
            {
                return _providers.Keys;
            }
        }

        // Note that this list is not exhaustive or even accurate so use with caution
        public IEnumerable<Type> AllConcreteTypes
        {
            get
            {
                return (from x in _providers from p in x.Value select p.GetInstanceType()).Where(x => x != null && !x.IsInterface && !x.IsAbstract).Distinct();
            }
        }

        public DiContainer CreateSubContainer()
        {
            return new DiContainer(this, _isValidating);
        }

        public void RegisterProvider(
            ProviderBase provider, BindingId bindingId)
        {
            if (_providers.ContainsKey(bindingId))
            {
                // Prevent duplicate singleton bindings:
                if (_providers[bindingId].Find(item => ReferenceEquals(item, provider)) != null)
                {
                    throw new ZenjectBindException(
                        "Found duplicate singleton binding for contract '{0}' and id '{1}'".Fmt(bindingId.Type, bindingId.Identifier));
                }

                _providers[bindingId].Add(provider);
            }
            else
            {
                _providers.Add(bindingId, new List<ProviderBase> {provider});
            }
        }

        public int UnregisterProvider(ProviderBase provider)
        {
            int numRemoved = 0;

            foreach (var keyValue in _providers)
            {
                numRemoved += keyValue.Value.RemoveAll(x => x == provider);
            }

            Assert.That(numRemoved > 0, "Tried to unregister provider that was not registered");

            // Remove any empty contracts
            foreach (var bindingId in _providers.Where(x => x.Value.IsEmpty()).Select(x => x.Key).ToList())
            {
                _providers.Remove(bindingId);
            }

            provider.Dispose();

            return numRemoved;
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateResolve<TContract>()
        {
            return ValidateResolve<TContract>((string)null);
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateResolve<TContract>(string identifier)
        {
            return ValidateResolve(typeof(TContract), identifier);
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateResolve(Type contractType)
        {
            return ValidateResolve(contractType, (string)null);
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateResolve(Type contractType, string identifier)
        {
            return ValidateResolve(new InjectContext(this, contractType, identifier));
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateAll(params Type[] ignoreTypes)
        {
            foreach (var error in ValidateValidatables(ignoreTypes))
            {
                yield return error;
            }

            // Use ToList() in case it changes somehow during iteration
            foreach (var pair in _providers.ToList())
            {
                var bindingId = pair.Key;

                if (ignoreTypes.Where(i => bindingId.Type.DerivesFromOrEqual(i)).Any())
                {
                    continue;
                }

                foreach (var provider in pair.Value)
                {
                    var injectCtx = new InjectContext(
                        this, bindingId.Type, bindingId.Identifier);

                    foreach (var error in provider.ValidateBinding(injectCtx))
                    {
                        yield return error;
                    }
                }
            }
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateValidatables(params Type[] ignoreTypes)
        {
            // Use ToList() in case it changes somehow during iteration
            foreach (var pair in _providers.ToList())
            {
                var bindingId = pair.Key;

                if (ignoreTypes.Where(i => bindingId.Type.DerivesFromOrEqual(i)).Any())
                {
                    continue;
                }

                // Validate all IValidatableFactory's
                List<ProviderBase> validatableFactoryProviders;

                var providers = pair.Value;

                if (bindingId.Type.DerivesFrom<IValidatableFactory>())
                {
                    validatableFactoryProviders = providers;
                }
                else
                {
                    validatableFactoryProviders = providers.Where(x => x.GetInstanceType().DerivesFrom<IValidatableFactory>()).ToList();
                }

                var injectCtx = new InjectContext(this, bindingId.Type, bindingId.Identifier);

                foreach (var provider in validatableFactoryProviders)
                {
                    var factory = (IValidatableFactory)provider.GetInstance(injectCtx);

                    var type = factory.ConstructedType;
                    var providedArgs = factory.ProvidedTypes;

                    foreach (var error in ValidateObjectGraph(type, injectCtx, null, providedArgs))
                    {
                        yield return error;
                    }
                }

                // Validate all IValidatable's
                List<ProviderBase> validatableProviders;

                if (bindingId.Type.DerivesFrom<IValidatable>())
                {
                    validatableProviders = providers;
                }
                else
                {
                    validatableProviders = providers.Where(x => x.GetInstanceType().DerivesFrom<IValidatable>()).ToList();
                }

                Assert.That(validatableFactoryProviders.Intersect(validatableProviders).IsEmpty(),
                    "Found provider implementing both IValidatable and IValidatableFactory.  This is not allowed.");

                foreach (var provider in validatableProviders)
                {
                    var factory = (IValidatable)provider.GetInstance(injectCtx);

                    foreach (var error in factory.Validate())
                    {
                        yield return error;
                    }
                }
            }
        }

        // Wrap IEnumerable<> to avoid LINQ mistakes
        internal List<ProviderBase> GetAllProviderMatches(InjectContext context)
        {
            return GetProviderMatchesInternal(context).Select(x => x.Provider).ToList();
        }

        // Be careful with this method since it is a coroutine
        IEnumerable<ProviderPair> GetProviderMatchesInternal(InjectContext context)
        {
            return GetProvidersForContract(context.BindingId, context.SourceType).Where(x => x.Provider.Matches(context));
        }

        IEnumerable<ProviderPair> GetProvidersForContract(BindingId bindingId, InjectSources sourceType)
        {
            switch (sourceType)
            {
                case InjectSources.Local:
                {
                    return GetLocalProviders(bindingId).Select(x => new ProviderPair(x, this));
                }
                case InjectSources.Any:
                {
                    var localPairs = GetLocalProviders(bindingId).Select(x => new ProviderPair(x, this));

                    if (_parentContainer == null)
                    {
                        return localPairs;
                    }

                    return localPairs.Concat(
                        _parentContainer.GetProvidersForContract(bindingId, InjectSources.Any));
                }
                case InjectSources.AnyParent:
                {
                    if (_parentContainer == null)
                    {
                        return Enumerable.Empty<ProviderPair>();
                    }

                    return _parentContainer.GetProvidersForContract(bindingId, InjectSources.Any);
                }
                case InjectSources.Parent:
                {
                    if (_parentContainer == null)
                    {
                        return Enumerable.Empty<ProviderPair>();
                    }

                    return _parentContainer.GetProvidersForContract(bindingId, InjectSources.Local);
                }
            }

            Assert.Throw("Invalid source type");
            return null;
        }

        List<ProviderBase> GetLocalProviders(BindingId bindingId)
        {
            List<ProviderBase> localProviders;

            if (_providers.TryGetValue(bindingId, out localProviders))
            {
                return localProviders;
            }

            // If we are asking for a List<int>, we should also match for any localProviders that are bound to the open generic type List<>
            // Currently it only matches one and not the other - not totally sure if this is better than returning both
            if (bindingId.Type.IsGenericType && _providers.TryGetValue(new BindingId(bindingId.Type.GetGenericTypeDefinition(), bindingId.Identifier), out localProviders))
            {
                return localProviders;
            }

            return new List<ProviderBase>();
        }

        // See comment in IResolver.cs for description of this method
        public IList ResolveAll(InjectContext context)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            CheckForInstallWarning(context);

            var matches = GetProviderMatchesInternal(context).ToList();

            if (matches.Any())
            {
                return ReflectionUtil.CreateGenericList(
                    context.MemberType, matches.Select(x => SafeGetInstance(x.Provider, context)).ToArray());
            }

            if (!context.Optional)
            {
                throw new ZenjectResolveException(
                    "Could not find required dependency with type '" + context.MemberType.Name() + "' \nObject graph:\n" + context.GetObjectGraphString());
            }

            return ReflectionUtil.CreateGenericList(context.MemberType, new object[] {});
        }

        void CheckForInstallWarning(InjectContext context)
        {
#if DEBUG || UNITY_EDITOR
            if (!_isInstalling)
            {
                return;
            }

            if (_hasDisplayedInstallWarning)
            {
                return;
            }

            if (context == null)
            {
                // No way to tell whether this is ok or not so just assume ok
                return;
            }

            var rootContext = context.ParentContextsAndSelf.Last();

            if (rootContext.MemberType.DerivesFrom<IInstaller>())
            {
                // Resolving/instantiating/injecting installers is valid during install phase
                return;
            }

#if !ZEN_NOT_UNITY3D
            if (rootContext.MemberType.DerivesFrom<DecoratorInstaller>())
            {
                return;
            }

            if (rootContext.Identifier == DefaultParentId)
            {
                // The default transform gets resolved which is also fine
                return;
            }
#endif

            _hasDisplayedInstallWarning = true;
            // Feel free to comment this out if you are comfortable with this practice
            Log.Warn("Zenject Warning: It is bad practice to call Inject/Resolve/Instantiate before all the Installers have completed!  This is important to ensure that all bindings have properly been installed in case they are needed when injecting/instantiating/resolving.  Detected when operating on type '{0}'", rootContext.MemberType.Name());
#endif
        }

        // See comment in IResolver.cs for description of this method
        public List<Type> ResolveTypeAll(InjectContext context)
        {
            CheckForInstallWarning(context);

            if (_providers.ContainsKey(context.BindingId))
            {
                return _providers[context.BindingId].Select(x => x.GetInstanceType()).Where(x => x != null).ToList();
            }

            return new List<Type> {};
        }

        // See comment in IBinder.cs for description for this method
        public void Install(IEnumerable<IInstaller> installers)
        {
            foreach (var installer in installers)
            {
                Assert.IsNotNull(installer, "Tried to install a null installer");

                if (installer.IsEnabled)
                {
                    Install(installer);
                }
            }
        }

        // See comment in IBinder.cs for description for this method
        public void InstallExplicit(IInstaller installer, List<TypeValuePair> extraArgs)
        {
            Assert.That(installer.IsEnabled);

            InjectExplicit(installer, extraArgs);
            InstallInstallerInternal(installer);
        }

        // See comment in IBinder.cs for description for this method
        public void Install(IInstaller installer, params object[] extraArgs)
        {
            Assert.That(installer.IsEnabled);

            Inject(installer, extraArgs);
            InstallInstallerInternal(installer);
        }

        // See comment in IBinder.cs for description for this method
        public void Install<T>(params object[] extraArgs)
            where T : IInstaller
        {
            Install(typeof(T), extraArgs);
        }

        // See comment in IBinder.cs for description of this method
        public void Install(Type installerType, params object[] extraArgs)
        {
            InstallExplicit(installerType, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // See comment in IBinder.cs for description of this method
        public void InstallExplicit(Type installerType, List<TypeValuePair> extraArgs)
        {
            Assert.That(installerType.DerivesFrom<IInstaller>());

#if !ZEN_NOT_UNITY3D
            if (installerType.DerivesFrom<MonoInstaller>())
            {
                var gameObj = InstantiateAndParentPrefabResource("Installers/" + installerType.Name());

                var installer = gameObj.GetComponentInChildren<MonoInstaller>();

                InjectExplicit(installer, extraArgs);

                try
                {
                    InstallInstallerInternal(installer);
                }
                finally
                {
                    // When running, it is nice to keep the installer around so that you can change the settings live
                    // But when validating at edit time, we don't want to add the new game object
                    if (!Application.isPlaying)
                    {
                        GameObject.DestroyImmediate(installer.gameObject);
                    }
                }
            }
            else
#endif
            {
                var installer = (IInstaller)InstantiateExplicit(installerType, extraArgs);
                InstallInstallerInternal(installer);
            }
        }

        // See comment in IBinder.cs for description of this method
        public bool HasInstalled<T>()
            where T : IInstaller
        {
            return HasInstalled(typeof(T));
        }

        // See comment in IBinder.cs for description of this method
        public bool HasInstalled(Type installerType)
        {
            return _installedInstallers.Where(x => x == installerType).Any();
        }

        void InstallInstallerInternal(IInstaller installer)
        {
            var installerType = installer.GetType();

            Log.Debug("Installing installer '{0}'", installerType);

            Assert.That(!_installsInProgress.Contains(installerType),
                "Potential infinite loop detected while installing '{0}'", installerType.Name());

            Assert.That(!_installedInstallers.Contains(installerType),
                "Tried installing installer '{0}' twice", installerType.Name());

            _installedInstallers.Add(installerType);
            _installsInProgress.Push(installerType);

            try
            {
                installer.InstallBindings();
            }
            catch (Exception e)
            {
                // This context information is really helpful when bind commands fail
                throw new Exception(
                    "Error occurred while running installer '{0}'".Fmt(installer.GetType().Name()), e);
            }
            finally
            {
                Assert.That(_installsInProgress.Peek().Equals(installerType));
                _installsInProgress.Pop();
            }
        }

        // Try looking up a single provider for a given context
        // Note that this method should not throw zenject exceptions
        internal ProviderLookupResult TryGetUniqueProvider(
            InjectContext context, out ProviderBase provider)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)
            var providers = GetProviderMatchesInternal(context).ToList();

            if (providers.IsEmpty())
            {
                provider = null;
                return ProviderLookupResult.None;
            }

            if (providers.Count > 1)
            {
                // If we find multiple providers and we are looking for just one, then
                // try to intelligently choose one from the list before giving up

                // First try picking the most 'local' dependencies
                // This will bias towards bindings for the lower level specific containers rather than the global high level container
                // This will, for example, allow you to just ask for a DiContainer dependency without needing to specify [Inject(InjectSources.Local)]
                // (otherwise it would always match for a list of DiContainer's for all parent containers)
                var sortedProviders = providers.Select(x => new { Pair = x, Distance = GetContainerHeirarchyDistance(x.Container) }).OrderBy(x => x.Distance).ToList();

                sortedProviders.RemoveAll(x => x.Distance != sortedProviders[0].Distance);

                if (sortedProviders.Count == 1)
                {
                    // We have one match that is the closest
                    provider = sortedProviders[0].Pair.Provider;
                }
                else
                {
                    // Try choosing the one with a condition before giving up and throwing an exception
                    // This is nice because it allows us to bind a default and then override with conditions
                    provider = sortedProviders.Select(x => x.Pair.Provider).Where(x => x.Condition != null).OnlyOrDefault();

                    if (provider == null)
                    {
                        return ProviderLookupResult.Multiple;
                    }
                }
            }
            else
            {
                provider = providers.Single().Provider;
            }

            Assert.IsNotNull(provider);
            return ProviderLookupResult.Success;
        }

        // See comment in IResolver.cs for description of this method
        public object Resolve(InjectContext context)
        {
            ProviderBase provider;

            CheckForInstallWarning(context);

            var result = TryGetUniqueProvider(context, out provider);

            if (result == ProviderLookupResult.Multiple)
            {
                throw new ZenjectResolveException(
                    "Found multiple matches when only one was expected for type '{0}'{1}. \nObject graph:\n {2}"
                    .Fmt(
                        context.MemberType.Name(),
                        (context.ObjectType == null ? "" : " while building object with type '{0}'".Fmt(context.ObjectType.Name())),
                        context.GetObjectGraphString()));
            }

            if (result == ProviderLookupResult.None)
            {
                // If it's a generic list then try matching multiple instances to its generic type
                if (ReflectionUtil.IsGenericList(context.MemberType))
                {
                    var subType = context.MemberType.GetGenericArguments().Single();
                    var subContext = context.ChangeMemberType(subType);

                    return ResolveAll(subContext);
                }

                if (context.Optional)
                {
                    return context.FallBackValue;
                }

                throw new ZenjectResolveException(
                    "Unable to resolve type '{0}'{1}. \nObject graph:\n{2}"
                    .Fmt(
                        context.MemberType.Name() + (context.Identifier == null ? "" : " with ID '" + context.Identifier.ToString() + "'"),
                        (context.ObjectType == null ? "" : " while building object with type '{0}'".Fmt(context.ObjectType.Name())),
                        context.GetObjectGraphString()));
            }

            Assert.That(result == ProviderLookupResult.Success);
            Assert.IsNotNull(provider);

            return SafeGetInstance(provider, context);
        }

        object SafeGetInstance(ProviderBase provider, InjectContext context)
        {
            if (ChecksForCircularDependencies)
            {
                var lookupId = new LookupId(provider, context.BindingId);

                // Allow one before giving up so that you can do circular dependencies via postinject or fields
                if (_resolvesInProgress.Where(x => x.Equals(lookupId)).Count() > 1)
                {
                    throw new ZenjectResolveException(
                        "Circular dependency detected! \nObject graph:\n {0}".Fmt(context.GetObjectGraphString()));
                }

                _resolvesInProgress.Push(lookupId);
                try
                {
                    return provider.GetInstance(context);
                }
                finally
                {
                    Assert.That(_resolvesInProgress.Peek().Equals(lookupId));
                    _resolvesInProgress.Pop();
                }
            }
            else
            {
                return provider.GetInstance(context);
            }
        }

        int GetContainerHeirarchyDistance(DiContainer container)
        {
            return GetContainerHeirarchyDistance(container, 0);
        }

        int GetContainerHeirarchyDistance(DiContainer container, int depth)
        {
            if (container == this)
            {
                return depth;
            }

            Assert.IsNotNull(_parentContainer);
            return _parentContainer.GetContainerHeirarchyDistance(container, depth + 1);
        }

        public IEnumerable<Type> GetDependencyContracts<TContract>()
        {
            return GetDependencyContracts(typeof(TContract));
        }

        public IEnumerable<Type> GetDependencyContracts(Type contract)
        {
            foreach (var injectMember in TypeAnalyzer.GetInfo(contract).AllInjectables)
            {
                yield return injectMember.MemberType;
            }
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap, InjectContext currentContext, string concreteIdentifier, bool autoInject)
        {
#if PROFILING_ENABLED
            using (ProfileBlock.Start("Zenject.Instantiate({0})", concreteType))
#endif
            {
                return InstantiateInternal(concreteType, extraArgMap, currentContext, concreteIdentifier, autoInject);
            }
        }

        object InstantiateInternal(
            Type concreteType, IEnumerable<TypeValuePair> extraArgs, InjectContext currentContext, string concreteIdentifier, bool autoInject)
        {
#if !ZEN_NOT_UNITY3D
            Assert.That(!concreteType.DerivesFrom<UnityEngine.Component>(),
                "Error occurred while instantiating object of type '{0}'. Instantiator should not be used to create new mono behaviours.  Must use InstantiatePrefabForComponent, InstantiatePrefab, InstantiateComponentOnNewGameObject, InstantiateGameObject, or InstantiateComponent.  You may also want to use MonoBehaviourFactory class or plain old GameObject.Instantiate.", concreteType.Name());
#endif

            CheckForInstallWarning(currentContext);

            var typeInfo = TypeAnalyzer.GetInfo(concreteType);

            if (typeInfo.InjectConstructor == null)
            {
                throw new ZenjectResolveException(
                    "More than one (or zero) constructors found for type '{0}' when creating dependencies.  Use one [Inject] attribute to specify which to use.".Fmt(concreteType));
            }

            // Make a copy since we remove from it below
            var extraArgList = extraArgs.ToList();
            var paramValues = new List<object>();

            foreach (var injectInfo in typeInfo.ConstructorInjectables)
            {
                object value;

                if (!InstantiateUtil.PopValueWithType(extraArgList, injectInfo.MemberType, out value))
                {
                    value = Resolve(injectInfo.CreateInjectContext(this, currentContext, null, concreteIdentifier));
                }

                paramValues.Add(value);
            }

            object newObj;

            //Log.Debug("Zenject: Instantiating type '{0}'", concreteType.Name());
            try
            {
#if PROFILING_ENABLED
                using (ProfileBlock.Start("{0}.{0}()", concreteType))
#endif
                {
#if USE_EXPRESSIONS
                    newObj = typeInfo.InjectConstructor.New(paramValues.ToArray());
#else
                    newObj = typeInfo.InjectConstructor.Invoke(paramValues.ToArray());
#endif
                }
            }
            catch (Exception e)
            {
                throw new ZenjectResolveException(
                    "Error occurred while instantiating object with type '{0}'".Fmt(concreteType.Name()), e);
            }

            if (autoInject)
            {
                InjectExplicit(newObj, extraArgList, true, typeInfo, currentContext, concreteIdentifier);
            }
            else
            {
                if (!extraArgList.IsEmpty())
                {
                    throw new ZenjectResolveException(
                        "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}"
                        .Fmt(newObj.GetType().Name(), String.Join(",", extraArgList.Select(x => x.Type.Name()).ToArray()), currentContext.GetObjectGraphString()));
                }
            }

            return newObj;
        }

        // See comment in IResolver.cs for description of this method
        public void InjectExplicit(
            object injectable, IEnumerable<TypeValuePair> extraArgs,
            bool shouldUseAll, ZenjectTypeInfo typeInfo, InjectContext context,
            string concreteIdentifier)
        {
            Assert.IsEqual(typeInfo.TypeAnalyzed, injectable.GetType());
            Assert.That(injectable != null);

#if !ZEN_NOT_UNITY3D
            Assert.That(injectable.GetType() != typeof(GameObject),
                "Use InjectGameObject to Inject game objects instead of Inject method");
#endif

            CheckForInstallWarning(context);

            // Make a copy since we remove from it below
            var extraArgsList = extraArgs.ToList();

            foreach (var injectInfo in typeInfo.FieldInjectables.Concat(typeInfo.PropertyInjectables))
            {
                object value;

                if (InstantiateUtil.PopValueWithType(extraArgsList, injectInfo.MemberType, out value))
                {
                    injectInfo.Setter(injectable, value);
                }
                else
                {
                    value = Resolve(
                        injectInfo.CreateInjectContext(
                            this, context, injectable, concreteIdentifier));

                    if (injectInfo.Optional && value == null)
                    {
                        // Do not override in this case so it retains the hard-coded value
                    }
                    else
                    {
                        injectInfo.Setter(injectable, value);
                    }
                }
            }

            foreach (var method in typeInfo.PostInjectMethods)
            {
#if PROFILING_ENABLED
                using (ProfileBlock.Start("{0}.{1}()", injectable.GetType(), method.MethodInfo.Name))
#endif
                {
                    var paramValues = new List<object>();

                    foreach (var injectInfo in method.InjectableInfo)
                    {
                        object value;

                        if (!InstantiateUtil.PopValueWithType(extraArgsList, injectInfo.MemberType, out value))
                        {
                            value = Resolve(
                                injectInfo.CreateInjectContext(this, context, injectable, concreteIdentifier));
                        }

                        paramValues.Add(value);
                    }

                    method.MethodInfo.Invoke(injectable, paramValues.ToArray());
                }
            }

            if (shouldUseAll && !extraArgsList.IsEmpty())
            {
                throw new ZenjectResolveException(
                    "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}"
                    .Fmt(injectable.GetType().Name(), String.Join(",", extraArgsList.Select(x => x.Type.Name()).ToArray()), context.GetObjectGraphString()));
            }
        }

#if !ZEN_NOT_UNITY3D

        // See comment in IInstantiator.cs for description of this method
        public Component InstantiateComponent(
            Type componentType, GameObject gameObject, params object[] extraArgMap)
        {
            Assert.That(componentType.DerivesFrom<Component>());

            var monoBehaviour = (Component)gameObject.AddComponent(componentType);
            Inject(monoBehaviour, extraArgMap);
            return monoBehaviour;
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefabResourceExplicit(
            string resourcePath, IEnumerable<object> extraArgMap, InjectContext context)
        {
            return InstantiatePrefabResourceExplicit(resourcePath, extraArgMap, context, false);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefabResourceExplicit(
            string resourcePath, IEnumerable<object> extraArgMap, InjectContext context, bool includeInactive)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);
            Assert.IsNotNull(prefab, "Could not find prefab at resource location '{0}'".Fmt(resourcePath));
            return InstantiatePrefabExplicit(prefab, extraArgMap, context, includeInactive);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefabExplicit(
            GameObject prefab, IEnumerable<object> extraArgMap, InjectContext context)
        {
            return InstantiatePrefabExplicit(prefab, extraArgMap, context, false);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefabExplicit(
            GameObject prefab, IEnumerable<object> extraArgMap, InjectContext context, bool includeInactive)
        {
            return InstantiatePrefabExplicit(prefab, extraArgMap, context, includeInactive, null);
        }

        GameObject InstantiateAndParentPrefabResource(string resourcePath)
        {
            return InstantiateAndParentPrefabResource(resourcePath, null);
        }

        GameObject InstantiateAndParentPrefabResource(string resourcePath, string groupName)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);

            Assert.IsNotNull(prefab,
                "Could not find prefab at resource location '{0}'".Fmt(resourcePath));

            return InstantiateAndParentPrefab(prefab, groupName);
        }

        GameObject InstantiateAndParentPrefab(GameObject prefab, string groupName)
        {
            var gameObj = (GameObject)GameObject.Instantiate(prefab);

            gameObj.transform.SetParent(GetTransformGroup(groupName), false);

            gameObj.SetActive(true);

            return gameObj;
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefabExplicit(
            GameObject prefab, IEnumerable<object> extraArgMap, InjectContext context, bool includeInactive, string groupName)
        {
            var gameObj = InstantiateAndParentPrefab(prefab, groupName);

            InjectGameObject(gameObj, true, includeInactive, extraArgMap, context);

            return gameObj;
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiateGameObject(string name)
        {
            var gameObj = new GameObject(name);
            gameObj.transform.SetParent(DefaultParent, false);
            return gameObj;
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiateComponentOnNewGameObjectExplicit(
            Type componentType, string name, List<TypeValuePair> extraArgMap, InjectContext currentContext)
        {
            Assert.That(componentType.DerivesFrom<Component>(), "Expected type '{0}' to derive from UnityEngine.Component", componentType.Name());

            var gameObj = InstantiateGameObject(name);

            if (componentType == typeof(Transform))
            {
                Assert.That(extraArgMap.IsEmpty());
                return gameObj.transform;
            }

            var component = (Component)gameObj.AddComponent(componentType);

            InjectExplicit(component, extraArgMap, currentContext);

            return component;
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabResourceForComponentExplicit(
            Type componentType, string resourcePath, List<TypeValuePair> extraArgs, InjectContext currentContext)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);
            Assert.IsNotNull(prefab, "Could not find prefab at resource location '{0}'".Fmt(resourcePath));
            return InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, currentContext);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabForComponentExplicit(
            Type componentType, GameObject prefab, List<TypeValuePair> extraArgs,
            InjectContext currentContext)
        {
            return InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, currentContext, false);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabForComponentExplicit(
            Type componentType, GameObject prefab, List<TypeValuePair> extraArgs,
            InjectContext currentContext, bool includeInactive)
        {
            return InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, currentContext, includeInactive, null);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabForComponentExplicit(
            Type componentType, GameObject prefab, List<TypeValuePair> extraArgs,
            InjectContext currentContext, bool includeInactive, string groupName)
        {
            Assert.That(prefab != null, "Null prefab found when instantiating game object");

            Assert.That(componentType.IsInterface || componentType.DerivesFrom<Component>(),
                "Expected type '{0}' to derive from UnityEngine.Component", componentType.Name());

            var gameObj = (GameObject)GameObject.Instantiate(prefab);

            try
            {
                gameObj.transform.SetParent(GetTransformGroup(groupName), false);

                gameObj.SetActive(true);

                Component requestedScript = null;

                // Inject on the children first since the parent objects are more likely to use them in their post inject methods
                foreach (var component in ZenUtilInternal.GetInjectableComponentsBottomUp(gameObj, true, includeInactive).ToList())
                {
                    if (component == null)
                    {
                        Log.Warn("Found null component while instantiating prefab '{0}'.  Possible missing script.", prefab.name);
                        continue;
                    }

                    if (component.GetType().DerivesFrom<MonoInstaller>())
                    {
                        // Do not inject on installers since these are always injected before they are installed
                        continue;
                    }

                    if (component.GetType().DerivesFromOrEqual(componentType))
                    {
                        Assert.IsNull(requestedScript,
                            "Found multiple matches with type '{0}' when instantiating new game object from prefab '{1}'", componentType, prefab.name);
                        requestedScript = component;

                        InjectExplicit(component, extraArgs);
                    }
                    else
                    {
                        Inject(component);
                    }
                }

                if (requestedScript == null)
                {
                    throw new ZenjectResolveException(
                        "Could not find component with type '{0}' when instantiating new game object".Fmt(componentType));
                }

                return requestedScript;
            }
            catch (Exception e)
            {
                // If we do get exceptions don't leave half-initialized objects around
                GameObject.DestroyImmediate(gameObj);

                throw new Exception(
                    "Error while instantiating prefab '{0}'".Fmt(prefab.name), e);
            }
        }

        Transform GetTransformGroup(string groupName)
        {
            if (DefaultParent == null)
            {
                if (groupName == null)
                {
                    return null;
                }

                return (GameObject.Find("/" + groupName) ?? new GameObject(groupName)).transform;
            }

            if (groupName == null)
            {
                return DefaultParent;
            }

            foreach (Transform child in DefaultParent)
            {
                if (child.name == groupName)
                {
                    return child;
                }
            }

            var group = new GameObject(groupName).transform;
            group.SetParent(DefaultParent, false);
            return group;
        }

#endif

        // See comment in IInstantiator.cs for description of this method
        public T Instantiate<T>(
            params object[] extraArgs)
        {
            return (T)Instantiate(typeof(T), extraArgs);
        }

        // See comment in IInstantiator.cs for description of this method
        public object Instantiate(
            Type concreteType, params object[] extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateExplicit", concreteType);

            return InstantiateExplicit(
                concreteType, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // See comment in IInstantiator.cs for description of this method
        public T InstantiateExplicit<T>(
            List<TypeValuePair> extraArgMap)
        {
            return (T)InstantiateExplicit(typeof(T), extraArgMap);
        }

        // See comment in IInstantiator.cs for description of this method
        public T InstantiateExplicit<T>(
            List<TypeValuePair> extraArgMap, InjectContext context)
        {
            return (T)InstantiateExplicit(
                typeof(T), extraArgMap, context);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap)
        {
            return InstantiateExplicit(
                concreteType, extraArgMap, new InjectContext(this, concreteType, null));
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap, InjectContext context)
        {
            return InstantiateExplicit(
                concreteType, extraArgMap, context, null, true);
        }

#if !ZEN_NOT_UNITY3D
        // See comment in IInstantiator.cs for description of this method
        public TContract InstantiateComponent<TContract>(
            GameObject gameObject, params object[] args)
            where TContract : Component
        {
            return (TContract)InstantiateComponent(typeof(TContract), gameObject, args);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefab(
            GameObject prefab, params object[] args)
        {
            return InstantiatePrefabExplicit(prefab, args, null);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefab(
            string groupName, GameObject prefab, params object[] args)
        {
            return InstantiatePrefabExplicit(prefab, args, null, false, groupName);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefab(
            bool includeInactive, GameObject prefab, params object[] args)
        {
            return InstantiatePrefabExplicit(prefab, args, null, includeInactive);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefab(
            bool includeInactive, string groupName, GameObject prefab, params object[] args)
        {
            return InstantiatePrefabExplicit(prefab, args, null, includeInactive, groupName);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefabResource(
            string resourcePath, params object[] args)
        {
            return InstantiatePrefabResourceExplicit(resourcePath, args, null, false);
        }

        // See comment in IInstantiator.cs for description of this method
        public GameObject InstantiatePrefabResource(
            bool includeInactive, string resourcePath, params object[] args)
        {
            return InstantiatePrefabResourceExplicit(resourcePath, args, null, includeInactive);
        }

        /////////////// InstantiatePrefabForComponent

        // See comment in IInstantiator.cs for description of this method
        public T InstantiatePrefabForComponent<T>(
            GameObject prefab, params object[] extraArgs)
        {
            return (T)InstantiatePrefabForComponent(typeof(T), prefab, extraArgs);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabForComponent(
            Type concreteType, GameObject prefab, params object[] extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiatePrefabForComponentExplicit", concreteType);

            return InstantiatePrefabForComponentExplicit(
                concreteType, prefab, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // See comment in IInstantiator.cs for description of this method
        public T InstantiatePrefabForComponent<T>(
            bool includeInactive, GameObject prefab, params object[] extraArgs)
        {
            return (T)InstantiatePrefabForComponent(includeInactive, typeof(T), prefab, extraArgs);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabForComponent(
            bool includeInactive, Type concreteType, GameObject prefab, params object[] extraArgs)
        {
            Assert.That(!extraArgs.Contains(null),
            "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiatePrefabForComponentExplicit", concreteType);

            return InstantiatePrefabForComponentExplicit(
                concreteType, prefab,
                InstantiateUtil.CreateTypeValueList(extraArgs),
                new InjectContext(this, concreteType, null), includeInactive);
        }

        // See comment in IInstantiator.cs for description of this method
        public T InstantiatePrefabForComponentExplicit<T>(
            GameObject prefab, List<TypeValuePair> extraArgMap)
        {
            return (T)InstantiatePrefabForComponentExplicit(typeof(T), prefab, extraArgMap);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabForComponentExplicit(
            Type concreteType, GameObject prefab, List<TypeValuePair> extraArgMap)
        {
            return InstantiatePrefabForComponentExplicit(
                concreteType, prefab, extraArgMap, new InjectContext(this, concreteType, null));
        }


        /////////////// InstantiatePrefabForComponent

        // See comment in IInstantiator.cs for description of this method
        public T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, params object[] extraArgs)
        {
            return (T)InstantiatePrefabResourceForComponent(typeof(T), resourcePath, extraArgs);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabResourceForComponent(
            Type concreteType, string resourcePath, params object[] extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
            "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiatePrefabForComponentExplicit", concreteType);

            return InstantiatePrefabResourceForComponentExplicit(
                concreteType, resourcePath, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // See comment in IInstantiator.cs for description of this method
        public T InstantiatePrefabResourceForComponentExplicit<T>(
            string resourcePath, List<TypeValuePair> extraArgMap)
        {
            return (T)InstantiatePrefabResourceForComponentExplicit(typeof(T), resourcePath, extraArgMap);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiatePrefabResourceForComponentExplicit(
            Type concreteType, string resourcePath, List<TypeValuePair> extraArgMap)
        {
            return InstantiatePrefabResourceForComponentExplicit(
                concreteType, resourcePath, extraArgMap, new InjectContext(this, concreteType, null));
        }

        /////////////// InstantiateComponentOnNewGameObject

        // See comment in IInstantiator.cs for description of this method
        public T InstantiateComponentOnNewGameObject<T>(
            string name, params object[] extraArgs)
        {
            return (T)InstantiateComponentOnNewGameObject(typeof(T), name, extraArgs);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiateComponentOnNewGameObject(
            Type concreteType, string name, params object[] extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateComponentOnNewGameObjectExplicit", concreteType);

            return InstantiateComponentOnNewGameObjectExplicit(
                concreteType, name, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // See comment in IInstantiator.cs for description of this method
        public T InstantiateComponentOnNewGameObjectExplicit<T>(
            string name, List<TypeValuePair> extraArgMap)
        {
            return (T)InstantiateComponentOnNewGameObjectExplicit(typeof(T), name, extraArgMap);
        }

        // See comment in IInstantiator.cs for description of this method
        public object InstantiateComponentOnNewGameObjectExplicit(
            Type concreteType, string name, List<TypeValuePair> extraArgMap)
        {
            return InstantiateComponentOnNewGameObjectExplicit(
                concreteType, name, extraArgMap, new InjectContext(this, concreteType, null));
        }
#endif

        ////////////// Convenience methods for IResolver ////////////////

#if !ZEN_NOT_UNITY3D
        // See comment in IResolver.cs for description of this method
        public void InjectGameObject(
            GameObject gameObject, bool recursive, bool includeInactive)
        {
            InjectGameObject(gameObject, recursive, includeInactive, Enumerable.Empty<object>());
        }

        // See comment in IResolver.cs for description of this method
        public void InjectGameObject(
            GameObject gameObject, bool recursive)
        {
            InjectGameObject(gameObject, recursive, false);
        }

        // See comment in IResolver.cs for description of this method
        public void InjectGameObject(
            GameObject gameObject)
        {
            InjectGameObject(gameObject, true, false);
        }

        // See comment in IResolver.cs for description of this method
        public void InjectGameObject(
            GameObject gameObject,
            bool recursive, bool includeInactive, IEnumerable<object> extraArgs)
        {
            InjectGameObject(
                gameObject, recursive, includeInactive, extraArgs, null);
        }

        // See comment in IResolver.cs for description of this method
        public void InjectGameObject(
            GameObject gameObject, bool recursive, bool includeInactive,
            IEnumerable<object> extraArgs, InjectContext context)
        {
            // Inject on the children first since the parent objects are more likely to use them in their post inject methods
            foreach (var component in ZenUtilInternal.GetInjectableComponentsBottomUp(gameObject, recursive, includeInactive).ToList())
            {
                if (component == null)
                {
                    Log.Warn("Found null component while injecting game object '{0}'.  Possible missing script.", gameObject.name);
                    continue;
                }

                if (component.GetType().DerivesFrom<MonoInstaller>())
                {
                    // Do not inject on installers since these are always injected before they are installed
                    continue;
                }

                Inject(component, extraArgs, false, context);
            }
        }
#endif

        // See comment in IResolver.cs for description of this method
        public void Inject(object injectable)
        {
            Inject(injectable, Enumerable.Empty<object>());
        }

        // See comment in IResolver.cs for description of this method
        public void Inject(object injectable, IEnumerable<object> additional)
        {
            Inject(injectable, additional, true);
        }

        // See comment in IResolver.cs for description of this method
        public void Inject(object injectable, IEnumerable<object> additional, bool shouldUseAll)
        {
            Inject(
                injectable, additional, shouldUseAll, new InjectContext(this, injectable.GetType(), null));
        }

        // See comment in IResolver.cs for description of this method
        public void Inject(
            object injectable, IEnumerable<object> additional, bool shouldUseAll, InjectContext context)
        {
            Inject(
                injectable, additional, shouldUseAll, context, TypeAnalyzer.GetInfo(injectable.GetType()));
        }

        // See comment in IResolver.cs for description of this method
        public void Inject(
            object injectable, IEnumerable<object> additional, bool shouldUseAll,
            InjectContext context, ZenjectTypeInfo typeInfo)
        {
            Assert.That(!additional.ContainsItem(null),
                "Null value given to injection argument list. In order to use null you must provide a List<TypeValuePair> and not just a list of objects");

            InjectExplicit(
                injectable, InstantiateUtil.CreateTypeValueList(additional),
                shouldUseAll, typeInfo, context, null);
        }

        // See comment in IResolver.cs for description of this method
        public void InjectExplicit(object injectable, List<TypeValuePair> additional)
        {
            InjectExplicit(
                injectable, additional, new InjectContext(this, injectable.GetType(), null));
        }

        // See comment in IResolver.cs for description of this method
        public void InjectExplicit(object injectable, List<TypeValuePair> additional, InjectContext context)
        {
            InjectExplicit(
                injectable, additional, true,
                TypeAnalyzer.GetInfo(injectable.GetType()), context, null);
        }

        // See comment in IResolver.cs for description of this method
        public List<Type> ResolveTypeAll(Type type)
        {
            return ResolveTypeAll(new InjectContext(this, type, null));
        }

        // See comment in IResolver.cs for description of this method
        public TContract Resolve<TContract>()
        {
            return Resolve<TContract>((string)null);
        }

        // See comment in IResolver.cs for description of this method
        public TContract Resolve<TContract>(string identifier)
        {
            return Resolve<TContract>(new InjectContext(this, typeof(TContract), identifier));
        }

        // See comment in IResolver.cs for description of this method
        public TContract TryResolve<TContract>()
            where TContract : class
        {
            return TryResolve<TContract>((string)null);
        }

        // See comment in IResolver.cs for description of this method
        public TContract TryResolve<TContract>(string identifier)
            where TContract : class
        {
            return (TContract)TryResolve(typeof(TContract), identifier);
        }

        // See comment in IResolver.cs for description of this method
        public object TryResolve(Type contractType)
        {
            return TryResolve(contractType, null);
        }

        // See comment in IResolver.cs for description of this method
        public object TryResolve(Type contractType, string identifier)
        {
            return Resolve(new InjectContext(this, contractType, identifier, true));
        }

        // See comment in IResolver.cs for description of this method
        public object Resolve(Type contractType)
        {
            return Resolve(new InjectContext(this, contractType, null));
        }

        // See comment in IResolver.cs for description of this method
        public object Resolve(Type contractType, string identifier)
        {
            return Resolve(new InjectContext(this, contractType, identifier));
        }

        // See comment in IResolver.cs for description of this method
        public TContract Resolve<TContract>(InjectContext context)
        {
            Assert.IsEqual(context.MemberType, typeof(TContract));
            return (TContract) Resolve(context);
        }

        // See comment in IResolver.cs for description of this method
        public List<TContract> ResolveAll<TContract>()
        {
            return ResolveAll<TContract>((string)null);
        }

        // See comment in IResolver.cs for description of this method
        public List<TContract> ResolveAll<TContract>(bool optional)
        {
            return ResolveAll<TContract>(null, optional);
        }

        // See comment in IResolver.cs for description of this method
        public List<TContract> ResolveAll<TContract>(string identifier)
        {
            return ResolveAll<TContract>(identifier, false);
        }

        // See comment in IResolver.cs for description of this method
        public List<TContract> ResolveAll<TContract>(string identifier, bool optional)
        {
            var context = new InjectContext(this, typeof(TContract), identifier, optional);
            return ResolveAll<TContract>(context);
        }

        // See comment in IResolver.cs for description of this method
        public List<TContract> ResolveAll<TContract>(InjectContext context)
        {
            Assert.IsEqual(context.MemberType, typeof(TContract));
            return (List<TContract>) ResolveAll(context);
        }

        // See comment in IResolver.cs for description of this method
        public IList ResolveAll(Type contractType)
        {
            return ResolveAll(contractType, null);
        }

        // See comment in IResolver.cs for description of this method
        public IList ResolveAll(Type contractType, string identifier)
        {
            return ResolveAll(contractType, identifier, false);
        }

        // See comment in IResolver.cs for description of this method
        public IList ResolveAll(Type contractType, bool optional)
        {
            return ResolveAll(contractType, null, optional);
        }

        // See comment in IResolver.cs for description of this method
        public IList ResolveAll(Type contractType, string identifier, bool optional)
        {
            var context = new InjectContext(this, contractType, identifier, optional);
            return ResolveAll(context);
        }

        ////////////// IBinder ////////////////

        // See comment in IBinder.cs for description of this method
        public void UnbindAll()
        {
            foreach (var provider in _providers.Values.SelectMany(x => x))
            {
                provider.Dispose();
            }

            _providers.Clear();
        }

        // See comment in IBinder.cs for description of this method
        public bool Unbind<TContract>()
        {
            return Unbind<TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public bool Unbind<TContract>(string identifier)
        {
            List<ProviderBase> providersToRemove;
            var bindingId = new BindingId(typeof(TContract), identifier);

            if (_providers.TryGetValue(bindingId, out providersToRemove))
            {
                _providers.Remove(bindingId);

                // Only dispose if the provider is not bound to another type
                foreach (var provider in providersToRemove)
                {
                    if (_providers.Where(x => x.Value.ContainsItem(provider)).IsEmpty())
                    {
                        provider.Dispose();
                    }
                }

                return true;
            }

            return false;
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindInstance<TContract>(string identifier, TContract obj)
        {
            return Bind<TContract>(identifier).ToInstance(obj);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindInstance<TContract>(TContract obj)
        {
            return Bind<TContract>().ToInstance(obj);
        }

        // See comment in IBinder.cs for description of this method
        public IGenericBinder<TContract> Bind<TContract>()
        {
            return Bind<TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public IUntypedBinder Bind(Type contractType)
        {
            return Bind(contractType, (string)null);
        }

        // See comment in IBinder.cs for description of this method
        public IUntypedBinder Bind(Type contractType, string identifier)
        {
            return Bind(new List<Type>() { contractType }, identifier);
        }

        // See comment in IBinder.cs for description of this method
        public IUntypedBinder Bind(params Type[] contractTypes)
        {
            return Bind((string)null, contractTypes);
        }

        // See comment in IBinder.cs for description of this method
        public IUntypedBinder Bind(string identifier, params Type[] contractTypes)
        {
            return new UntypedBinder(this, contractTypes.ToList(), identifier);
        }

        // See comment in IBinder.cs for description of this method
        public IUntypedBinder Bind(IEnumerable<Type> contractTypes, string identifier)
        {
            return new UntypedBinder(this, contractTypes.ToList(), identifier);
        }

        public IUntypedBinder BindAllInterfaces<T>()
        {
            return BindAllInterfaces(typeof(T));
        }

        // See comment in IBinder.cs for description of this method
        public IUntypedBinder BindAllInterfaces(Type type)
        {
            // We must only have one dependency root per container
            // We need this when calling this with a facade
            return Bind(type.GetInterfaces().Except(typeof(IDependencyRoot)).ToArray());
        }

        // See comment in IBinder.cs for description of this method
        public IUntypedBinder BindAllInterfacesAndSelf<T>()
        {
            return BindAllInterfacesAndSelf(typeof(T));
        }

        // See comment in IBinder.cs for description of this method
        public IUntypedBinder BindAllInterfacesAndSelf(Type type)
        {
            // We must only have one dependency root per container
            // We need this when calling this with a facade
            return Bind(
                type.GetInterfaces().Except(typeof(IDependencyRoot)).Append(type).ToArray());
        }

        // See comment in IBinder.cs for description of this method
        public bool HasBinding(InjectContext context)
        {
            List<ProviderBase> providers;

            if (!_providers.TryGetValue(context.BindingId, out providers))
            {
                return false;
            }

            return providers.Where(x => x.Matches(context)).HasAtLeast(1);
        }

        // See comment in IBinder.cs for description of this method
        public bool HasBinding<TContract>()
        {
            return HasBinding<TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public bool HasBinding<TContract>(string identifier)
        {
            return HasBinding(
                new InjectContext(this, typeof(TContract), identifier));
        }

        // See comment in IBinder.cs for description of this method
        public IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>()
        {
            return BindIFactoryUntyped<TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>(string identifier)
        {
            return BindIFactoryUntyped<TContract>(identifier, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>(string identifier, ContainerTypes containerType)
        {
            return new IFactoryUntypedBinder<TContract>(this, identifier, containerType);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TContract> BindIFactory<TContract>()
        {
            return BindIFactory<TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TContract> BindIFactory<TContract>(string identifier)
        {
            return BindIFactory<TContract>(identifier, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TContract> BindIFactory<TContract>(string identifier, ContainerTypes containerType)
        {
            return new IFactoryBinder<TContract>(this, identifier, containerType);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TContract> BindIFactory<TParam1, TContract>()
        {
            return BindIFactory<TParam1, TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TContract> BindIFactory<TParam1, TContract>(string identifier)
        {
            return BindIFactory<TParam1, TContract>(identifier, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TContract> BindIFactory<TParam1, TContract>(string identifier, ContainerTypes containerType)
        {
            return new IFactoryBinder<TParam1, TContract>(this, identifier, containerType);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>()
        {
            return BindIFactory<TParam1, TParam2, TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>(string identifier)
        {
            return BindIFactory<TParam1, TParam2, TContract>(identifier, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>(string identifier, ContainerTypes containerType)
        {
            return new IFactoryBinder<TParam1, TParam2, TContract>(this, identifier, containerType);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>()
        {
            return BindIFactory<TParam1, TParam2, TParam3, TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>(string identifier)
        {
            return BindIFactory<TParam1, TParam2, TParam3, TContract>(identifier, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>(string identifier, ContainerTypes containerType)
        {
            return new IFactoryBinder<TParam1, TParam2, TParam3, TContract>(this, identifier, containerType);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>()
        {
            return BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(null);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(string identifier)
        {
            return BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(identifier, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(string identifier, ContainerTypes containerType)
        {
            return new IFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract>(this, identifier, containerType);
        }

        // See comment in IBinder.cs for description of this method
        public IGenericBinder<TContract> Rebind<TContract>()
        {
            Unbind<TContract>();
            return Bind<TContract>();
        }

        // See comment in IBinder.cs for description of this method
        public IGenericBinder<TContract> Bind<TContract>(string identifier)
        {
            return new GenericBinder<TContract>(this, identifier);
        }

#if !ZEN_NOT_UNITY3D

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindMonoFacadeFactory<TFactory>(
            GameObject prefab)
            where TFactory : IMonoFacadeFactoryZeroParams
        {
            return BindMonoFacadeFactory<TFactory>(prefab, null);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindMonoFacadeFactory<TFactory>(
            GameObject prefab, string groupName)
            where TFactory : IMonoFacadeFactoryZeroParams
        {
            return BindMonoFacadeFactory<TFactory>(prefab, groupName, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindMonoFacadeFactory<TFactory>(
            GameObject prefab, string groupName, ContainerTypes containerType)
            where TFactory : IMonoFacadeFactoryZeroParams
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindMonoFacadeFactory for type '{0}'".Fmt(typeof(TFactory).Name()));
            }

            // We could bind the factory ToSingle but doing it this way is better
            // since it allows us to have multiple game object factories that
            // use different prefabs and have them injected into different places
            return Bind<TFactory>().ToMethod((ctx) => (containerType == ContainerTypes.RuntimeContainer ? ctx.Container : this).InstantiateExplicit<TFactory>(
                new List<TypeValuePair>() { InstantiateUtil.CreateTypePair(prefab), InstantiateUtil.CreateTypePair(groupName) }));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindMonoFacadeFactory<TInstaller, TFactory>(
            GameObject prefab)
            where TInstaller : MonoInstaller
            where TFactory : IMonoFacadeFactoryMultipleParams
        {
            return BindMonoFacadeFactory<TInstaller, TFactory>(prefab, null);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindMonoFacadeFactory<TInstaller, TFactory>(
            GameObject prefab, string groupName)
            where TInstaller : MonoInstaller
            where TFactory : IMonoFacadeFactoryMultipleParams
        {
            return BindMonoFacadeFactory<TInstaller, TFactory>(prefab, groupName, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindMonoFacadeFactory<TInstaller, TFactory>(
            GameObject prefab, string groupName, ContainerTypes containerType)
            where TInstaller : MonoInstaller
            where TFactory : IMonoFacadeFactoryMultipleParams
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindMonoFacadeFactory for type '{0}'".Fmt(typeof(TFactory).Name()));
            }

            // We could bind the factory ToSingle but doing it this way is better
            // since it allows us to have multiple game object factories that
            // use different prefabs and have them injected into different places
            return Bind<TFactory>().ToMethod((ctx) => (containerType == ContainerTypes.RuntimeContainer ? ctx.Container : this).InstantiateExplicit<TFactory>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(prefab),
                    InstantiateUtil.CreateTypePair(groupName),
                    InstantiateUtil.CreateTypePair(typeof(TInstaller)),
                }));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindMonoBehaviourFactory<TFactory>(GameObject prefab)
            where TFactory : IMonoBehaviourFactory
        {
            return BindMonoBehaviourFactory<TFactory>(prefab, null);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            GameObject prefab, string groupName)
            where TFactory : IMonoBehaviourFactory
        {
            return BindMonoBehaviourFactory<TFactory>((string)null, prefab, groupName);
        }

        public BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            string identifier, GameObject prefab, string groupName)
            where TFactory : IMonoBehaviourFactory
        {
            return BindMonoBehaviourFactory<TFactory>(
                identifier, prefab, groupName, ContainerTypes.RuntimeContainer);
        }

        public BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            GameObject prefab, string groupName, ContainerTypes createContainer)
            where TFactory : IMonoBehaviourFactory
        {
            return BindMonoBehaviourFactory<TFactory>(
                (string)null, prefab, groupName, createContainer);
        }

        public BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            string identifier, GameObject prefab, string groupName, ContainerTypes createContainer)
            where TFactory : IMonoBehaviourFactory
        {
            return BindMonoBehaviourFactory(
                typeof(TFactory), identifier, prefab, groupName, createContainer);
        }

        public BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, GameObject prefab)
        {
            return BindMonoBehaviourFactory(factoryType, prefab, null);
        }

        public BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, GameObject prefab, string groupName)
        {
            return BindMonoBehaviourFactory(factoryType, null, prefab, groupName);
        }

        public BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, string identifier, GameObject prefab, string groupName)
        {
            return BindMonoBehaviourFactory(factoryType, identifier, prefab, groupName, ContainerTypes.RuntimeContainer);
        }

        public BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, GameObject prefab, string groupName, ContainerTypes createContainer)
        {
            return BindMonoBehaviourFactory(
                factoryType, null, prefab, groupName, createContainer);
        }

        public BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, string identifier, GameObject prefab,
            string groupName, ContainerTypes createContainer)
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindMonoBehaviourFactory for type '{0}'".Fmt(factoryType.Name()));
            }

            if (!factoryType.DerivesFrom<IMonoBehaviourFactory>())
            {
                throw new ZenjectBindException(
                    "Expected given type '{0}' to derive from 'IMonoBehaviourFactory'"
                    .Fmt(factoryType.Name()));
            }

            // We could bind the factory ToSingle but doing it this way is better
            // since it allows us to have multiple game object factories that
            // use different prefabs and have them injected into different places
            return Bind(factoryType, identifier).ToMethod(
                factoryType, (ctx) => (createContainer == ContainerTypes.BindContainer ? this : ctx.Container).InstantiateExplicit(
                    factoryType,
                    new List<TypeValuePair>()
                    {
                        InstantiateUtil.CreateTypePair(prefab),
                        InstantiateUtil.CreateTypePair(groupName)
                    }));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindGameObjectFactory<TFactory>(GameObject prefab)
            where TFactory : GameObjectFactory
        {
            return BindGameObjectFactory<TFactory>(prefab, null);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindGameObjectFactory<TFactory>(
            GameObject prefab, string groupName)
            where TFactory : GameObjectFactory
        {
            return BindGameObjectFactory<TFactory>((string)null, prefab, groupName);
        }

        public BindingConditionSetter BindGameObjectFactory<TFactory>(
            string identifier, GameObject prefab, string groupName)
            where TFactory : GameObjectFactory
        {
            return BindGameObjectFactory<TFactory>(
                identifier, prefab, groupName, ContainerTypes.RuntimeContainer);
        }

        public BindingConditionSetter BindGameObjectFactory<TFactory>(
            GameObject prefab, string groupName, ContainerTypes createContainer)
            where TFactory : GameObjectFactory
        {
            return BindGameObjectFactory<TFactory>(
                (string)null, prefab, groupName, createContainer);
        }

        public BindingConditionSetter BindGameObjectFactory<TFactory>(
            string identifier, GameObject prefab, string groupName, ContainerTypes createContainer)
            where TFactory : GameObjectFactory
        {
            return BindGameObjectFactory(
                typeof(TFactory), identifier, prefab, groupName, createContainer);
        }

        public BindingConditionSetter BindGameObjectFactory(
            Type factoryType, GameObject prefab)
        {
            return BindGameObjectFactory(factoryType, prefab, null);
        }

        public BindingConditionSetter BindGameObjectFactory(
            Type factoryType, GameObject prefab, string groupName)
        {
            return BindGameObjectFactory(factoryType, null, prefab, groupName);
        }

        public BindingConditionSetter BindGameObjectFactory(
            Type factoryType, string identifier, GameObject prefab, string groupName)
        {
            return BindGameObjectFactory(factoryType, identifier, prefab, groupName, ContainerTypes.RuntimeContainer);
        }

        public BindingConditionSetter BindGameObjectFactory(
            Type factoryType, GameObject prefab, string groupName, ContainerTypes createContainer)
        {
            return BindGameObjectFactory(
                factoryType, null, prefab, groupName, createContainer);
        }

        public BindingConditionSetter BindGameObjectFactory(
            Type factoryType, string identifier, GameObject prefab,
            string groupName, ContainerTypes createContainer)
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindGameObjectFactory for type '{0}'".Fmt(factoryType.Name()));
            }

            if (!factoryType.DerivesFrom<GameObjectFactory>())
            {
                throw new ZenjectBindException(
                    "Expected given type '{0}' to derive from 'GameObjectFactory'"
                    .Fmt(factoryType.Name()));
            }

            // We could bind the factory ToSingle but doing it this way is better
            // since it allows us to have multiple game object factories that
            // use different prefabs and have them injected into different places
            return Bind(factoryType, identifier).ToMethod(
                factoryType, (ctx) => (createContainer == ContainerTypes.BindContainer ? this : ctx.Container).InstantiateExplicit(
                    factoryType,
                    new List<TypeValuePair>()
                    {
                        InstantiateUtil.CreateTypePair(prefab),
                        InstantiateUtil.CreateTypePair(groupName)
                    }));
        }
#endif

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TFacadeFactory>(
            Action<DiContainer> facadeInstaller)
            where TFacadeFactory : IFacadeFactoryParams
        {
            return BindFacadeFactoryMethod<TFacadeFactory>(facadeInstaller, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TFacadeFactory>(
            Action<DiContainer> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : IFacadeFactoryParams
        {
            return Bind<TFacadeFactory>().ToMethod(
                x => (containerType == ContainerTypes.RuntimeContainer ? x.Container : this).Instantiate<TFacadeFactory>(facadeInstaller));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TFacadeFactory>(
            Action<DiContainer, TParam1> facadeInstaller)
            where TFacadeFactory : IFacadeFactoryParams<TParam1>
        {
            return BindFacadeFactoryMethod<TParam1, TFacadeFactory>(facadeInstaller, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TFacadeFactory>(
            Action<DiContainer, TParam1> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : IFacadeFactoryParams<TParam1>
        {
            return Bind<TFacadeFactory>().ToMethod(
                x => (containerType == ContainerTypes.RuntimeContainer ? x.Container : this).Instantiate<TFacadeFactory>(facadeInstaller));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TFacadeFactory>(
            Action<DiContainer, TParam1, TParam2> facadeInstaller)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2>
        {
            return BindFacadeFactoryMethod<TParam1, TParam2, TFacadeFactory>(facadeInstaller, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TFacadeFactory>(
            Action<DiContainer, TParam1, TParam2> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2>
        {
            return Bind<TFacadeFactory>().ToMethod(
                x => (containerType == ContainerTypes.RuntimeContainer ? x.Container : this).Instantiate<TFacadeFactory>(facadeInstaller));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TFacadeFactory>(
            Action<DiContainer, TParam1, TParam2, TParam3> facadeInstaller)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2, TParam3>
        {
            return BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TFacadeFactory>(facadeInstaller, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TFacadeFactory>(
            Action<DiContainer, TParam1, TParam2, TParam3> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2, TParam3>
        {
            return Bind<TFacadeFactory>().ToMethod(
                x => (containerType == ContainerTypes.RuntimeContainer ? x.Container : this).Instantiate<TFacadeFactory>(facadeInstaller));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> facadeInstaller)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2, TParam3, TParam4>
        {
            return BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TFacadeFactory>(facadeInstaller, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2, TParam3, TParam4>
        {
            return Bind<TFacadeFactory>().ToMethod(
                x => (containerType == ContainerTypes.RuntimeContainer ? x.Container : this).Instantiate<TFacadeFactory>(facadeInstaller));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> facadeInstaller)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TFacadeFactory>(facadeInstaller, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return Bind<TFacadeFactory>().ToMethod(
                x => (containerType == ContainerTypes.RuntimeContainer ? x.Container : this).Instantiate<TFacadeFactory>(facadeInstaller));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> facadeInstaller)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacadeFactory>(facadeInstaller, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : IFacadeFactoryParams<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return Bind<TFacadeFactory>().ToMethod(
                x => (containerType == ContainerTypes.RuntimeContainer ? x.Container : this).Instantiate<TFacadeFactory>(facadeInstaller));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryInstaller<TFacadeFactory, TInstaller>()
            where TFacadeFactory : IFacadeFactory
            where TInstaller : Installer
        {
            return BindFacadeFactoryInstaller<TFacadeFactory>(typeof(TInstaller));
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryInstaller<TFacadeFactory>(Type installerType)
            where TFacadeFactory : IFacadeFactory
        {
            return BindFacadeFactoryInstaller<TFacadeFactory>(installerType, ContainerTypes.RuntimeContainer);
        }

        // See comment in IBinder.cs for description of this method
        public BindingConditionSetter BindFacadeFactoryInstaller<TFacadeFactory>(Type installerType, ContainerTypes containerType)
            where TFacadeFactory : IFacadeFactory
        {
            return Bind<TFacadeFactory>().ToMethod(
                x => (containerType == ContainerTypes.RuntimeContainer ? x.Container : this).InstantiateExplicit<TFacadeFactory>(
                    InstantiateUtil.CreateTypeValueListExplicit(installerType)));
        }

        ////////////// Other ////////////////

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateResolve(InjectContext context)
        {
            CheckForInstallWarning(context);

            ProviderBase provider = null;
            var result = TryGetUniqueProvider(context, out provider);

            if (result == DiContainer.ProviderLookupResult.Success)
            {
                Assert.IsNotNull(provider);

                if (ChecksForCircularDependencies)
                {
                    var lookupId = new LookupId(provider, context.BindingId);

                    // Allow one before giving up so that you can do circular dependencies via postinject or fields
                    if (_resolvesInProgress.Where(x => x.Equals(lookupId)).Count() > 1)
                    {
                        yield return new ZenjectResolveException(
                            "Circular dependency detected! \nObject graph:\n {0}".Fmt(context.GetObjectGraphString()));
                    }

                    _resolvesInProgress.Push(lookupId);
                    try
                    {
                        foreach (var error in provider.ValidateBinding(context))
                        {
                            yield return error;
                        }
                    }
                    finally
                    {
                        Assert.That(_resolvesInProgress.Peek().Equals(lookupId));
                        _resolvesInProgress.Pop();
                    }
                }
                else
                {
                    foreach (var error in provider.ValidateBinding(context))
                    {
                        yield return error;
                    }
                }
            }
            else if (result == DiContainer.ProviderLookupResult.Multiple)
            {
                yield return new ZenjectResolveException(
                    "Found multiple matches when only one was expected for dependency with type '{0}'{1} \nObject graph:\n{2}"
                    .Fmt(
                        context.MemberType.Name(),
                        (context.ObjectType == null ? "" : " when injecting into '{0}'".Fmt(context.ObjectType.Name())),
                        context.GetObjectGraphString()));
            }
            else
            {
                Assert.That(result == DiContainer.ProviderLookupResult.None);

                if (ReflectionUtil.IsGenericList(context.MemberType))
                {
                    var subType = context.MemberType.GetGenericArguments().Single();
                    var subContext = context.ChangeMemberType(subType);

                    var matches = GetAllProviderMatches(subContext);

                    if (matches.IsEmpty())
                    {
                        if (!context.Optional)
                        {
                            yield return new ZenjectResolveException(
                                "Could not find dependency with type 'List(0)'{1}.  If the empty list is also valid, you can allow this by using the [InjectOptional] attribute.' \nObject graph:\n{2}"
                                .Fmt(
                                    subContext.MemberType.Name(),
                                    (context.ObjectType == null ? "" : " when injecting into '{0}'".Fmt(context.ObjectType.Name())),
                                    context.GetObjectGraphString()));
                        }
                    }
                    else
                    {
                        foreach (var match in matches)
                        {
                            foreach (var error in match.ValidateBinding(context))
                            {
                                yield return error;
                            }
                        }
                    }
                }
                else
                {
                    if (!context.Optional)
                    {
                        yield return new ZenjectResolveException(
                            "Could not find required dependency with type '{0}'{1} \nObject graph:\n{2}"
                            .Fmt(
                                context.MemberType.Name(),
                                (context.ObjectType == null ? "" : " when injecting into '{0}'".Fmt(context.ObjectType.Name())),
                                context.GetObjectGraphString()));
                    }
                }
            }
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateObjectGraph<TConcrete>(
            params Type[] extras)
        {
            return ValidateObjectGraph(typeof(TConcrete), extras);
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateObjectGraph<TConcrete>(InjectContext context, params Type[] extras)
        {
            return ValidateObjectGraph(typeof(TConcrete), context, extras);
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateObjectGraph(
            Type contractType, params Type[] extras)
        {
            return ValidateObjectGraph(
                contractType, new InjectContext(this, contractType), extras);
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateObjectGraph(
            Type contractType, InjectContext context, params Type[] extras)
        {
            return ValidateObjectGraph(
                contractType, context, null, extras);
        }

        // See comment in IResolver.cs for description of this method
        public IEnumerable<ZenjectResolveException> ValidateObjectGraph(
            Type concreteType, InjectContext currentContext, string concreteIdentifier, params Type[] extras)
        {
            CheckForInstallWarning(currentContext);

            if (concreteType.IsAbstract)
            {
                throw new ZenjectResolveException(
                    "Expected contract type '{0}' to be non-abstract".Fmt(concreteType.Name()));
            }

            var typeInfo = TypeAnalyzer.GetInfo(concreteType);
            var extrasList = extras.ToList();

            foreach (var dependInfo in typeInfo.AllInjectables)
            {
                Assert.IsEqual(dependInfo.ObjectType, concreteType);

                if (TryTakingFromExtras(dependInfo.MemberType, extrasList))
                {
                    continue;
                }

                var context = dependInfo.CreateInjectContext(this, currentContext, null, concreteIdentifier);

                foreach (var error in ValidateResolve(context))
                {
                    yield return error;
                }
            }

            if (!extrasList.IsEmpty())
            {
                yield return new ZenjectResolveException(
                    "Found unnecessary extra parameters passed when injecting into '{0}' with types '{1}'.  \nObject graph:\n{2}"
                    .Fmt(concreteType.Name(), String.Join(",", extrasList.Select(x => x.Name()).ToArray()), currentContext.GetObjectGraphString()));
            }
        }

        bool TryTakingFromExtras(Type contractType, List<Type> extrasList)
        {
            foreach (var extraType in extrasList)
            {
                if (extraType.DerivesFromOrEqual(contractType))
                {
                    var removed = extrasList.Remove(extraType);
                    Assert.That(removed);
                    return true;
                }
            }

            return false;
        }

        ////////////// Types ////////////////

        class ProviderPair
        {
            public readonly ProviderBase Provider;
            public readonly DiContainer Container;

            public ProviderPair(
                ProviderBase provider,
                DiContainer container)
            {
                Provider = provider;
                Container = container;
            }
        }

        public enum ProviderLookupResult
        {
            Success,
            Multiple,
            None
        }

        struct LookupId
        {
            public readonly ProviderBase Provider;
            public readonly BindingId BindingId;

            public LookupId(
                ProviderBase provider, BindingId bindingId)
            {
                Provider = provider;
                BindingId = bindingId;
            }
        }
    }
}
