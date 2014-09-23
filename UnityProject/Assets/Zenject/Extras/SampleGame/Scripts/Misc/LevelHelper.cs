using System;
using ModestTree.Zenject.Api.Misc;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;
using Random=UnityEngine.Random;

namespace ModestTree.Asteroids
{
    public class LevelHelper
    {
        Camera _camera;

        public float Bottom
        {
            get
            {
                return -ExtentHeight;
            }
        }

        public float Top
        {
            get
            {
                return ExtentHeight;
            }
        }

        public float Left
        {
            get
            {
                return -ExtentWidth;
            }
        }

        public float Right
        {
            get
            {
                return ExtentWidth;
            }
        }

        public float ExtentHeight
        {
            get
            {
                return _camera.orthographicSize;
            }
        }

        public float Height
        {
            get
            {
                return ExtentHeight * 2.0f;
            }
        }

        public float ExtentWidth
        {
            get
            {
                return _camera.aspect * _camera.orthographicSize;
            }
        }

        public float Width
        {
            get
            {
                return ExtentWidth * 2.0f;
            }
        }

        public LevelHelper(
            [Inject(Cameras.Main)]
            Camera camera)
        {
            _camera = camera;
        }
    }
}

