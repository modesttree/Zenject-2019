#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;

namespace Zenject.Tests.Bindings
{
    public class TestFromComponentInHierarchy : ZenjectIntegrationTestFixture
    {
        Foo _foo1;
        Foo _foo2;

        public void Setup1()
        {
            var root = new GameObject();

            _foo1 = root.AddComponent<Foo>();

            var child1 = new GameObject();
            child1.transform.SetParent(root.transform);

            var child2 = new GameObject();
            child2.transform.SetParent(root.transform);

            _foo2 = child2.AddComponent<Foo>();
        }

        public void Setup2()
        {
            var root = new GameObject();

            var child1 = new GameObject();
            child1.transform.SetParent(root.transform);
        }

        [UnityTest]
        public IEnumerator RunMatchSingle()
        {
            Setup1();
            PreInstall();
            Container.Bind<Qux>().AsSingle();
            Container.Bind<Foo>().FromComponentInHierarchy().AsSingle();

            PostInstall();

            var qux = Container.Resolve<Qux>();
            Assert.IsEqual(qux.Foos.Count, 1);
            Assert.IsEqual(qux.Foos[0], _foo1);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchMultiple()
        {
            Setup1();
            PreInstall();
            Container.Bind<Qux>().AsSingle();
            Container.Bind<Foo>().FromComponentsInHierarchy().AsCached();

            PostInstall();

            var qux = Container.Resolve<Qux>();
            Assert.IsEqual(qux.Foos.Count, 2);
            Assert.IsEqual(qux.Foos[0], _foo1);
            Assert.IsEqual(qux.Foos[1], _foo2);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchNotFoundFailure()
        {
            Setup2();
            PreInstall();
            Container.Bind<Bar>().AsSingle().NonLazy();
            Container.Bind<Foo>().FromComponentInHierarchy().AsSingle();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchNotFoundSuccess()
        {
            Setup2();
            PreInstall();

            Container.Bind<Qux>().AsSingle().NonLazy();
            Container.Bind<Foo>().FromComponentsInHierarchy().AsCached();

            PostInstall();

            var qux = Container.Resolve<Qux>();
            Assert.IsEqual(qux.Foos.Count, 0);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptional()
        {
            PreInstall();

            Container.Bind<Qiv>().AsSingle().NonLazy();
            Container.Bind<Foo>().FromComponentInHierarchy().AsSingle();

            PostInstall();

            var qiv = Container.Resolve<Qiv>();
            Assert.IsNull(qiv.Foo);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchSingleNonGeneric()
        {
            Setup1();
            PreInstall();
            Container.Bind<Qux>().AsSingle();
            Container.Bind(typeof(Foo)).FromComponentInHierarchy().AsSingle();

            PostInstall();

            var qux = Container.Resolve<Qux>();
            Assert.IsEqual(qux.Foos.Count, 1);
            Assert.IsEqual(qux.Foos[0], _foo1);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchMultipleNonGeneric()
        {
            Setup1();
            PreInstall();
            Container.Bind<Qux>().AsSingle();
            Container.Bind(typeof(Foo)).FromComponentsInHierarchy().AsCached();

            PostInstall();

            var qux = Container.Resolve<Qux>();
            Assert.IsEqual(qux.Foos.Count, 2);
            Assert.IsEqual(qux.Foos[0], _foo1);
            Assert.IsEqual(qux.Foos[1], _foo2);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchNotFoundFailureNonGeneric()
        {
            Setup2();
            PreInstall();
            Container.Bind<Bar>().AsSingle().NonLazy();
            Container.Bind(typeof(Foo)).FromComponentInHierarchy().AsSingle();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchNotFoundSuccessNonGeneric()
        {
            Setup2();
            PreInstall();

            Container.Bind<Qux>().AsSingle().NonLazy();
            Container.Bind(typeof(Foo)).FromComponentsInHierarchy().AsCached();

            PostInstall();

            var qux = Container.Resolve<Qux>();
            Assert.IsEqual(qux.Foos.Count, 0);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptionalNonGeneric()
        {
            PreInstall();

            Container.Bind<Qiv>().AsSingle().NonLazy();
            Container.Bind(typeof(Foo)).FromComponentInHierarchy().AsSingle();

            PostInstall();

            var qiv = Container.Resolve<Qiv>();
            Assert.IsNull(qiv.Foo);
            yield break;
        }

        public class Foo : MonoBehaviour
        {
        }

        public class Qux
        {
            [Inject]
            public List<Foo> Foos;
        }

        public class Bar
        {
            [Inject]
            public Foo Foo;
        }

        public class Qiv
        {
            [InjectOptional]
            public Foo Foo;
        }
    }
}

#endif
