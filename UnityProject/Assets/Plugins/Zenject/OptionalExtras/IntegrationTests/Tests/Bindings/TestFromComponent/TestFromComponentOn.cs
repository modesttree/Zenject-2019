#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;

namespace Zenject.Tests.Bindings
{
    public class TestFromComponentOn : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator TestBasic()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");
            gameObject.AddComponent<Foo>();

            Container.Bind<Foo>().FromComponentOn(gameObject).AsSingle().NonLazy();

            PostInstall();

            Assert.IsNotNull(Container.Resolve<Foo>());
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestBasicMultiple()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");
            gameObject.AddComponent<Foo>();
            gameObject.AddComponent<Foo>();

            Container.Bind<Foo>().FromComponentsOn(gameObject).AsCached().NonLazy();

            PostInstall();

            Assert.IsEqual(Container.ResolveAll<Foo>().Count, 2);
            FixtureUtil.AssertComponentCount<Foo>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestBasicByMethod()
        {
            PreInstall();

            var gameObject = Container.CreateEmptyGameObject("Foo");
            gameObject.AddComponent<Foo>();

            Container.Bind<Foo>().FromComponentOn(context => gameObject).AsSingle().NonLazy();

            PostInstall();

            Assert.IsNotNull(Container.Resolve<Foo>());
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestBasicByMethodMultiple()
        {
            PreInstall();

            var gameObject = Container.CreateEmptyGameObject("Foo");
            gameObject.AddComponent<Foo>();
            gameObject.AddComponent<Foo>();

            Container.Bind<Foo>().FromComponentsOn(context => gameObject).AsCached().NonLazy();

            PostInstall();

            Assert.IsEqual(Container.ResolveAll<Foo>().Count, 2);
            FixtureUtil.AssertComponentCount<Foo>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");
            gameObject.AddComponent<Foo>();

            Container.Bind(typeof(IFoo), typeof(Foo)).To<Foo>().FromComponentOn(gameObject).AsSingle().NonLazy();

            PostInstall();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCachedMultipleConcrete()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");
            gameObject.AddComponent<Foo>();
            gameObject.AddComponent<Bar>();

            Container.Bind(typeof(IFoo), typeof(IBar))
                .To(new List<Type> { typeof(Foo), typeof(Bar) })
                .FromComponentOn(gameObject).AsCached().NonLazy();

            PostInstall();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 2);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            yield break;
        }

        public interface IBar
        {
        }

        public interface IFoo2
        {
        }

        public interface IFoo
        {
        }

        public class Foo : MonoBehaviour, IFoo, IBar, IFoo2
        {
        }

        public class Bar : MonoBehaviour, IFoo, IBar, IFoo2
        {
        }
    }
}

#endif

