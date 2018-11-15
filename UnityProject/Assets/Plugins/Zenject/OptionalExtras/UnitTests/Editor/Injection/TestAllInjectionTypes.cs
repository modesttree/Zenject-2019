using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestAllInjectionTypes : ZenjectUnitTestFixture
    {
        static int InjectCounter;

        [Test]
        // Test all variations of injection
        public void TestCase1()
        {
            Container.Bind<Test0>().FromInstance(new Test0()).NonLazy();
            Container.Bind<IFoo>().To<FooDerived>().AsSingle().NonLazy();

            InjectCounter = 1;

            var foo = Container.Resolve<IFoo>();

            Assert.That(foo.DidPostInjectBase);
            Assert.That(foo.DidPostInjectDerived);

            Assert.IsEqual(foo.BaseTypePropertyInjectCount, 1);
            Assert.IsEqual(foo.DerivedTypePropertyInjectCount, 2);

            Assert.IsEqual(foo.BaseTypeMethodInjectCount, 3);
            Assert.IsEqual(foo.DerivedTypeMethodInjectCount, 4);
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

            int BaseTypePropertyInjectCount
            {
                get;
            }

            int DerivedTypePropertyInjectCount
            {
                get;
            }

            int BaseTypeMethodInjectCount
            {
                get;
            }

            int DerivedTypeMethodInjectCount
            {
                get;
            }
        }

        abstract class FooBase : IFoo
        {
            bool _didPostInjectBase;

            [Inject]
            public static Test0 BaseStaticFieldPublic;

            [Inject]
            static Test0 BaseStaticFieldPrivate;

            [Inject]
            protected static Test0 BaseStaticFieldProtected;

            [Inject]
            public static Test0 BaseStaticPropertyPublic
            {
                get;
                set;
            }

            [Inject]
            static Test0 BaseStaticPropertyPrivate
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
            public Test0 BaseFieldPublic;

            [Inject]
            Test0 BaseFieldPrivate;

            [Inject]
            protected readonly Test0 BaseFieldProtected;

            Test0 _basePropertyPublicValue;

            [Inject]
            public Test0 BasePropertyPublic
            {
                get { return _basePropertyPublicValue; }
                set
                {
                    BaseTypePropertyInjectCount = InjectCounter++;
                    _basePropertyPublicValue = value;
                }
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
                Assert.IsNull(BaseStaticFieldPublic);
                Assert.IsNull(BaseStaticFieldPrivate);
                Assert.IsNull(BaseStaticFieldProtected);
                Assert.IsNull(BaseStaticPropertyPublic);
                Assert.IsNull(BaseStaticPropertyPrivate);
                Assert.IsNull(BaseStaticPropertyProtected);

                Assert.IsNotNull(BaseFieldPublic);
                Assert.IsNotNull(BaseFieldPrivate);
                Assert.IsNotNull(BaseFieldProtected);
                Assert.IsNotNull(BasePropertyPublic);
                Assert.IsNotNull(BasePropertyPrivate);
                Assert.IsNotNull(BasePropertyProtected);

                BaseTypeMethodInjectCount = InjectCounter++;

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

            public int BaseTypePropertyInjectCount
            {
                get; set;
            }

            public int BaseTypeMethodInjectCount
            {
                get; set;
            }

            public abstract int DerivedTypeMethodInjectCount
            {
                get;
            }

            public abstract int DerivedTypePropertyInjectCount
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
            public static Test0 DerivedStaticFieldPublic;

            [Inject]
            static Test0 DerivedStaticFieldPrivate;

            [Inject]
            protected static Test0 DerivedStaticFieldProtected;

            [Inject]
            public static Test0 DerivedStaticPropertyPublic
            {
                get;
                set;
            }

            [Inject]
            static Test0 DerivedStaticPropertyPrivate
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

            [Inject]
            public void PostInject()
            {
                Assert.IsNull(DerivedStaticFieldPublic);
                Assert.IsNull(DerivedStaticFieldPrivate);
                Assert.IsNull(DerivedStaticFieldProtected);
                Assert.IsNull(DerivedStaticPropertyPublic);
                Assert.IsNull(DerivedStaticPropertyPrivate);
                Assert.IsNull(DerivedStaticPropertyProtected);

                Assert.IsNotNull(DerivedFieldPublic);
                Assert.IsNotNull(DerivedFieldPrivate);
                Assert.IsNotNull(DerivedFieldProtected);
                Assert.IsNotNull(DerivedPropertyPublic);
                Assert.IsNotNull(DerivedPropertyPrivate);
                Assert.IsNotNull(DerivedPropertyProtected);
                Assert.IsNotNull(ConstructorParam);

                _derivedTypeMethodInjectCount = InjectCounter++;

                _didPostInject = true;
            }

            [Inject]
            public Test0 DerivedFieldPublic;

            [Inject]
            Test0 DerivedFieldPrivate;

            [Inject]
            protected Test0 DerivedFieldProtected;

            Test0 _derivedPropertyPublicValue;

            [Inject]
            public Test0 DerivedPropertyPublic
            {
                get { return _derivedPropertyPublicValue; }
                set
                {
                    _derivedTypePropertyInjectCount = InjectCounter++;
                    _derivedPropertyPublicValue = value;
                }
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

            int _derivedTypeMethodInjectCount;

            public override int DerivedTypeMethodInjectCount
            {
                get { return _derivedTypeMethodInjectCount; }
            }

            int _derivedTypePropertyInjectCount;

            public override int DerivedTypePropertyInjectCount
            {
                get { return _derivedTypePropertyInjectCount; }
            }
        }
    }
}


