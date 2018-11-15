#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine.TestTools;
using Zenject.Tests.Bindings.FromNewScriptableObjectResource;

namespace Zenject.Tests.Bindings
{
    public class TestFromNewScriptableObjectResource : ZenjectIntegrationTestFixture
    {
        const string PathPrefix = "TestFromNewScriptableObjectResource/";

        [UnityTest]
        public IEnumerator TestTransientError()
        {
            PreInstall();
            // Validation should detect that it doesn't exist
            Container.Bind<Foo>().FromNewScriptableObjectResource(PathPrefix + "asdfasdfas").AsTransient().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestTransient()
        {
            PreInstall();
            Foo.InstanceCount = 0;
            Container.Bind<Foo>().FromNewScriptableObjectResource(PathPrefix + "Foo").AsTransient();

            PostInstall();

            var foo = Container.Resolve<Foo>();
            Assert.That(foo.WasInjected);

            Assert.IsEqual(Foo.InstanceCount, 1);

            var foo2 = Container.Resolve<Foo>();
            Assert.IsNotEqual(foo, foo2);
            Assert.IsEqual(Foo.InstanceCount, 2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle()
        {
            PreInstall();
            Foo.InstanceCount = 0;

            Container.Bind(typeof(IFoo), typeof(Foo)).To<Foo>().FromNewScriptableObjectResource(PathPrefix + "Foo").AsSingle();

            PostInstall();

            Container.Resolve<IFoo>();
            Assert.IsEqual(Foo.InstanceCount, 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestAbstractBinding()
        {
            PreInstall();
            Foo.InstanceCount = 0;

            Container.Bind<IFoo>().To<Foo>()
                .FromNewScriptableObjectResource(PathPrefix + "Foo").AsSingle().NonLazy();

            PostInstall();

            Container.Resolve<IFoo>();
            Assert.IsEqual(Foo.InstanceCount, 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArgumentsFail()
        {
            PreInstall();
            Container.Bind<Bob>()
                .FromNewScriptableObjectResource(PathPrefix + "Bob").AsSingle().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArguments()
        {
            PreInstall();
            Container.Bind<Bob>()
                .FromNewScriptableObjectResource(PathPrefix + "Bob").AsSingle()
                .WithArguments("test1").NonLazy();

            PostInstall();

            Assert.IsEqual(Container.Resolve<Bob>().Arg, "test1");
            yield break;
        }
    }
}

#endif
