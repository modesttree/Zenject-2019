using ModestTree;
using UnityEngine;

namespace Zenject.Tests
{
    public class SceneContextEventsTester : MonoBehaviour
    {
        [SerializeField]
        SceneContext _sceneContext;

        bool _calledPreInstall;
        bool _calledPostInstall;
        bool _calledPreResolve;
        bool _calledPostResolve;

        public void Awake()
        {
            Assert.That(!_sceneContext.HasResolved);
            Assert.That(!_sceneContext.HasInstalled);

            _sceneContext.PreInstall += OnPreInstall;
            _sceneContext.PostInstall += OnPostInstall;
            _sceneContext.PreResolve += OnPreResolve;
            _sceneContext.PostResolve += OnPostResolve;
        }

        public void Start()
        {
            Assert.That(_calledPreInstall);
            Assert.That(_calledPostInstall);
            Assert.That(_calledPreResolve);
            Assert.That(_calledPostResolve);
        }

        void OnPreInstall()
        {
            _calledPreInstall = true;
            Assert.IsNotNull(_sceneContext.Container);
        }

        void OnPostInstall()
        {
            _calledPostInstall = true;
        }

        void OnPreResolve()
        {
            _calledPreResolve = true;
        }

        void OnPostResolve()
        {
            _calledPostResolve = true;
        }
    }
}
