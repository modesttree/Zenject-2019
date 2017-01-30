
## Writing Automated Unit Tests and Integration Tests

When writing properly loosely coupled code using dependency injection, it is much easier to isolate specific areas of your code base for the purposes of running tests on them without needing to fire up your entire project.  This can take the form of user-driven test-beds or fully automated tests using NUnit.  Automated tests are especially useful when used with a continuous integration server.  This allows you to automatically run the tests whenever new commits are pushed to source control.

There are two very basic helper classes included with Zenject that can make it easier to write automated tests for your game.  One is for Unit tests and the other is for Integration tests.  Both approaches are run via Unity's built in Editor Test Runner (which also has a command line interface that you can hook up to a continuous integration server).  The main difference between the two is that Unit Tests are much smaller in scope and meant for testing a small subset of the classes in your application, whereas Integration Tests can be more expansive and can involve firing up many different systems.

This is best shown with some examples.

### Unit Tests

As an example, let's add the following class to our Unity project:

```csharp
using System;

public class Logger
{
    public Logger()
    {
        Log = "";
    }

    public string Log
    {
        get;
        private set;
    }

    public void Write(string value)
    {
        if (value == null)
        {
            throw new ArgumentException();
        }

        Log += value;
    }
}
```

Now, to test this class, create a new folder named Editor, then add a new file named TestLogger.cs and copy and paste the following:

```csharp
using System;
using Zenject;
using NUnit.Framework;

[TestFixture]
public class TestLogger : ZenjectUnitTestFixture
{
    [SetUp]
    public void CommonInstall()
    {
        Container.Bind<Logger>().AsSingle();
    }

    [Test]
    public void TestInitialValues()
    {
        var logger = Container.Resolve<Logger>();

        Assert.That(logger.Log == "");
    }

    [Test]
    public void TestFirstEntry()
    {
        var logger = Container.Resolve<Logger>();

        logger.Write("foo");
        Assert.That(logger.Log == "foo");
    }

    [Test]
    public void TestAppend()
    {
        var logger = Container.Resolve<Logger>();

        logger.Write("foo");
        logger.Write("bar");

        Assert.That(logger.Log == "foobar");
    }

    [Test]
    [ExpectedException]
    public void TestNullValue()
    {
        var logger = Container.Resolve<Logger>();

        logger.Write(null);
    }
}

```

To run the tests open up Unity's test runner by selecting `Window -> Editor Tests Runner`.  Then click Run All or right click on the specific test you want to run.

As you can see above, this approach is very basic and just involves inheriting from the `ZenjectUnitTestFixture` class.  All this helper class does is ensure that a new Container is re-created before each test method is called.   That's it.  This is the entire code for it:

```csharp
public abstract class ZenjectUnitTestFixture
{
    DiContainer _container;

    protected DiContainer Container
    {
        get
        {
            return _container;
        }
    }

    [SetUp]
    public virtual void Setup()
    {
        _container = new DiContainer();
    }
}
```

So typically you run installers from within `[SetUp]` methods and then directly call `Resolve<>` to retrieve instances of the classes you want to test.

You could also avoid all the calls to `Container.Resolve` by injecting into the unit test itself after finishing the install, by changing your unit test to this:

```csharp
using System;
using Zenject;
using NUnit.Framework;

[TestFixture]
public class TestLogger : ZenjectUnitTestFixture
{
    [SetUp]
    public void CommonInstall()
    {
        Container.Bind<Logger>().AsSingle();
        Container.Inject(this);
    }

    [Inject]
    Logger _logger;

    [Test]
    public void TestInitialValues()
    {
        Assert.That(_logger.Log == "");
    }

    [Test]
    public void TestFirstEntry()
    {
        _logger.Write("foo");
        Assert.That(_logger.Log == "foo");
    }

    [Test]
    public void TestAppend()
    {
        _logger.Write("foo");
        _logger.Write("bar");

        Assert.That(_logger.Log == "foobar");
    }

    [Test]
    [ExpectedException]
    public void TestNullValue()
    {
        _logger.Write(null);
    }
}
```

### Integration Testss

Integration tests, on the other hand, are executed in a similar environment to the scenes in your project.  Unlike ZenjectUnitTestFixture, a `SceneContext` and `ProjectContext` are created for each test, so your code will run in similar way that it would normally.  For example, any bindings to IInitializable and IDisposable will be executed how you expect.

Let's pull from the included sample project and test one of the classes there (AsteroidManager):

```csharp
[TestFixture]
public class TestAsteroidManager : ZenjectIntegrationTestFixture
{
    [SetUp]
    public void CommonInstall()
    {
        GameSettingsInstaller.InstallFromResource(Container);
        var gameSettings = Container.Resolve<GameInstaller.Settings>();
        Container.Bind<AsteroidManager>().AsSingle();
        Container.BindFactory<Asteroid, Asteroid.Factory>().FromPrefab(gameSettings.AsteroidPrefab);
        Container.Bind<Camera>().WithId("Main").FromGameObject();
        Container.Bind<LevelHelper>().AsSingle();

        Initialize();
    }

    [Inject]
    AsteroidManager _asteroidManager;

    [Inject]
    AsteroidManager.Settings _asteroidManagerSettings;

    [Test]
    [ValidateOnly]
    public void TestValidate()
    {
    }

    [Test]
    public void TestInitialSpawns()
    {
        _asteroidManager.Start();

        Assert.AreEqual(_asteroidManager.Asteroids.Count(), _asteroidManagerSettings.startingSpawns);
    }

    [Test]
    public void TestSpawnNext()
    {
        _asteroidManager.Start();
        _asteroidManager.SpawnNext();

        Assert.AreEqual(_asteroidManager.Asteroids.Count(), _asteroidManagerSettings.startingSpawns + 1);
    }
}
```

Similar to unit tests, writing integration tests is similar to writing unit tests described above, except instead of having your test fixture inherit from `ZenjectUnitTestFixture`, you inherit from `ZenjectIntegrationTestFixture` instead.

We want to test the AsteroidManager class here, but it has a few dependencies that it needs in order to run.  And some of those dependencies have their own dependencies.  So what we do here is we add the minimum number of bindings necessary to be able to create a new AsteroidManager so that we can run some basic sanity tests on it.

When you start running a test through Unity's EditorTestRunner window, Unity will open up a new empty scene and run the tests there, and then Unity will re-open your previously open scenes once the tests complete.  This is great because any new game objects you create will not affect any currently open scenes (more details <a href="https://docs.unity3d.com/550/Documentation/Manual/testing-editortestsrunner.html">here</a>.)

Before each one of your test methods are run, ZenjectIntegrationTestFixture will create a new SceneContext in this empty scene.  The Container that you reference in your tests refers to the container used by this new temporary SceneContext.  After you have finished installing all your bindings on the Container, either by using [SetUp] methods or from within your test method, you need to call ZenjectIntegrationTestFixture.Initialize().  This will resolve all the NonLazy bindings and also trigger IInitializable.Initialize for all classes that are bound to IInitializable.  If you have common fields on your test marked with [Inject] then these fields are also filled in at this time.  Alternatively, you can diretcly call `Container.Resolve<>` from within your test methods to get the classes you want to run tests on.

You can also run zenject validation on your test by adding a `[ValidateOnly]` attribute above your test method (as shown in example above).  This will cause the test to not instantiate any of your bindings and instead just verify that the configuration of your Container is valid.  However, you could also just as easily rely on configuration errors to be caught when running normal tests too, so it probably has limited use for you.

Limitations:
- The tests are executed in the editor without entering play mode.  This means that any game objects you create will not have their Awake() or Start() methods called unless you do so explicitly
- Each test is executed in one frame of execution, so you can't test behaviour that occurs across frames

### User Driven Test Beds

A third common approach to testing worth mentioning is User Driven Test Beds.  This just involves creating a new scene with a SceneContext etc. just as you do for production scenes, except installing only a subset of the bindings that you would normally include in the production scenes, and possibly mocking out certain parts that you don't need to test.  Then, by iterating on the system you are working on using this test bed, it can be much faster to make progress rather than needing to fire up your normal production scene.

This might also be necessary if the functionality you want to test is too complex for a unit test or an integration test.

The only drawback with this approach is that it isn't automated, so you can't have these tests run as part of a continuous integration server

