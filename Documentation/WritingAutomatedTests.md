
## Writing Automated Unit Tests and Integration Tests

When writing properly loosely coupled code using dependency injection, it is much easier to isolate specific areas of your code base for the purposes of running tests on them without needing to fire up your entire project.  This can take the form of user-driven test-beds or fully automated tests using NUnit.  Automated tests are especially useful when used with a continuous integration server.  This allows you to automatically run the tests whenever new commits are pushed to source control.

There are two very basic helper classes included with Zenject that can make it easier to write automated tests for your game.  One is for Unit tests and the other is for Integration tests.  Both approaches are run via Unity's built in Test Runner (which also has a command line interface that you can hook up to a continuous integration server).  The main difference between the two is that Unit Tests are much smaller in scope and meant for testing a small subset of the classes in your application, whereas Integration Tests can be more expansive and can involve firing up many different systems.

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

Now, to test this class, create a new folder named Editor, then right click on it inside the Project tab, and select Create -> Zenject -> Unit Test.  Name it TestLogger.cs.  This will create a basic template that we can fill in with our tests.  Copy and paste the following:

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
        PreInstall ();
        Container.Bind<Logger> ().AsSingle ();
        PostInstall ();
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
    public void TestNullValue()
    {
        var logger = Container.Resolve<Logger>();
        Assert.Throws<System.ArgumentException> (() => logger.Write (null));
    }
}

```

To run it, open up Unity's test runner by selecting `Window -> Test Runner`.  Then make sure the EditMode tab is selected, then click Run All or right click on the specific test you want to run.

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
    public void TestNullValue()
    {
        Assert.Throws(() => _logger.Write(null));
    }
}
```

### Integration Tests

Integration tests, on the other hand, are executed in a similar environment to the scenes in your project.  Unlike unit tests, integration tests involve a `SceneContext` and `ProjectContext`, and any bindings to IInitializable, ITickable, and IDisposable will be executed just like when running your game normally.  It achieves this by using Unity's support for 'playmode tests'.

As a very simple example, let's say we have the following class we want to test:

```csharp
public class SpaceShip : MonoBehaviour
{
    [InjectOptional]
    public Vector3 Velocity
    {
        get; set;
    }

    public void Update()
    {
        transform.position += Velocity * Time.deltaTime;
    }
}
```

After adding this class to your project, you can add an integration test for it by right clicking somewhere in the Project tab and then selecting `Create -> Zenject -> Integration Test` and then naming it `SpaceShipTests.cs`.  This will create the following template code with everything you need to start writing your test:

```csharp
public class SpaceShipTests : ZenjectIntegrationTestFixture
{
    [UnityTest]
    public IEnumerator RunTest1()
    {
        // Setup initial state by creating game objects from scratch, loading prefabs/scenes, etc

        PreInstall();

        // Call Container.Bind methods

        PostInstall();

        // Add test assertions for expected state
        // Using Container.Resolve or [Inject] fields
        yield break;
    }
}
```

Let's fill in some test code for our SpaceShip class:

```csharp
public class SpaceShipTests : ZenjectIntegrationTestFixture
{
    [UnityTest]
    public IEnumerator TestVelocity()
    {
        PreInstall();

        Container.Bind<SpaceShip>().FromNewComponentOnNewGameObject()
            .AsSingle().WithArguments(new Vector3(1, 0, 0));

        PostInstall();

        var spaceShip = Container.Resolve<SpaceShip>();

        Assert.IsEqual(spaceShip.transform.position, Vector3.zero);

        yield return null;

        // Should move in the direction of the velocity
        Assert.That(spaceShip.transform.position.x > 0);
    }
}
```

All we're doing here is ensuring that the space ship moves in the same direction as its velocity.  If we had many tests to run on SpaceShip we could also change it to this:

```csharp
public class SpaceShipTests : ZenjectIntegrationTestFixture
{
    void CommonInstall()
    {
        PreInstall();

        Container.Bind<SpaceShip>().FromNewComponentOnNewGameObject()
            .AsSingle().WithArguments(new Vector3(1, 0, 0));

        PostInstall();
    }

    [Inject]
    SpaceShip _spaceship;

    [UnityTest]
    public IEnumerator TestInitialState()
    {
        CommonInstall();

        Assert.IsEqual(_spaceship.transform.position, Vector3.zero);
        Assert.IsEqual(_spaceship.Velocity, new Vector3(1, 0, 0));
        yield break;
    }

    [UnityTest]
    public IEnumerator TestVelocity()
    {
        CommonInstall();

        // Wait one frame to allow update logic for SpaceShip to run
        yield return null;

        // Should move in the direction of the velocity
        Assert.That(_spaceship.transform.position.x > 0);
    }
}
```

After PostInstall() is called, our integration test is injected, so we can define [Inject] fields on it like above if we don't want to call Container.Resolve for every test.

Note that we can yield our coroutine to test behaviour across time.  If you are unfamiliar with how Unity's test runner works (and in particular how 'playmode test' work) please see the [unity documentation](https://docs.unity3d.com/Manual/testing-editortestsrunner.html).

Every zenject integration test is broken up into three phases:

- Before PreInstall - Set up the initial scene how you want for your test. This could involve loading prefabs from the Resources directory, creating new GameObject's from scratch, etc.
- After PreInstall - Install all the bindings to the Container that you need for your test
- After PostInstall - At this point, all the non-lazy objects that we've bound to the container have been instantiated, all objects in the scene have been injected, and all IInitializable.Initialize methods have been called.  So we can now start adding Assert's to test the state, manipulate the runtime state of the objects, etc.

For a more real world example, let's add an integration test for one of the spaceship classes in the included sample project (SpaceFighter).  During the game, more and more enemies are spawned into the scene that the player has to defend themselves against.  Each enemy object contains a lot of moving parts within it, but does not have very many dependencies on other classes in the game.  Each enemy needs to know some state about the player but otherwise behaves more or less independently of the rest of the game, which means that it might be a good candidate for testing in isolation.

So what we can do is install only the bindings that we need to test one instance of the enemy class:

```csharp
public class EnemyTests : ZenjectIntegrationTestFixture
{
    void CommonInstall()
    {
        PreInstall();

        var gameSettings = GameSettingsInstaller.InstallFromResource(
            "SpaceFighter/GameSettings", Container);

        Container.Bind<EnemyFacade>().FromSubContainerResolve()
            .ByNewPrefab(gameSettings.GameInstaller.EnemyFacadePrefab);

        Container.BindMemoryPool<Bullet, Bullet.Pool>()
            .FromComponentInNewPrefab(gameSettings.GameInstaller.BulletPrefab);

        Container.BindMemoryPool<Explosion, Explosion.Pool>()
            .FromComponentInNewPrefab(gameSettings.GameInstaller.ExplosionPrefab);

        GameSignalsInstaller.Install(Container);

        // Don't care about these so just mock them out
        Container.Bind<IPlayer>().FromMock().AsSingle();
        Container.Bind<IAudioPlayer>().FromMock().AsSingle();

        PostInstall();
    }

    [Inject]
    EnemyFacade _enemy;

    [UnityTest]
    public IEnumerator TestEnemyStateChanges()
    {
        CommonInstall();

        // Should always start by chasing the player
        Assert.IsEqual(_enemy.State, EnemyStates.Follow);

        // Wait a frame for AI logic to run
        yield return null;

        // Our player mock is always at position zero, so if we move the _enemy there then the _enemy
        // should immediately go into attack mode
        _enemy.Position = Vector3.zero;

        // Wait a frame for AI logic to run
        yield return null;

        Assert.IsEqual(_enemy.State, EnemyStates.Attack);

        _enemy.Position = new Vector3(100, 100, 0);

        // Wait a frame for AI logic to run
        yield return null;

        // The enemy is very far away now, so it should return to searching for the player
        Assert.IsEqual(_enemy.State, EnemyStates.Follow);
    }
}
```

As you can see, integration tests can be very powerful, because they can be run in a very similar way to how the production scenes are run.  In this case, we can ensure that the AI of the enemy class attacks the player when the player is nearby, and otherwise moves closer to the player.  Also, because this is all automated, we can run this code on a continuous integration server, and have it tested against every code change.  Then as soon as anything breaks in the AI system of the enemy class, we will get notified automatically.

### User Driven Test Beds

A third common approach to testing worth mentioning is User Driven Test Beds.  This just involves creating a new scene with a SceneContext etc. just as you do for production scenes, except installing only a subset of the bindings that you would normally include in the production scenes, and possibly mocking out certain parts that you don't need to test.  Then, by iterating on the system you are working on using this test bed, it can be much faster to make progress rather than needing to fire up your normal production scene.

This might also be necessary if the functionality you want to test is too complex for a unit test or an integration test.

The only drawback with this approach is that it isn't automated, so you can't have these tests run as part of a continuous integration server

