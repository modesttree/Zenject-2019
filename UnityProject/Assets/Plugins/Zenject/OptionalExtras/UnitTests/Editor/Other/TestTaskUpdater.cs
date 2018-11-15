using System;
using ModestTree.Util;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestTaskUpdater
    {
        DiContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();

            _container.Bind<TaskUpdater<ITickable>>().FromInstance(new TickablesTaskUpdater());
        }

        public void BindTickable<TTickable>(int priority) where TTickable : ITickable
        {
            _container.BindInterfacesAndSelfTo<TTickable>().AsSingle();
            _container.Bind<ValuePair<Type, int>>().FromInstance(ValuePair.New(typeof(TTickable), priority));
        }

        [Test]
        public void TestTickablesAreOptional()
        {
            Assert.IsNotNull(_container.Resolve<TaskUpdater<ITickable>>());
        }

        [Test]
        // Test that tickables get called in the correct order
        public void TestOrder()
        {
            BindTickable<Tickable3>(2);
            BindTickable<Tickable1>(0);
            BindTickable<Tickable2>(1);

            var taskUpdater = _container.Resolve<TaskUpdater<ITickable>>();

            var tick1 = _container.Resolve<Tickable1>();
            var tick2 = _container.Resolve<Tickable2>();
            var tick3 = _container.Resolve<Tickable3>();

            int tickCount = 0;

            tick1.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 0);
                tickCount++;
            };

            tick2.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 1);
                tickCount++;
            };

            tick3.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 2);
                tickCount++;
            };

            taskUpdater.UpdateAll();
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
