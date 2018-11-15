#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject.Tests.Factories.BindFactory;

namespace Zenject.Tests.Factories
{
    public class TestBindFactory : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get { return FixtureUtil.GetPrefab("TestBindFactory/Foo"); }
        }

        GameObject CameraPrefab
        {
            get { return FixtureUtil.GetPrefab("TestBindFactory/Camera"); }
        }

        GameObject FooSubContainerPrefab
        {
            get { return FixtureUtil.GetPrefab("TestBindFactory/FooSubContainer"); }
        }

        [UnityTest]
        public IEnumerator TestFromNewScriptableObjectResource()
        {
            PreInstall();
            Container.BindFactory<Bar, Bar.Factory>()
                .FromNewScriptableObjectResource("TestBindFactory/Bar");

            PostInstall();

            var factory = Container.Resolve<Bar.Factory>();
            var bar = factory.Create();
            Assert.IsNotNull(bar);
            Assert.IsNotEqual(bar, factory.Create());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromComponentInHierarchy()
        {
            PreInstall();
            var foo = new GameObject().AddComponent<Foo>();

            Container.BindFactory<Foo, Foo.Factory>().FromComponentInHierarchy();

            PostInstall();

            var factory = Container.Resolve<Foo.Factory>();
            var foo2 = factory.Create();
            Assert.IsNotNull(foo2);
            Assert.IsEqual(foo, foo2);
            Assert.IsEqual(foo, factory.Create());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromComponentInHierarchyErrors()
        {
            PreInstall();
            Container.BindFactory<Foo, Foo.Factory>().FromComponentInHierarchy();

            PostInstall();

            var factory = Container.Resolve<Foo.Factory>();

            // zero matches
            Assert.Throws(() => factory.Create());

            new GameObject().AddComponent<Foo>();

            factory.Create();

            new GameObject().AddComponent<Foo>();

            // Multiple is ok too to mirror unity's GetComponentsInChildren behaviour
            factory.Create();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOn()
        {
            PreInstall();
            var go = new GameObject();

            Container.BindFactory<Foo, Foo.Factory>().FromNewComponentOn(go);

            PostInstall();

            var factory = Container.Resolve<Foo.Factory>();

            Assert.IsNull(go.GetComponent<Foo>());
            var foo = factory.Create();
            Assert.IsNotNull(go.GetComponent<Foo>());
            Assert.IsEqual(go.GetComponent<Foo>(), foo);

            var foo2 = factory.Create();

            Assert.IsNotEqual(foo2, foo);

            var allFoos = go.GetComponents<Foo>();
            Assert.IsEqual(allFoos.Length, 2);
            Assert.IsEqual(allFoos[0], foo);
            Assert.IsEqual(allFoos[1], foo2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewGameObject()
        {
            PreInstall();
            Container.BindFactory<Foo, Foo.Factory>().FromNewComponentOnNewGameObject();

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewGameObjectComponent()
        {
            PreInstall();
            Container.BindFactory<Camera, CameraFactory>().FromNewComponentOnNewGameObject();

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Camera, CameraFactory>(Container);
            FixtureUtil.AssertComponentCount<Camera>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewGameObjectComponentFailure()
        {
            PreInstall();
            Container.BindFactory<string, Camera, CameraFactory2>().FromNewComponentOnNewGameObject();

            PostInstall();

            Assert.Throws(() => Container.Resolve<CameraFactory2>().Create("asdf"));
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewGameObjectWithParamsSuccess()
        {
            PreInstall();
            Container.BindFactory<int, Foo2, Foo2.Factory2>().FromNewComponentOnNewGameObject();

            PostInstall();

            Container.Resolve<Foo2.Factory2>().Create(5);

            FixtureUtil.AssertComponentCount<Foo2>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewGameObjectWithParamsFailure()
        {
            PreInstall();
            Container.BindFactory<Foo2, Foo2.Factory>().FromNewComponentOnNewGameObject();

            PostInstall();

            Assert.Throws(() => Container.Resolve<Foo2.Factory>().Create());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnNewGameObjectConcrete()
        {
            PreInstall();
            Container.BindFactory<IFoo, IFooFactory>().To<Foo>().FromNewComponentOnNewGameObject();

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnSelf()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("foo");

            Container.BindFactory<Foo, Foo.Factory>().FromNewComponentOn(gameObject);

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnSelfFail()
        {
            PreInstall();
            Assert.Throws(() => Container.BindFactory<Foo2, Foo2.Factory>().FromNewComponentOn((GameObject)null));

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewComponentOnConcrete()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("foo");

            Container.BindFactory<IFoo, IFooFactory>().To<Foo>().FromNewComponentOn(gameObject);

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromComponentInNewPrefab()
        {
            PreInstall();
            Container.BindFactory<Foo, Foo.Factory>().FromComponentInNewPrefab(FooPrefab).WithGameObjectName("asdf");

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromComponentInPrefabComponent()
        {
            PreInstall();
            Container.BindFactory<Camera, CameraFactory>().FromComponentInNewPrefab(CameraPrefab).WithGameObjectName("asdf");

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Camera, CameraFactory>(Container);

            FixtureUtil.AssertComponentCount<Camera>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToPrefabSelfFail()
        {
            PreInstall();
            // Foo3 is not on the prefab
            Container.BindFactory<Foo3, Foo3.Factory>().FromComponentInNewPrefab(FooPrefab);

            PostInstall();

            Assert.Throws(() => FixtureUtil.CallFactoryCreateMethod<Foo3, Foo3.Factory>(Container));
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToPrefabConcrete()
        {
            PreInstall();
            Container.BindFactory<IFoo, IFooFactory>().To<Foo>().FromComponentInNewPrefab(FooPrefab).WithGameObjectName("asdf");

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToResourceSelf()
        {
            PreInstall();
            Container.BindFactory<Texture, PlaceholderFactory<Texture>>()
                .FromResource("TestBindFactory/TestTexture").NonLazy();

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Texture, PlaceholderFactory<Texture>>(Container);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToResource()
        {
            PreInstall();
            Container.BindFactory<Object, PlaceholderFactory<Object>>()
                .To<Texture>().FromResource("TestBindFactory/TestTexture").NonLazy();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToPrefabResourceSelf()
        {
            PreInstall();
            Container.BindFactory<Foo, Foo.Factory>().FromComponentInNewPrefabResource("TestBindFactory/Foo").WithGameObjectName("asdf");

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToPrefabResourceConcrete()
        {
            PreInstall();
            Container.BindFactory<Foo, Foo.Factory>().To<Foo>().FromComponentInNewPrefabResource("TestBindFactory/Foo").WithGameObjectName("asdf");

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToSubContainerPrefabSelf()
        {
            PreInstall();
            Container.BindFactory<Foo, Foo.Factory>().FromSubContainerResolve().ByNewContextPrefab(FooSubContainerPrefab);

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToSubContainerPrefabConcrete()
        {
            PreInstall();
            Container.BindFactory<IFoo, IFooFactory>()
                .To<Foo>().FromSubContainerResolve().ByNewContextPrefab(FooSubContainerPrefab);

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToSubContainerPrefabResourceSelf()
        {
            PreInstall();
            Container.BindFactory<Foo, Foo.Factory>()
                .FromSubContainerResolve().ByNewContextPrefabResource("TestBindFactory/FooSubContainer");

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToSubContainerPrefabResourceConcrete()
        {
            PreInstall();
            Container.BindFactory<IFoo, IFooFactory>()
                .To<Foo>().FromSubContainerResolve().ByNewContextPrefabResource("TestBindFactory/FooSubContainer");

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestUnderTransformGroup()
        {
            PreInstall();
            Container.BindFactory<Foo, Foo.Factory>()
                .FromNewComponentOnNewGameObject().UnderTransformGroup("Foos");

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            var child = GameObject.Find("Foos").transform.GetChild(0);

            Assert.IsNotNull(child.GetComponent<Foo>());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestUnderTransform()
        {
            PreInstall();
            var tempGameObject = new GameObject("Foo");

            Container.BindFactory<Foo, Foo.Factory>().FromNewComponentOnNewGameObject().
                UnderTransform(tempGameObject.transform);

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            Assert.IsNotNull(tempGameObject.transform.GetChild(0).GetComponent<Foo>());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestUnderTransformGetter()
        {
            PreInstall();
            var tempGameObject = new GameObject("Foo");

            Container.BindFactory<Foo, Foo.Factory>().FromNewComponentOnNewGameObject()
                .UnderTransform(context => tempGameObject.transform);

            PostInstall();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            Assert.IsNotNull(tempGameObject.transform.GetChild(0).GetComponent<Foo>());
            yield break;
        }

        public class CameraFactory2 : PlaceholderFactory<string, Camera>
        {
        }

        public class CameraFactory : PlaceholderFactory<Camera>
        {
        }

        public class Foo3 : MonoBehaviour
        {
            public class Factory : PlaceholderFactory<Foo3>
            {
            }
        }

        public class Foo2 : MonoBehaviour
        {
            [Inject]
            public int Value
            {
                get; private set;
            }

            public class Factory : PlaceholderFactory<Foo2>
            {
            }

            public class Factory2 : PlaceholderFactory<int, Foo2>
            {
            }
        }
    }
}

#endif
