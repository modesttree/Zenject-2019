using System.Linq;

using NUnit.Framework;

using Zenject.ReflectionBaking.Mono.Cecil.Rocks;

namespace Zenject.ReflectionBaking.Mono.Cecil.Tests {

	[TestFixture]
	public class ModuleDefinitionRocksTests : BaseTestFixture {

		[Test]
		public void GetAllTypesTest ()
		{
			TestCSharp ("Types.cs", module => {
				var sequence = new [] {
				module.GetType ("<Module>"),
				module.GetType ("Foo"),
				module.GetType ("Foo/Bar"),
				module.GetType ("Foo/Gazonk"),
				module.GetType ("Foo/Gazonk/Baz"),
				module.GetType ("Pan"),
			};

				Assert.IsTrue (sequence.SequenceEqual (module.GetAllTypes ()));
			});
		}
	}
}
