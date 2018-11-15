using System;
using System.Collections.Generic;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFrom : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSelfSingle()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSelfSingleExplicit()
        {
            Container.Bind<Foo>().ToSelf().FromNew().AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSelfTransient()
        {
            Container.Bind<Foo>().AsTransient().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSelfCached()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestConcreteSingle()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestConcreteTransient()
        {
            Container.Bind<IFoo>().To<Foo>().AsTransient().NonLazy();

            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestConcreteTransient2()
        {
            Container.Bind<IFoo>().To<Foo>().AsTransient().NonLazy();

            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestConcreteCached()
        {
            Container.Bind<Foo>().AsCached().NonLazy();
            Container.Bind<IFoo>().To<Foo>().AsCached().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestDuplicateBindingsFail()
        {
            Container.Bind<Foo>().AsSingle();
            Container.Bind<Foo>().AsSingle();

            Assert.Throws(
                () => Container.FlushBindings());
        }

        [Test]
        public void TestConcreteMultipleTransient()
        {
            Container.Bind<IFoo>().To(typeof(Foo), typeof(Bar)).AsTransient().NonLazy();

            var foos = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos.Count, 2);
            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            var foos2 = Container.ResolveAll<IFoo>();

            Assert.IsNotEqual(foos[0], foos2[0]);
            Assert.IsNotEqual(foos[1], foos2[1]);
        }

        [Test]
        public void TestConcreteMultipleSingle()
        {
            Container.Bind<IFoo>().To(typeof(Foo), typeof(Bar)).AsSingle().NonLazy();

            var foos = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos.Count, 2);
            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            var foos2 = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos[0], foos2[0]);
            Assert.IsEqual(foos[1], foos2[1]);
        }

        [Test]
        public void TestMultipleBindingsSingleFail1()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).AsSingle();

            Assert.Throws(() => Container.FlushBindings());
        }

        [Test]
        public void TestMultipleBindingsSingleFail2()
        {
            Assert.Throws(() => Container.Bind(typeof(IFoo), typeof(IBar)).To<Qux>().AsSingle());
        }

        [Test]
        public void TestMultipleBindingsSingle()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
            Assert.That(Container.Resolve<IFoo>() is Foo);
        }

        [Test]
        public void TestMultipleBindingsTransient()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsTransient().NonLazy();

            Assert.That(Container.Resolve<IFoo>() is Foo);
            Assert.That(Container.Resolve<IBar>() is Foo);

            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestMultipleBindingsCached()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleSingle()
        {
            Container.Bind(typeof(IFoo), typeof(IBar))
                .To(new List<Type> {typeof(Foo), typeof(Bar)}).AsSingle().NonLazy();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsEqual(foos[0], bars[0]);
            Assert.IsEqual(foos[1], bars[1]);
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleTransient()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type> {typeof(Foo), typeof(Bar)}).AsTransient().NonLazy();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsNotEqual(foos[0], bars[0]);
            Assert.IsNotEqual(foos[1], bars[1]);
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleCached()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type> {typeof(Foo), typeof(Bar)}).AsCached().NonLazy();
            Container.Bind<Foo>().AsCached().NonLazy();
            Container.Bind<Bar>().AsCached().NonLazy();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsEqual(foos[0], bars[0]);
            Assert.IsEqual(foos[1], bars[1]);

            Assert.IsNotEqual(foos[0], Container.Resolve<Foo>());
            Assert.IsNotEqual(foos[1], Container.Resolve<Bar>());
        }

        interface IBar
        {
        }

        interface IFoo
        {
        }

        class Foo : IFoo, IBar
        {
        }

        class Bar : IFoo, IBar
        {
        }

        public class Qux
        {
        }
    }
}
