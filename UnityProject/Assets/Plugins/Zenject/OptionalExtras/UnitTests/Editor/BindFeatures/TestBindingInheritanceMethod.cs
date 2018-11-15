using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestBindingInheritanceMethod : ZenjectUnitTestFixture
    {
        [Test]
        public void TestNoCopy()
        {
            Container.Bind<Foo>().AsSingle();

            var sub1 = Container.CreateSubContainer();

            Assert.IsEqual(sub1.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestCopyIntoAll1()
        {
            Container.Bind<Foo>().AsSingle().CopyIntoAllSubContainers();

            var sub1 = Container.CreateSubContainer();

            Assert.IsNotEqual(sub1.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestCopyIntoAll2()
        {
            Container.Bind<IBar>().To<Bar>().FromResolve().CopyIntoAllSubContainers();
            Container.Bind<Bar>().AsSingle();

            var sub1 = Container.CreateSubContainer();

            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 1);
            Assert.IsEqual(sub1.ResolveAll<IBar>().Count, 2);
        }

        [Test]
        public void TestCopyDirect1()
        {
            Container.Bind<Foo>().AsSingle().CopyIntoDirectSubContainers();

            var sub1 = Container.CreateSubContainer();
            var sub2 = sub1.CreateSubContainer();

            Assert.That(Container.HasBindingId(typeof(Foo), null, InjectSources.Local));
            Assert.That(sub1.HasBindingId(typeof(Foo), null, InjectSources.Local));
            Assert.That(!sub2.HasBindingId(typeof(Foo), null, InjectSources.Local));
        }

        [Test]
        public void TestMoveDirect1()
        {
            Container.Bind<Foo>().AsSingle().MoveIntoDirectSubContainers();

            var sub1 = Container.CreateSubContainer();
            var sub2 = sub1.CreateSubContainer();

            Assert.That(!Container.HasBindingId(typeof(Foo), null, InjectSources.Local));
            Assert.That(sub1.HasBindingId(typeof(Foo), null, InjectSources.Local));
            Assert.That(!sub2.HasBindingId(typeof(Foo), null, InjectSources.Local));
        }

        [Test]
        public void TestMoveAll()
        {
            Container.Bind<Foo>().AsSingle().MoveIntoAllSubContainers();

            var sub1 = Container.CreateSubContainer();
            var sub2 = sub1.CreateSubContainer();

            Assert.That(!Container.HasBindingId(typeof(Foo), null, InjectSources.Local));
            Assert.That(sub1.HasBindingId(typeof(Foo), null, InjectSources.Local));
            Assert.That(sub2.HasBindingId(typeof(Foo), null, InjectSources.Local));
        }

        public interface IBar
        {
        }

        public class Foo
        {
        }

        public class Bar : IBar
        {
        }
    }
}
