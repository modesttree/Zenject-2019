#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;

namespace Zenject.Tests.Bindings.InstantiateCallback
{
    public class TestInstantiateCallback : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get { return GetPrefab("Foo"); }
        }

        GameObject EmptyPrefab
        {
            get { return GetPrefab("Empty"); }
        }

        GameObject GetPrefab(string name)
        {
            return FixtureUtil.GetPrefab(GetPrefabPath(name));
        }

        string GetPrefabPath(string name)
        {
            return "TestInstantiateCallback/{0}".Fmt(name);
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewGameObject()
        {
            PreInstall();

            Container.Bind<Foo>().FromNewComponentOnNewGameObject()
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOn()
        {
            PreInstall();

            var gameObject = new GameObject();

            Container.Bind<Foo>().FromNewComponentOn(gameObject)
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOn2()
        {
            PreInstall();

            var gameObject = new GameObject();

            Container.Bind<Foo>().FromNewComponentOn(ctx => gameObject)
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewPrefab()
        {
            PreInstall();

            Container.Bind<Foo>().FromNewComponentOnNewPrefab(EmptyPrefab)
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewPrefabResource()
        {
            PreInstall();

            Container.Bind<Foo>().FromNewComponentOnNewPrefabResource(GetPrefabPath("Empty"))
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnRoot()
        {
            PreInstall();

            Container.Bind<Foo>().FromNewComponentOnRoot()
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
            yield break;
        }

        public class Bar : MonoBehaviour
        {
            [Inject]
            public Foo Foo;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentSibling()
        {
            PreInstall();

            var bar = new GameObject().AddComponent<Bar>();

            Container.QueueForInject(bar);
            Container.Bind<Foo>().FromNewComponentSibling()
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            Assert.IsEqual(bar.Foo.Value, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromComponentInNewPrefab()
        {
            PreInstall();

            Container.Bind<Foo>().FromComponentInNewPrefab(FooPrefab)
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromComponentInNewPrefabResource()
        {
            PreInstall();

            Container.Bind<Foo>().FromComponentInNewPrefabResource(GetPrefabPath("Foo"))
                .AsSingle().OnInstantiated<Foo>((ctx, obj) =>
                    {
                        Assert.That(obj.WasInjected);
                        obj.Value = "asdf";
                    });

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
            yield break;
        }

    }
}

#endif
