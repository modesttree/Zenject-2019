using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestValidationSettings
    {
        DiContainer Container
        {
            get; set;
        }

        [SetUp]
        public void Setup()
        {
            Container = new DiContainer(true);
        }

        // Doesn't work because the logged error is flagged as a test error
        //[Test]
        //public void TestValidationErrorLogOnly()
        //{
            //Container.Settings = new ZenjectSettings(ValidationErrorResponses.Log);
            //Container.Bind<Bar>().AsSingle().NonLazy();

            //Container.ResolveRoots();
        //}

        [Test]
        public void TestValidationErrorThrows()
        {
            Container.Settings = new ZenjectSettings(ValidationErrorResponses.Throw);

            Container.Bind<Bar>().AsSingle().NonLazy();

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestOutsideObjectGraph1()
        {
            Container.Settings = new ZenjectSettings(ValidationErrorResponses.Throw);

            Container.Bind<Bar>().AsSingle();

            Container.ResolveRoots();
        }

        [Test]
        public void TestOutsideObjectGraph2()
        {
            Container.Settings = new ZenjectSettings(
                ValidationErrorResponses.Throw, RootResolveMethods.All);

            Container.Bind<Bar>().AsSingle();

            Assert.Throws(() => Container.ResolveRoots());
        }

        public class Bar
        {
            public Bar(Foo foo)
            {
            }
        }

        public class Foo
        {
        }
    }
}


