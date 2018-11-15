using NUnit.Framework;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestTransientMockProvider : ZenjectUnitTestFixture
    {
        public interface IFoo
        {
            int GetBar();
        }

        [Test]
        public void TestCase1()
        {
            // Commented out because this requires that zenject be installed with mocking support which isn't always the case

            //Container.FallbackProvider = new TransientMockProvider(Container);

            //var foo = Container.Resolve<IFoo>();

            //Assert.IsEqual(foo.GetBar(), 0);
        }
    }
}
