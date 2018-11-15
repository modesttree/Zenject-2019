using System;
using UnityEngine;

namespace Zenject.Tests.TestDestructionOrder
{
    public class FooDisposable2 : IDisposable
    {
        public void Dispose()
        {
            Debug.Log("Destroyed FooDisposable2");
        }
    }
}
