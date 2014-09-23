using ModestTree.Zenject.Api.Misc;
using UnityEngine;

namespace ModestTree.Asteroids
{
    public enum GameStates
    {
        WaitingToStart,
        Playing,
        GameOver,
    }

    public class GameController : IInitializable, ITickable
    {
        Ship _ship;
        GameStates _state = GameStates.WaitingToStart;
        AsteroidManager _asteroidSpawner;
        float _elapsedTime;

        public float ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public GameStates State
        {
            get
            {
                return _state;
            }
        }

        public GameController(Ship ship, AsteroidManager asteroidSpawner)
        {
            _asteroidSpawner = asteroidSpawner;
            _ship = ship;
        }

        public void Initialize()
        {
            Physics.gravity = Vector3.zero;
            Screen.showCursor = false;
            GameEvent.ShipCrashed += OnShipCrashed;

            Debug.Log("Started Game");
        }

        public void Tick()
        {
            switch (_state)
            {
                case GameStates.WaitingToStart:
                {
                    UpdateStarting();
                    break;
                }
                case GameStates.Playing:
                {
                    UpdatePlaying();
                    break;
                }
                case GameStates.GameOver:
                {
                    UpdateGameOver();
                    break;
                }
                default:
                {
                    Assert.That(false);
                    break;
                }
            }
        }

        void UpdateGameOver()
        {
            Assert.That(_state == GameStates.GameOver);

            if (Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }

        void OnShipCrashed()
        {
            Assert.That(_state == GameStates.Playing);
            _state = GameStates.GameOver;
            _asteroidSpawner.Stop();
        }

        void UpdatePlaying()
        {
            Assert.That(_state == GameStates.Playing);
            _elapsedTime += Time.deltaTime;
        }

        void UpdateStarting()
        {
            Assert.That(_state == GameStates.WaitingToStart);

            if (Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }

        void StartGame()
        {
            Assert.That(_state == GameStates.WaitingToStart || _state == GameStates.GameOver);

            _ship.Position = Vector3.zero;
            _elapsedTime = 0;
            _asteroidSpawner.Start();
            _ship.ChangeState(ShipStates.Moving);
            _state = GameStates.Playing;
        }
    }
}
