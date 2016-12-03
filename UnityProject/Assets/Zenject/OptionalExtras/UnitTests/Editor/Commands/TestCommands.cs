using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestCommands : ZenjectIntegrationTestFixture
    {
        [SetUp]
        public void CommonInstall()
        {
            Container.BindAllInterfacesAndSelf<CommandManager>().To<CommandManager>().AsSingle();
        }

        [Test]
        public void TestToSingle1()
        {
            Bar.WasTriggered = false;
            Bar.InstanceCount = 0;

            Container.Bind<Bar>().AsSingle();

            Container.DeclareCommand<DoSomethingCommand>();
            Container.HandleCommand<DoSomethingCommand>()
                .With<Bar>(x => x.Execute).AsSingle();

            Initialize();

            Container.Resolve<Bar>();
            var cmd = Container.Resolve<DoSomethingCommand>();

            Assert.IsEqual(Bar.InstanceCount, 1);
            Assert.That(!Bar.WasTriggered);

            cmd.Execute();

            Assert.That(Bar.WasTriggered);
            Assert.IsEqual(Bar.InstanceCount, 1);
        }

        [Test]
        public void TestToCached1()
        {
            Bar.WasTriggered = false;
            Bar.InstanceCount = 0;

            Container.Bind<Bar>().AsCached();

            Container.DeclareCommand<DoSomethingCommand>();
            Container.HandleCommand<DoSomethingCommand>()
                .With<Bar>(x => x.Execute).AsCached();

            Initialize();

            Container.Resolve<Bar>();
            var cmd = Container.Resolve<DoSomethingCommand>();

            Assert.IsEqual(Bar.InstanceCount, 1);
            Assert.That(!Bar.WasTriggered);

            cmd.Execute();

            Assert.That(Bar.WasTriggered);
            Assert.IsEqual(Bar.InstanceCount, 2);

            cmd.Execute();
            Assert.IsEqual(Bar.InstanceCount, 2);
        }

        [Test]
        public void TestNoHandlerDefault()
        {
            Container.DeclareCommand<DoSomethingCommand>();

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand>();
            cmd.Execute();
        }

        [Test]
        public void TestNoHandlerRequiredFailure()
        {
            Container.DeclareCommand<DoSomethingCommand>().RequireHandler();

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand>();

            Assert.Throws(() => cmd.Execute());
        }

        [Test]
        public void TestNoHandlerRequiredSuccess()
        {
            Container.DeclareCommand<DoSomethingCommand>().RequireHandler();
            Container.HandleCommand<DoSomethingCommand>()
                .With<Bar>(x => x.Execute).AsCached();

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand>();
            cmd.Execute();
        }

        [Test]
        public void TestToMethod()
        {
            bool wasCalled = false;

            Container.DeclareCommand<DoSomethingCommand>();
            Container.HandleCommand<DoSomethingCommand>()
                .WithMethod(() => wasCalled = true);

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand>();

            Assert.That(!wasCalled);
            cmd.Execute();
            Assert.That(wasCalled);
        }

        [Test]
        public void TestMultipleHandlers()
        {
            bool wasCalled1 = false;
            bool wasCalled2 = false;

            Container.DeclareCommand<DoSomethingCommand>();
            Container.HandleCommand<DoSomethingCommand>()
                .WithMethod(() => wasCalled1 = true);
            Container.HandleCommand<DoSomethingCommand>()
                .WithMethod(() => wasCalled2 = true);

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand>();

            Assert.That(!wasCalled1);
            Assert.That(!wasCalled2);
            cmd.Execute();
            Assert.That(wasCalled1);
            Assert.That(wasCalled2);
        }

        public void TestMultipleHandlersError()
        {
            bool wasCalled1 = false;
            bool wasCalled2 = false;

            Container.DeclareCommand<DoSomethingCommand>()
                .RequireSingleHandler();

            Container.HandleCommand<DoSomethingCommand>()
                .WithMethod(() => wasCalled1 = true);
            Container.HandleCommand<DoSomethingCommand>()
                .WithMethod(() => wasCalled2 = true);

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand>();

            Assert.That(!wasCalled1);
            Assert.That(!wasCalled2);
            cmd.Execute();
            Assert.That(wasCalled1);
            Assert.That(wasCalled2);
        }

        public class DoSomethingCommand : Command
        {
        }

        public class Bar
        {
            public static int InstanceCount = 0;

            public Bar()
            {
                InstanceCount ++;
            }

            public static bool WasTriggered
            {
                get;
                set;
            }

            public void Execute()
            {
                WasTriggered = true;
            }
        }
    }
}

