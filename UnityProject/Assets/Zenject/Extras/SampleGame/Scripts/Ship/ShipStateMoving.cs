using System;
using ModestTree.Zenject.Api.Misc;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public class ShipStateMoving : ShipState
    {
        Settings _settings;
        Camera _mainCamera;
        Vector3 _lastPosition;
        float _oscillationTheta;

        public ShipStateMoving(
            Settings settings, Ship ship,
            [Inject(Cameras.Main)]
            Camera mainCamera)
            : base(ship)
        {
            _settings = settings;
            _mainCamera = mainCamera;
        }

        public override void Update()
        {
            UpdateThruster();
            Move();
            ApplyOscillation();
        }

        void ApplyOscillation()
        {
            var obj = _ship.MeshRenderer.gameObject;

            var cycleInterval = 1.0f / _settings.oscillationFrequency;
            var thetaMoveSpeed = 2 * Mathf.PI / cycleInterval;

            _oscillationTheta += thetaMoveSpeed * Time.deltaTime;

            obj.transform.position = obj.transform.parent.position + new Vector3(0, _settings.oscillationAmplitude * Mathf.Sin(_oscillationTheta), 0);
        }

        void UpdateThruster()
        {
            var speed = (_ship.Position - _lastPosition).magnitude / Time.deltaTime;
            var speedPx = Mathf.Clamp(speed / _settings.speedForMaxEmisssion, 0.0f, 1.0f);

            _ship.ParticleEmitter.maxEmission = _settings.maxEmission * speedPx;
        }

        void Move()
        {
            var mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var mousePos = mouseRay.origin;
            mousePos.z = 0;

            _lastPosition = _ship.Position;
            _ship.Position = Vector3.Lerp(_ship.Position, mousePos, Mathf.Min(1.0f, _settings.moveSpeed * Time.deltaTime));

            var moveDelta = _ship.Position - _lastPosition;
            var moveDistance = moveDelta.magnitude;

            if (moveDistance > 0.01f)
            {
                var moveDir = moveDelta / moveDistance;
                _ship.Rotation = Quaternion.LookRotation(-moveDir);
            }
        }

        public override void Start()
        {
            _lastPosition = _ship.Position;
            _ship.ParticleEmitter.emit = true;
        }

        public override void Stop()
        {
            _ship.ParticleEmitter.emit = false;
        }

        public override void OnTriggerEnter(Collider other)
        {
            Assert.That(other.tag == "asteroid");
            _ship.ChangeState(ShipStates.Dead, _ship);
        }

        [Serializable]
        public class Settings
        {
            public float moveSpeed;
            public float rotateSpeed;

            public float speedForMaxEmisssion;
            public float maxEmission;

            public float oscillationFrequency;
            public float oscillationAmplitude;
        }
    }
}
