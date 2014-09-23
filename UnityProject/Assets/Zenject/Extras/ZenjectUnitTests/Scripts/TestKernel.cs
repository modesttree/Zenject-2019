using System;
using System.Collections.Generic;
using System.Diagnostics;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using UnityEngine;
using TestAssert = NUnit.Framework.Assert;
using ModestTree.Zenject;
using ModestTree.Zenject.Test;

namespace ModestTree.Zenject
{
    [TestFixture]
    public class TestKernel
    {
        DiContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();

            _container.Bind<IKernel>().ToSingle<StandardKernel>();
            _container.Bind<StandardKernel>().ToSingle();
        }

        public void BindTickable<TTickable>(int priority) where TTickable : ITickable
        {
            _container.Bind<ITickable>().ToSingle<TTickable>();
            _container.Bind<Tuple<Type, int>>().To(Tuple.New(typeof(TTickable), priority));
        }

        [Test]
        public void TestTickablesAreOptional()
        {
            TestAssert.That(_container.ValidateResolve<StandardKernel>().IsEmpty());
            TestAssert.IsNotNull(_container.Resolve<StandardKernel>());
        }

        [Test]
        // Test that tickables get called in the correct order
        public void TestOrder()
        {
            _container.Bind<Tickable1>().ToSingle();
            _container.Bind<Tickable2>().ToSingle();
            _container.Bind<Tickable3>().ToSingle();

            BindTickable<Tickable3>(2);
            BindTickable<Tickable1>(0);
            BindTickable<Tickable2>(1);

            TestAssert.That(_container.ValidateResolve<StandardKernel>().IsEmpty());
            var kernel = _container.Resolve<StandardKernel>();

            TestAssert.That(_container.ValidateResolve<Tickable1>().IsEmpty());
            var tick1 = _container.Resolve<Tickable1>();
            TestAssert.That(_container.ValidateResolve<Tickable2>().IsEmpty());
            var tick2 = _container.Resolve<Tickable2>();
            TestAssert.That(_container.ValidateResolve<Tickable3>().IsEmpty());
            var tick3 = _container.Resolve<Tickable3>();

            int tickCount = 0;

            tick1.TickCalled += delegate
            {
                TestAssert.AreEqual(tickCount, 0);
                tickCount++;
            };

            tick2.TickCalled += delegate
            {
                TestAssert.AreEqual(tickCount, 1);
                tickCount++;
            };

            tick3.TickCalled += delegate
            {
                TestAssert.AreEqual(tickCount, 2);
                tickCount++;
            };

            kernel.UpdateAll();
        }

        class Tickable1 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }

        class Tickable2 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }

        class Tickable3 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }
    }
}
