using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestDecoratorValidation
    {
        public interface ISaveHandler
        {
            void Save();
        }

        public class SaveHandler : ISaveHandler
        {
            public void Save()
            {
            }
        }

        public class SaveDecorator1 : ISaveHandler
        {
            readonly ISaveHandler _handler;

            public SaveDecorator1(ISaveHandler handler)
            {
                _handler = handler;
            }

            public void Save()
            {
                _handler.Save();
            }
        }

        DiContainer Container
        {
            get; set;
        }

        [SetUp]
        public void Setup()
        {
            Container = new DiContainer(true);
            Container.Settings = new ZenjectSettings(ValidationErrorResponses.Throw);
        }

        public class Foo
        {
            public Foo(ISaveHandler saveHandler)
            {
            }
        }

        [Test]
        public void TestSuccess1()
        {
            Container.Bind<ISaveHandler>().To<SaveHandler>().AsSingle();
            Container.Decorate<ISaveHandler>().With<SaveDecorator1>();
            Container.Bind<Foo>().AsSingle().NonLazy();

            Container.ResolveRoots();
        }

        [Test]
        public void TestFail1()
        {
            Container.Bind<ISaveHandler>().To<SaveHandler>().AsSingle();
            Container.Decorate<ISaveHandler>().With<SaveDecorator1>().FromResolve(Guid.NewGuid());
            Container.Bind<Foo>().AsSingle().NonLazy();

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestFail2()
        {
            Container.Bind<ISaveHandler>().To<SaveHandler>().FromResolve(Guid.NewGuid()).AsSingle();
            Container.Decorate<ISaveHandler>().With<SaveDecorator1>();
            Container.Bind<Foo>().AsSingle().NonLazy();

            Assert.Throws(() => Container.ResolveRoots());
        }
    }
}

