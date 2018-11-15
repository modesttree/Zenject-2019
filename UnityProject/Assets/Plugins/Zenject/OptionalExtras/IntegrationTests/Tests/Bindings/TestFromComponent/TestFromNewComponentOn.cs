#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Zenject.Tests.Bindings
{
    public class TestFromNewComponentOn : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator TestBasic()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance(gameObject).WithId("Foo");

            Container.Bind<Foo>().FromNewComponentOn(gameObject).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestBasicByMethod()
        {
            PreInstall();

            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance(gameObject).WithId("Foo");

            Container.Bind<Foo>().FromNewComponentOn(context => gameObject).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestTransient()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance(gameObject).WithId("Foo");

            Container.Bind<Foo>().FromNewComponentOn(gameObject).AsTransient().NonLazy();
            Container.Bind<IFoo>().To<Foo>().FromNewComponentOn(gameObject).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance(gameObject).WithId("Foo");

            Container.Bind(typeof(IFoo), typeof(Foo)).To<Foo>().FromNewComponentOn(gameObject).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCached1()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance(gameObject).WithId("Foo");

            Container.Bind<Foo>().FromNewComponentOn(gameObject).AsCached().NonLazy();
            Container.Bind<IFoo>().To<Foo>().FromNewComponentOn(gameObject).AsCached().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCached2()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance(gameObject).WithId("Foo");

            Container.Bind(typeof(IFoo), typeof(Foo)).To<Foo>()
                .FromNewComponentOn(gameObject).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCachedMultipleConcrete()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance(gameObject).WithId("Foo");

            Container.Bind(typeof(IFoo), typeof(IBar))
                .To(new List<Type> { typeof(Foo), typeof(Bar) })
                .FromNewComponentOn(gameObject).AsCached().NonLazy();

            PostInstall();

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
