using System;
using System.Collections.Generic;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public abstract class FacadeFactory<TFacade> : IFactory<TFacade>, IValidatable
        where TFacade : IFacade
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<DiContainer> _containerInitializer = null;

        public TFacade Create()
        {
            var facade = CreateSubContainer(_container, _containerInitializer).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        // Made static for use by FacadeBinder
        public static DiContainer CreateSubContainer(
            DiContainer parentContainer, Action<DiContainer> containerInitializer)
        {
            Assert.IsNotNull(containerInitializer);
            var subContainer = parentContainer.CreateSubContainer();
            subContainer.IsValidating = parentContainer.IsValidating;
            containerInitializer(subContainer);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        // Made static for use by FacadeBinder
        public static IEnumerable<ZenjectResolveException> Validate(
            DiContainer parentContainer, Action<DiContainer> containerInitializer)
        {
            var subContainer = CreateSubContainer(parentContainer, containerInitializer);

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            return Validate(_container, _containerInitializer);
        }
    }

    public abstract class FacadeFactory<TParam1, TFacade> : IValidatable
        where TFacade : IFacade
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<DiContainer, TParam1> _containerInitializer = null;

        public TFacade Create(TParam1 param1)
        {
            var facade = CreateSubContainer(param1).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.IsValidating = _container.IsValidating;
            _containerInitializer(subContainer, param1);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1));

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TFacade> : IValidatable
        where TFacade : IFacade
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<DiContainer, TParam1, TParam2> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2)
        {
            var facade = CreateSubContainer(param1, param2).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.IsValidating = _container.IsValidating;
            _containerInitializer(subContainer, param1, param2);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2));

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TFacade> : IValidatable
        where TFacade : IFacade
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<DiContainer, TParam1, TParam2, TParam3> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            var facade = CreateSubContainer(param1, param2, param3).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.IsValidating = _container.IsValidating;
            _containerInitializer(subContainer, param1, param2, param3);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3));

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TFacade> : IValidatable
        where TFacade : IFacade
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            var facade = CreateSubContainer(param1, param2, param3, param4).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.IsValidating = _container.IsValidating;
            _containerInitializer(subContainer, param1, param2, param3, param4);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3), default(TParam4));

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade> : IValidatable
        where TFacade : IFacade
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            var facade = CreateSubContainer(param1, param2, param3, param4, param5).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.IsValidating = _container.IsValidating;
            _containerInitializer(subContainer, param1, param2, param3, param4, param5);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3), default(TParam4), default(TParam5));

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade> : IValidatable
        where TFacade : IFacade
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
        {
            var facade = CreateSubContainer(param1, param2, param3, param4, param5, param6).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.IsValidating = _container.IsValidating;
            _containerInitializer(subContainer, param1, param2, param3, param4, param5, param6);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3), default(TParam4), default(TParam5), default(TParam6));

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TFacade> : IValidatable
        where TFacade : IFacade
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
        {
            var facade = CreateSubContainer(param1, param2, param3, param4, param5, param6, param7).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.IsValidating = _container.IsValidating;
            _containerInitializer(subContainer, param1, param2, param3, param4, param5, param6, param7);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3), default(TParam4), default(TParam5), default(TParam6), default(TParam7));

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }
}
