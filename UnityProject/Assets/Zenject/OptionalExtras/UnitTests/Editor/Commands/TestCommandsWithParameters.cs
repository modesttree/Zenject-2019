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
    public class TestCommandsWithParameters : ZenjectIntegrationTestFixture
    {
        [SetUp]
        public void CommonInstall()
        {
            Container.BindAllInterfacesAndSelf<CommandManager>().To<CommandManager>().AsSingle();
        }

        [Test]
        public void TestParameters1()
        {
            Bar1.Value = null;

            Container.DeclareCommand<DoSomethingCommand1>();
            Container.HandleCommand<string, DoSomethingCommand1>()
                .With<Bar1>(x => x.Execute).AsSingle();

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand1>();

            Assert.IsNull(Bar1.Value);

            cmd.Execute("asdf");

            Assert.IsEqual(Bar1.Value, "asdf");
        }

        [Test]
        public void TestParameters2()
        {
            Bar2.Value1 = null;
            Bar2.Value2 = 0;

            Container.DeclareCommand<DoSomethingCommand2>();
            Container.HandleCommand<string, int, DoSomethingCommand2>()
                .With<Bar2>(x => x.Execute).AsSingle();

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand2>();

            Assert.IsNull(Bar2.Value1);
            Assert.IsEqual(Bar2.Value2, 0);

            cmd.Execute("asdf", 4);

            Assert.IsEqual(Bar2.Value1, "asdf");
            Assert.IsEqual(Bar2.Value2, 4);
        }

        [Test]
        public void TestParameters3()
        {
            Bar3.Value1 = null;
            Bar3.Value2 = 0;
            Bar3.Value3 = 0.0f;

            Container.DeclareCommand<DoSomethingCommand3>();
            Container.HandleCommand<string, int, float, DoSomethingCommand3>()
                .With<Bar3>(x => x.Execute).AsSingle();

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand3>();

            Assert.IsNull(Bar3.Value1);
            Assert.IsEqual(Bar3.Value2, 0);
            Assert.IsEqual(Bar3.Value3, 0.0f);

            cmd.Execute("asdf", 4, 7.2f);

            Assert.IsEqual(Bar3.Value1, "asdf");
            Assert.IsEqual(Bar3.Value2, 4);
            Assert.IsEqual(Bar3.Value3, 7.2f);
        }

        [Test]
        public void TestParameters4()
        {
            Bar4.Value1 = null;
            Bar4.Value2 = 0;
            Bar4.Value3 = 0.0f;
            Bar4.Value4 = '0';

            Container.DeclareCommand<DoSomethingCommand4>();
            Container.HandleCommand<string, int, float, char, DoSomethingCommand4>()
                .With<Bar4>(x => x.Execute).AsSingle();

            Initialize();

            var cmd = Container.Resolve<DoSomethingCommand4>();

            Assert.IsNull(Bar4.Value1);
            Assert.IsEqual(Bar4.Value2, 0);
            Assert.IsEqual(Bar4.Value3, 0.0f);
            Assert.IsEqual(Bar4.Value4, '0');

            cmd.Execute("asdf", 4, 7.2f, 'z');

            Assert.IsEqual(Bar4.Value1, "asdf");
            Assert.IsEqual(Bar4.Value2, 4);
            Assert.IsEqual(Bar4.Value3, 7.2f);
            Assert.IsEqual(Bar4.Value4, 'z');
        }

        public class DoSomethingCommand1 : Command<string> { }
        public class DoSomethingCommand2 : Command<string, int> { }
        public class DoSomethingCommand3 : Command<string, int, float> { }
        public class DoSomethingCommand4 : Command<string, int, float, char> { }

        public class Bar1
        {
            public static string Value;

            public void Execute(string value)
            {
                Value = value;
            }
        }

        public class Bar2
        {
            public static string Value1;
            public static int Value2;

            public void Execute(string value1, int value2)
            {
                Value1 = value1;
                Value2 = value2;
            }
        }

        public class Bar3
        {
            public static string Value1;
            public static int Value2;
            public static float Value3;

            public void Execute(string value1, int value2, float value3)
            {
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
            }
        }

        public class Bar4
        {
            public static string Value1;
            public static int Value2;
            public static float Value3;
            public static char Value4;

            public void Execute(string value1, int value2, float value3, char value4)
            {
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
                Value4 = value4;
            }
        }
    }
}


