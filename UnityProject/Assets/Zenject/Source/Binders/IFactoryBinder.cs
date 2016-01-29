using System;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    ////////////////////////////// Zero parameters //////////////////////////////
    [System.Diagnostics.DebuggerStepThrough]
    public class IFactoryBinder<TContract>
    {
        readonly DiContainer _container;
        readonly string _identifier;

        public IFactoryBinder(DiContainer container, string identifier)
        {
            _container = container;
            _identifier = identifier;
        }

        public BindingConditionSetter ToInstance(TContract instance)
        {
            return ToMethod((c) => instance);
        }

        public BindingConditionSetter ToMethod(
            Func<DiContainer, TContract> method)
        {
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<FactoryMethod<TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract,
                "Unable to create abstract type '{0}' in Factory", typeof(TContract).Name());
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToTransient<Factory<TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            return ToCustomFactory<TConcrete, Factory<TConcrete>>();
        }

        // Note that we assume here that IFactory<TConcrete> is bound somewhere else
        public BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>(c.Container.Resolve<IFactory<TConcrete>>()));
        }

		public BindingConditionSetter ToFacadeFactory<TConcrete, TFactory>(Action<DiContainer> facadeInstaller)
			where TFactory : IFactory<TConcrete>
			where TConcrete : TContract
		{
			return _container.Bind<IFactory<TContract>>(_identifier)
				.ToMethod(c => new FactoryNested<TContract, TConcrete>(c.Container.Instantiate<TFactory>(facadeInstaller)));
		}

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TContract>
        {
            return _container.Bind<IFactory<TContract>>(_identifier).ToTransient<TFactory>();
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>(params object[] args)
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>(c.Container.Instantiate<TFactory>(args)));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToPrefab(GameObject prefab)
        {
            Assert.That(typeof(TContract).DerivesFrom<Component>());

            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindIFactory<{0}>().ToPrefab".Fmt(typeof(TContract).Name()));
            }

            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<GameObjectFactory<TContract>>(prefab));
        }
#endif
    }

    ////////////////////////////// One parameter //////////////////////////////
    [System.Diagnostics.DebuggerStepThrough]
    public class IFactoryBinder<TParam1, TContract>
    {
        readonly DiContainer _container;
        readonly string _identifier;

        public IFactoryBinder(DiContainer container, string identifier)
        {
            _container = container;
            _identifier = identifier;
        }

        public BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TContract> method)
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<FactoryMethod<TParam1, TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract);
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToTransient<Factory<TParam1, TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            return ToCustomFactory<TConcrete, Factory<TParam1, TConcrete>>();
        }

        // Note that we assume here that IFactory<TConcrete> is bound somewhere else
        public BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TContract, TConcrete>(
                        c.Container.Resolve<IFactory<TParam1, TConcrete>>()));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TContract>
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier).ToTransient<TFactory>();
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TConcrete>
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TContract, TConcrete>(
                        c.Container.Instantiate<TFactory>()));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToPrefab(GameObject prefab)
        {
            Assert.That(typeof(TContract).DerivesFrom<Component>());

            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindIFactory<{0}>().ToPrefab".Fmt(typeof(TContract).Name()));
            }

            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<GameObjectFactory<TParam1, TContract>>(prefab));
        }
#endif
    }

    ////////////////////////////// Two parameters //////////////////////////////
    [System.Diagnostics.DebuggerStepThrough]
    public class IFactoryBinder<TParam1, TParam2, TContract>
    {
        readonly DiContainer _container;
        readonly string _identifier;

        public IFactoryBinder(DiContainer container, string identifier)
        {
            _container = container;
            _identifier = identifier;
        }

        public BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TParam2, TContract> method)
        {
            return _container.Bind<IFactory<TParam1, TParam2, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<FactoryMethod<TParam1, TParam2, TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract);
            return _container.Bind<IFactory<TParam1, TParam2, TContract>>(_identifier)
                .ToTransient<Factory<TParam1, TParam2, TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            return ToCustomFactory<TConcrete, Factory<TParam1, TParam2, TConcrete>>();
        }

        // Note that we assume here that IFactory<TConcrete> is bound somewhere else
        public BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TParam2, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TParam2, TContract, TConcrete>(
                        c.Container.Resolve<IFactory<TParam1, TParam2, TConcrete>>()));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TContract>
        {
            return _container.Bind<IFactory<TParam1, TParam2, TContract>>(_identifier).ToTransient<TFactory>();
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TConcrete>
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TParam2, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TParam2, TContract, TConcrete>(
                        c.Container.Instantiate<TFactory>()));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToPrefab(GameObject prefab)
        {
            Assert.That(typeof(TContract).DerivesFrom<Component>());

            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindIFactory<{0}>().ToPrefab".Fmt(typeof(TContract).Name()));
            }

            return _container.Bind<IFactory<TParam1, TParam2, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<GameObjectFactory<TParam1, TParam2, TContract>>(prefab));
        }
#endif
    }

    ////////////////////////////// Three parameters //////////////////////////////
    [System.Diagnostics.DebuggerStepThrough]
    public class IFactoryBinder<TParam1, TParam2, TParam3, TContract>
    {
        readonly DiContainer _container;
        readonly string _identifier;

        public IFactoryBinder(DiContainer container, string identifier)
        {
            _container = container;
            _identifier = identifier;
        }

        public BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TParam2, TParam3, TContract> method)
        {
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<FactoryMethod<TParam1, TParam2, TParam3, TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract);
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TContract>>(_identifier)
                .ToTransient<Factory<TParam1, TParam2, TParam3, TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            return ToCustomFactory<TConcrete, Factory<TParam1, TParam2, TParam3, TConcrete>>();
        }

        // Note that we assume here that IFactory<TConcrete> is bound somewhere else
        public BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TParam2, TParam3, TContract, TConcrete>(
                        c.Container.Resolve<IFactory<TParam1, TParam2, TParam3, TConcrete>>()));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>
        {
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TContract>>(_identifier).ToTransient<TFactory>();
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TParam2, TParam3, TContract, TConcrete>(
                        c.Container.Instantiate<TFactory>()));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToPrefab(GameObject prefab)
        {
            Assert.That(typeof(TContract).DerivesFrom<Component>());

            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindIFactory<{0}>().ToPrefab".Fmt(typeof(TContract).Name()));
            }

            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<GameObjectFactory<TParam1, TParam2, TParam3, TContract>>(prefab));
        }
#endif
    }


    ////////////////////////////// Four parameters //////////////////////////////
    [System.Diagnostics.DebuggerStepThrough]
    public class IFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract>
    {
        readonly DiContainer _container;
        readonly string _identifier;

        public IFactoryBinder(DiContainer container, string identifier)
        {
            _container = container;
            _identifier = identifier;
        }

        public BindingConditionSetter ToMethod(
            ModestTree.Util.Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TContract> method)
        {
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TParam4, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<FactoryMethod<TParam1, TParam2, TParam3, TParam4, TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract);
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TParam4, TContract>>(_identifier)
                .ToTransient<Factory<TParam1, TParam2, TParam3, TParam4, TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            return ToCustomFactory<TConcrete, Factory<TParam1, TParam2, TParam3, TParam4, TConcrete>>();
        }

        // Note that we assume here that IFactory<TConcrete> is bound somewhere else
        public BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TParam4, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TParam2, TParam3, TParam4, TContract, TConcrete>(
                        c.Container.Resolve<IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>>()));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>
        {
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TParam4, TContract>>(_identifier).ToTransient<TFactory>();
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TParam4, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TParam2, TParam3, TParam4, TContract, TConcrete>(
                        c.Container.Instantiate<TFactory>()));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToPrefab(GameObject prefab)
        {
            Assert.That(typeof(TContract).DerivesFrom<Component>());

            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindIFactory<{0}>().ToPrefab".Fmt(typeof(TContract).Name()));
            }

            return _container.Bind<IFactory<TParam1, TParam2, TParam3, TParam4, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Container.Instantiate<GameObjectFactory<TParam1, TParam2, TParam3, TParam4, TContract>>(prefab));
        }
#endif
    }
}

