using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestAllInjectionTypes : TestWithContainer
    {
        [Test]
        // Test all variations of injection
        public void TestCase1()
        {
            _container.Bind<Test0>().To(new Test0());
            _container.Bind<IFoo>().ToSingle<FooDerived>();

            TestAssert.That(_container.ValidateResolve<IFoo>().IsEmpty());
            var foo = _container.Resolve<IFoo>();

            TestAssert.That(foo.DidPostInjectBase);
            TestAssert.That(foo.DidPostInjectDerived);
        }

        class Test0
        {
        }

        interface IFoo
        {
            bool DidPostInjectBase
            {
                get;
            }

            bool DidPostInjectDerived
            {
                get;
            }
        }

        abstract class FooBase : IFoo
        {
            bool _didPostInjectBase;

            [Inject]
            public static Test0 BaseStaticFieldPublic = null;

            [Inject]
            private static Test0 BaseStaticFieldPrivate = null;

            [Inject]
            protected static Test0 BaseStaticFieldProtected = null;

            [Inject]
            public static Test0 BaseStaticPropertyPublic
            {
                get;
                set;
            }

            [Inject]
            private static Test0 BaseStaticPropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected static Test0 BaseStaticPropertyProtected
            {
                get;
                set;
            }

            // Instance
            [Inject]
            public Test0 BaseFieldPublic = null;

            [Inject]
            private Test0 BaseFieldPrivate = null;

            [Inject]
            protected Test0 BaseFieldProtected = null;

            [Inject]
            public Test0 BasePropertyPublic
            {
                get;
                set;
            }

            [Inject]
            private Test0 BasePropertyPrivate
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

            [PostInject]
            public void PostInjectBase()
            {
                TestAssert.IsNull(BaseStaticFieldPublic);
                TestAssert.IsNull(BaseStaticFieldPrivate);
                TestAssert.IsNull(BaseStaticFieldProtected);
                TestAssert.IsNull(BaseStaticPropertyPublic);
                TestAssert.IsNull(BaseStaticPropertyPrivate);
                TestAssert.IsNull(BaseStaticPropertyProtected);

                TestAssert.IsNotNull(BaseFieldPublic);
                TestAssert.IsNotNull(BaseFieldPrivate);
                TestAssert.IsNotNull(BaseFieldProtected);
                TestAssert.IsNotNull(BasePropertyPublic);
                TestAssert.IsNotNull(BasePropertyPrivate);
                TestAssert.IsNotNull(BasePropertyProtected);

                _didPostInjectBase = true;
            }

            public bool DidPostInjectBase
            {
                get
                {
                    return _didPostInjectBase;
                }
            }

            public abstract bool DidPostInjectDerived
            {
                get;
            }
        }

        class FooDerived : FooBase
        {
            public bool _didPostInject;
            public Test0 ConstructorParam;

            public override bool DidPostInjectDerived
            {
                get
                {
                    return _didPostInject;
                }
            }

            [Inject]
            public static Test0 DerivedStaticFieldPublic = null;

            [Inject]
            private static Test0 DerivedStaticFieldPrivate = null;

            [Inject]
            protected static Test0 DerivedStaticFieldProtected = null;

            [Inject]
            public static Test0 DerivedStaticPropertyPublic
            {
                get;
                set;
            }

            [Inject]
            private static Test0 DerivedStaticPropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected static Test0 DerivedStaticPropertyProtected
            {
                get;
                set;
            }

            // Instance
            public FooDerived(Test0 param)
            {
                ConstructorParam = param;
            }

            [PostInject]
            public void PostInject()
            {
                TestAssert.IsNull(DerivedStaticFieldPublic);
                TestAssert.IsNull(DerivedStaticFieldPrivate);
                TestAssert.IsNull(DerivedStaticFieldProtected);
                TestAssert.IsNull(DerivedStaticPropertyPublic);
                TestAssert.IsNull(DerivedStaticPropertyPrivate);
                TestAssert.IsNull(DerivedStaticPropertyProtected);

                TestAssert.IsNotNull(DerivedFieldPublic);
                TestAssert.IsNotNull(DerivedFieldPrivate);
                TestAssert.IsNotNull(DerivedFieldProtected);
                TestAssert.IsNotNull(DerivedPropertyPublic);
                TestAssert.IsNotNull(DerivedPropertyPrivate);
                TestAssert.IsNotNull(DerivedPropertyProtected);
                TestAssert.IsNotNull(ConstructorParam);

                _didPostInject = true;
            }

            [Inject]
            public Test0 DerivedFieldPublic = null;

            [Inject]
            private Test0 DerivedFieldPrivate = null;

            [Inject]
            protected Test0 DerivedFieldProtected = null;

            [Inject]
            public Test0 DerivedPropertyPublic
            {
                get;
                set;
            }

            [Inject]
            private Test0 DerivedPropertyPrivate
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


