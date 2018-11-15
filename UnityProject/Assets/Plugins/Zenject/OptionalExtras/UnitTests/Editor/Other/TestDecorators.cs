using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestDecorators : ZenjectUnitTestFixture
    {
        static int CallCounter;

        public interface ISaveHandler
        {
            void Save();
        }

        public class SaveHandler : ISaveHandler
        {
            public SaveHandler()
            {
                NumInstances++;
            }

            public static int CallCount
            {
                get; set;
            }

            public static int NumInstances
            {
                get; set;
            }

            public void Save()
            {
                CallCount = CallCounter++;
            }
        }

        public class SaveDecorator1 : ISaveHandler
        {
            readonly ISaveHandler _handler;

            public SaveDecorator1(ISaveHandler handler)
            {
                _handler = handler;
                NumInstances++;
            }

            public static int NumInstances
            {
                get; set;
            }

            public static int CallCount
            {
                get; set;
            }

            public void Save()
            {
                CallCount = CallCounter++;
                _handler.Save();
            }
        }

        public class SaveDecorator2 : ISaveHandler
        {
            readonly ISaveHandler _handler;

            public SaveDecorator2(ISaveHandler handler)
            {
                _handler = handler;
            }

            public static int CallCount
            {
                get; set;
            }

            public void Save()
            {
                CallCount = CallCounter++;
                _handler.Save();
            }
        }

        public class Foo
        {
        }

        [Test]
        public void TestSimpleCase()
        {
            Container.Bind<ISaveHandler>().To<SaveHandler>().AsSingle();
            Container.Decorate<ISaveHandler>().With<SaveDecorator1>();

            CallCounter = 1;
            SaveHandler.CallCount = 0;
            SaveDecorator1.CallCount = 0;

            Container.Resolve<ISaveHandler>().Save();

            Assert.IsEqual(SaveDecorator1.CallCount, 1);
            Assert.IsEqual(SaveHandler.CallCount, 2);
        }

        [Test]
        public void TestMultiple()
        {
            Container.Bind<ISaveHandler>().To<SaveHandler>().AsSingle();

            Container.Decorate<ISaveHandler>().With<SaveDecorator1>();
            Container.Decorate<ISaveHandler>().With<SaveDecorator2>();

            CallCounter = 1;
            SaveHandler.CallCount = 0;
            SaveDecorator1.CallCount = 0;
            SaveDecorator2.CallCount = 0;

            Container.Resolve<ISaveHandler>().Save();

            Assert.IsEqual(SaveDecorator2.CallCount, 1);
            Assert.IsEqual(SaveDecorator1.CallCount, 2);
            Assert.IsEqual(SaveHandler.CallCount, 3);
        }

        [Test]
        public void TestCaching()
        {
            Container.Bind<ISaveHandler>().To<SaveHandler>().AsTransient();
            Container.Decorate<ISaveHandler>().With<SaveDecorator1>();

            SaveHandler.NumInstances = 0;
            SaveDecorator1.NumInstances = 0;

            Container.Resolve<ISaveHandler>().Save();

            Assert.IsEqual(SaveHandler.NumInstances, 1);
            Assert.IsEqual(SaveDecorator1.NumInstances, 1);

            Container.Resolve<ISaveHandler>().Save();

            Assert.IsEqual(SaveHandler.NumInstances, 2);
            Assert.IsEqual(SaveDecorator1.NumInstances, 2);
        }

        [Test]
        public void TestCaching2()
        {
            Container.Bind<ISaveHandler>().To<SaveHandler>().AsCached();
            Container.Decorate<ISaveHandler>().With<SaveDecorator1>();

            SaveHandler.NumInstances = 0;
            SaveDecorator1.NumInstances = 0;

            Container.Resolve<ISaveHandler>().Save();

            Assert.IsEqual(SaveHandler.NumInstances, 1);
            Assert.IsEqual(SaveDecorator1.NumInstances, 1);

            Container.Resolve<ISaveHandler>().Save();

            Assert.IsEqual(SaveHandler.NumInstances, 1);
            Assert.IsEqual(SaveDecorator1.NumInstances, 1);
        }

        [Test]
        public void TestDecoratorMethod()
        {
            SaveHandler.NumInstances = 0;
            SaveDecorator1.CallCount = 0;

            bool wasCalled = false;

            Container.Bind<ISaveHandler>().To<SaveHandler>().AsSingle();
            Container.Decorate<ISaveHandler>()
                .With<SaveDecorator1>().FromMethod((x, h) =>
                        {
                            wasCalled = true;
                            return new SaveDecorator1(h);
                        });

            CallCounter = 1;
            Assert.That(!wasCalled);
            Assert.IsEqual(SaveHandler.NumInstances, 0);
            Assert.IsEqual(SaveDecorator1.CallCount, 0);

            Container.Resolve<ISaveHandler>().Save();

            Assert.That(wasCalled);
            Assert.IsEqual(SaveHandler.NumInstances, 1);
            Assert.IsEqual(SaveDecorator1.CallCount, 1);
        }

        [Test]
        public void TestContainerInheritance()
        {
            Container.Bind<ISaveHandler>().To<SaveHandler>().AsSingle();
            Container.Decorate<ISaveHandler>().With<SaveDecorator1>();

            var subContainer = Container.CreateSubContainer();

            CallCounter = 1;
            SaveHandler.CallCount = 0;
            SaveDecorator1.CallCount = 0;

            subContainer.Resolve<ISaveHandler>().Save();

            Assert.IsEqual(SaveDecorator1.CallCount, 1);
            Assert.IsEqual(SaveHandler.CallCount, 2);
        }


        // TODO - Fix this
        //[Test]
        //public void TestContainerInheritance2()
        //{
            //Container.Bind<ISaveHandler>().To<SaveHandler>().AsSingle();
            //Container.Decorate<ISaveHandler>().With<SaveDecorator1>();

            //var subContainer = Container.CreateSubContainer();
            //subContainer.Decorate<ISaveHandler>().With<SaveDecorator2>();

            //CallCounter = 1;
            //SaveHandler.CallCount = 0;
            //SaveDecorator1.CallCount = 0;
            //SaveDecorator2.CallCount = 0;

            //subContainer.Resolve<ISaveHandler>().Save();

            //Assert.IsEqual(SaveDecorator2.CallCount, 1);
            //Assert.IsEqual(SaveDecorator1.CallCount, 2);
            //Assert.IsEqual(SaveHandler.CallCount, 3);
        //}
    }
}
