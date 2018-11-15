#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;

namespace Zenject.Tests.Bindings
{
    public class TestFromResource : ZenjectIntegrationTestFixture
    {
        const string ResourcePath = "TestFromResource/TestTexture";
        const string ResourcePath2 = "TestFromResource/TestTexture2";

        [UnityTest]
        public IEnumerator TestBasic()
        {
            PreInstall();
            Container.Bind<Texture>().FromResource(ResourcePath);

            Container.Bind<Runner>().FromNewComponentOnNewGameObject().AsSingle().WithArguments(1).NonLazy();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestTransient()
        {
            PreInstall();
            Container.Bind<Texture>().FromResource(ResourcePath).AsTransient();
            Container.Bind<Texture>().FromResource(ResourcePath);
            Container.Bind<Texture>().To<Texture>().FromResource(ResourcePath);

            Container.Bind<Runner>().FromNewComponentOnNewGameObject().AsSingle().WithArguments(3).NonLazy();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCached()
        {
            PreInstall();
            Container.Bind<Texture>().FromResource(ResourcePath).AsSingle();

            Container.Bind<Runner>().FromNewComponentOnNewGameObject().AsSingle().WithArguments(1).NonLazy();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle()
        {
            PreInstall();
            Container.Bind(typeof(Texture), typeof(Texture)).To<Texture>().FromResource(ResourcePath).AsSingle();

            Container.Bind<Runner>().FromNewComponentOnNewGameObject().AsSingle().WithArguments(2).NonLazy();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingleWithError()
        {
            PreInstall();
            Container.Bind<Texture>().FromResource(ResourcePath).AsSingle();
            Container.Bind<Texture>().FromResource(ResourcePath2).AsSingle();

            Assert.Throws(() => Container.FlushBindings());

            PostInstall();
            yield break;
        }

        public class Runner : MonoBehaviour
        {
            List<Texture> _textures;

            [Inject]
            public void Construct(List<Texture> textures, int expectedAmount)
            {
                _textures = textures;

                Assert.IsEqual(textures.Count, expectedAmount);
            }

            void OnGUI()
            {
                int top = 0;

                foreach (var tex in _textures)
                {
                    var rect = new Rect(0, top, Screen.width * 0.5f, Screen.height * 0.5f);

                    GUI.DrawTexture(rect, tex);

                    top += 200;
                }
            }
        }
    }
}

#endif
