using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ModestTree;
using Zenject.Internal;

#pragma warning disable 414

namespace Zenject.Tests.Injection
{
    //[TestFixture]
    // Conclusion here is that the compiled expressions are basically identical to reflection
    // baking during the instantiate (though there are wins during initialization)
    // When compiled expressions are not used such as IL2CPP however there is a noticable
    // improvement of maybe 15-20% for instantiate
    public class TestInstantiateApproaches : ZenjectUnitTestFixture
    {
        //[Test]
        public void TestWithoutReflectionBaking()
        {
            Log.Trace("Average without baking: {0:0.000}", Run<FooDerivedNoBaking>());
        }

        //[Test]
        public void TestWithReflectionBaking()
        {
            Log.Trace("Average with baking: {0:0.000}", Run<FooDerivedBaked>());
        }

        double Run<T>()
        {
            Container.Bind<Test0>().FromInstance(new Test0());

            // Do not include initial reflection costs
            Container.Instantiate<T>();
            Container.Instantiate<T>();

            var measurements = new List<double>();

            for (int k = 0; k < 10; k++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < 10000; i++)
                {
                    Container.Instantiate<T>();
                }
                stopwatch.Stop();
                measurements.Add(stopwatch.Elapsed.TotalSeconds);
            }

            return measurements.Average();
        }

        [NoReflectionBaking]
        class Test0
        {
        }

        [NoReflectionBaking]
        abstract class FooBaseBaked
        {
            [Inject]
            public Test0 BaseFieldPublic;

            [Inject]
            Test0 BaseFieldPrivate;

            [Inject]
            protected Test0 BaseFieldProtected;

            [Inject]
            public Test0 BasePropertyPublic
            {
                get; set;
            }

            [Inject]
            Test0 BasePropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected Test0 BasePropertyProtected
            {
                get;
                set;
            }

            [Inject]
            public void PostInjectBase()
            {
                DidPostInjectBase = true;
            }

            public bool DidPostInjectBase
            {
                get; private set;
            }

            private static void __zenFieldSetter0(object P_0, object P_1)
            {
                ((FooBaseBaked)P_0).BaseFieldPublic = (Test0)P_1;
            }

            private static void __zenFieldSetter1(object P_0, object P_1)
            {
                ((FooBaseBaked)P_0).BaseFieldPrivate = (Test0)P_1;
            }

            private static void __zenFieldSetter2(object P_0, object P_1)
            {
                ((FooBaseBaked)P_0).BaseFieldProtected = (Test0)P_1;
            }

            private static void __zenPropertySetter0(object P_0, object P_1)
            {
                ((FooBaseBaked)P_0).BasePropertyPublic = (Test0)P_1;
            }

            private static void __zenPropertySetter1(object P_0, object P_1)
            {
                ((FooBaseBaked)P_0).BasePropertyPrivate = (Test0)P_1;
            }

            private static void __zenPropertySetter2(object P_0, object P_1)
            {
                ((FooBaseBaked)P_0).BasePropertyProtected = (Test0)P_1;
            }

            private static void __zenInjectMethod0(object P_0, object[] P_1)
            {
                ((FooBaseBaked)P_0).PostInjectBase();
            }

            [Preserve]
            private static InjectTypeInfo CreateInjectTypeInfo()
            {
                return new InjectTypeInfo(typeof(FooBaseBaked), new InjectTypeInfo.InjectConstructorInfo(null, new InjectableInfo[0]), new InjectTypeInfo.InjectMethodInfo[1]
                {
                    new InjectTypeInfo.InjectMethodInfo(__zenInjectMethod0, new InjectableInfo[0], "PostInjectBase")
                }, new InjectTypeInfo.InjectMemberInfo[6]
                {
                    new InjectTypeInfo.InjectMemberInfo(__zenFieldSetter0, new InjectableInfo(false, null, "BaseFieldPublic", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenFieldSetter1, new InjectableInfo(false, null, "BaseFieldPrivate", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenFieldSetter2, new InjectableInfo(false, null, "BaseFieldProtected", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenPropertySetter0, new InjectableInfo(false, null, "BasePropertyPublic", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenPropertySetter1, new InjectableInfo(false, null, "BasePropertyPrivate", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenPropertySetter2, new InjectableInfo(false, null, "BasePropertyProtected", typeof(Test0), null, 0))
                });
            }
        }

        [NoReflectionBaking]
        class FooDerivedBaked : FooBaseBaked
        {
            public Test0 ConstructorParam;

            // Instance
            public FooDerivedBaked(Test0 param)
            {
                ConstructorParam = param;
            }

            [Inject]
            public void PostInject()
            {
            }

            [Inject]
            public Test0 DerivedFieldPublic;

            [Inject]
            Test0 DerivedFieldPrivate;

            [Inject]
            protected Test0 DerivedFieldProtected;

            [Inject]
            public Test0 DerivedPropertyPublic
            {
                get; set;
            }

            [Inject]
            Test0 DerivedPropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected Test0 DerivedPropertyProtected
            {
                get;
                set;
            }

            private static object __zenCreate(object[] P_0)
            {
                return new FooDerivedBaked((Test0)P_0[0]);
            }

            private static void __zenFieldSetter0(object P_0, object P_1)
            {
                ((FooDerivedBaked)P_0).DerivedFieldPublic = (Test0)P_1;
            }

            private static void __zenFieldSetter1(object P_0, object P_1)
            {
                ((FooDerivedBaked)P_0).DerivedFieldPrivate = (Test0)P_1;
            }

            private static void __zenFieldSetter2(object P_0, object P_1)
            {
                ((FooDerivedBaked)P_0).DerivedFieldProtected = (Test0)P_1;
            }

            private static void __zenPropertySetter0(object P_0, object P_1)
            {
                ((FooDerivedBaked)P_0).DerivedPropertyPublic = (Test0)P_1;
            }

            private static void __zenPropertySetter1(object P_0, object P_1)
            {
                ((FooDerivedBaked)P_0).DerivedPropertyPrivate = (Test0)P_1;
            }

            private static void __zenPropertySetter2(object P_0, object P_1)
            {
                ((FooDerivedBaked)P_0).DerivedPropertyProtected = (Test0)P_1;
            }

            private static void __zenInjectMethod0(object P_0, object[] P_1)
            {
                ((FooDerivedBaked)P_0).PostInject();
            }

            [Preserve]
            private static InjectTypeInfo CreateInjectTypeInfo()
            {
                return new InjectTypeInfo(typeof(FooDerivedBaked), new InjectTypeInfo.InjectConstructorInfo(__zenCreate, new InjectableInfo[1]
                {
                    new InjectableInfo(false, null, "param", typeof(Test0), null, 0)
                }), new InjectTypeInfo.InjectMethodInfo[1]
                {
                    new InjectTypeInfo.InjectMethodInfo(__zenInjectMethod0, new InjectableInfo[0], "PostInject")
                }, new InjectTypeInfo.InjectMemberInfo[6]
                {
                    new InjectTypeInfo.InjectMemberInfo(__zenFieldSetter0, new InjectableInfo(false, null, "DerivedFieldPublic", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenFieldSetter1, new InjectableInfo(false, null, "DerivedFieldPrivate", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenFieldSetter2, new InjectableInfo(false, null, "DerivedFieldProtected", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenPropertySetter0, new InjectableInfo(false, null, "DerivedPropertyPublic", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenPropertySetter1, new InjectableInfo(false, null, "DerivedPropertyPrivate", typeof(Test0), null, 0)),
                    new InjectTypeInfo.InjectMemberInfo(__zenPropertySetter2, new InjectableInfo(false, null, "DerivedPropertyProtected", typeof(Test0), null, 0))
                });
            }
        }

        [NoReflectionBaking]
        abstract class FooBaseNoBaking
        {
            [Inject]
            public Test0 BaseFieldPublic;

            [Inject]
            Test0 BaseFieldPrivate;

            [Inject]
            protected Test0 BaseFieldProtected;

            [Inject]
            public Test0 BasePropertyPublic
            {
                get; set;
            }

            [Inject]
            Test0 BasePropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected Test0 BasePropertyProtected
            {
                get;
                set;
            }

            [Inject]
            public void PostInjectBase()
            {
                DidPostInjectBase = true;
            }

            public bool DidPostInjectBase
            {
                get; private set;
            }
        }

        [NoReflectionBaking]
        class FooDerivedNoBaking : FooBaseNoBaking
        {
            public Test0 ConstructorParam;

            // Instance
            public FooDerivedNoBaking(Test0 param)
            {
                ConstructorParam = param;
            }

            [Inject]
            public void PostInject()
            {
            }

            [Inject]
            public Test0 DerivedFieldPublic;

            [Inject]
            Test0 DerivedFieldPrivate;

            [Inject]
            protected Test0 DerivedFieldProtected;

            [Inject]
            public Test0 DerivedPropertyPublic
            {
                get; set;
            }

            [Inject]
            Test0 DerivedPropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected Test0 DerivedPropertyProtected
            {
                get;
                set;
            }
        }
    }
}
