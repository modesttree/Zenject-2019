#if UNITY_EDITOR

using System.Linq;
using ModestTree;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zenject.Tests
{
    public static class FixtureUtil
    {
        public static GameObject GetPrefab(string resourcePath)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);
            Assert.IsNotNull(prefab, "Expected to find prefab at '{0}'", resourcePath);
            return prefab;
        }

        public static void AssertNumGameObjectsWithName(
            string name, int expectedNumGameObjects)
        {
            var numMatches = SceneManager.GetActiveScene()
                .GetRootGameObjects().Where(x => x.name == name).Count();

            Assert.IsEqual(
                numMatches, expectedNumGameObjects);
        }

        public static void AssertNumGameObjects(
            int expectedNumGameObjects)
        {
            var totalNumGameObjects =
                SceneManager.GetActiveScene().GetRootGameObjects().Count();

            // -1 because the scene context
            Assert.IsEqual(totalNumGameObjects - 1, expectedNumGameObjects);
        }

        public static void AssertComponentCount<TComponent>(
            int expectedNumComponents)
        {
            Assert.That(typeof(TComponent).DerivesFromOrEqual<Component>()
                || typeof(TComponent).IsAbstract());

            var actualCount = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<TComponent>()).Count();

            Assert.IsEqual(actualCount, expectedNumComponents,
                "Expected to find '{0}' components of type '{1}' but instead found '{2}'"
                .Fmt(expectedNumComponents, typeof(TComponent).PrettyName(), actualCount));
        }

        public static void AssertResolveCount<TContract>(
            DiContainer container, int expectedNum)
        {
            var actualCount = container.ResolveAll<TContract>().Count;
            Assert.That(actualCount == expectedNum,
                "Expected to find '{0}' instances of type '{1}' but instead found '{2}'",
                expectedNum, typeof(TContract).PrettyName(), actualCount);
        }

        public static void CallFactoryCreateMethod<TValue, TFactory>(DiContainer container)
            where TFactory : PlaceholderFactory<TValue>
        {
            container.Resolve<TFactory>().Create();
        }
    }
}

#endif
