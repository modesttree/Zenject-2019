
<img src="Build/ZenjectLogo.png?raw=true" alt="Zenject" width="600px" height="134px"/>

# Dependency Injection Framework for Unity3D

## NOTE

The following documentation is written to be packaged with Zenject as it appears in the Asset store (which you can find [here](http://u3d.as/content/modest-tree-media/zenject-dependency-injection/7ER))

## <a id="introduction"></a>Introduction

Zenject is a lightweight dependency injection framework built specifically to target Unity 3D.  It can be used to turn your Unity 3D application into a collection of loosely-coupled parts with highly segmented responsibilities.  Zenject can then glue the parts together in many different configurations to allow you to easily write, re-use, refactor and test your code in a scalable and extremely flexible way.

Tested in Unity 3D on the following platforms: PC/Mac/Linux, iOS, Android, WP8, Webplayer and WebGL (See <a href="#aot-support">here</a> for details on WebGL).

This project is open source.  You can find the official repository [here](https://github.com/modesttree/Zenject).

For general troubleshooting / support, please use the [zenject subreddit](http://www.reddit.com/r/zenject) or the [zenject google group](https://groups.google.com/forum/#!forum/zenject/).  If you have found a bug, you are also welcome to create an issue on the [github page](https://github.com/modesttree/Zenject), or a pull request if you have a fix / extension.  You can also follow [@Zenject](https://twitter.com/Zenject) on twitter for updates.  Finally, you can also email me directly at sfvermeulen@gmail.com

__Quick Start__:  If you are already familiar with dependency injection and are more interested in the syntax than anything else, you might want to start by looking over the <a href="#cheatsheet">cheatsheet</a> at the bottom of this page, which shows a bunch of typical example cases of usage.

## Table Of Contents

* <a href="#introduction">Introduction</a>
* <a href="#features">Features</a>
* <a href="#history">History</a>
* Dependency Injection
    * <a href="#theory">Theory</a>
    * <a href="#misconceptions">Misconceptions</a>
* Zenject API
    * <a href="#overview-of-the-zenject-api">Overview of the Zenject API</a>
        * <a href="#hello-world-example">Hello World Example</a>
        * <a href="#binding">Binding</a>
        * <a href="#inject-methods">Inject Methods</a>
        * <a href="#bind-methods">Bind Methods</a>
        * <a href="#list-bindings">List Bindings</a>
        * <a href="#optional-binding">Optional Binding</a>
        * <a href="#identifiers">Identifiers</a>
        * <a href="#conditional-bindings">Conditional Bindings</a>
        * <a href="#itickable">ITickable</a>
        * <a href="#iinitializable-and-postinject">IInitializable and PostInject</a>
        * <a href="#implementing-idisposable">Implementing IDisposable</a>
        * <a href="#installers">Installers</a>
    * <a href="#zenject-order-of-operations">Zenject Order Of Operations</a>
    * <a href="#di-guidelines--recommendations">Guidelines / Recommendations / Gotchas / Miscellaneous Tips and Tricks</a>
    * Advanced Features
        * <a href="#global-bindings">Global Bindings</a>
        * <a href="#update--initialization-order">Update Order And Initialization Order</a>
        * <a href="#object-graph-validation">Object Graph Validation</a>
        * <a href="#creating-objects-dynamically">Creating Objects Dynamically</a>
        * <a href="#using-the-unity-inspector-to-configure-settings">Using the Unity Inspector To Configure Settings</a>
        * <a href="#game-object-factories">Game Object Factories</a>
        * <a href="#abstract-factories">Abstract Factories</a>
        * <a href="#custom-factories">Custom Factories</a>
        * <a href="#injecting-data-across-scenes">Injecting Data Across Scenes</a>
        * <a href="#scenes-decorator">Scenes Decorators</a>
        * <a href="#advanced-factory-construction-using-subcontainers">Advanced Factory Construction Using SubContainers</a>
        * <a href="#sub-containers-and-facades">Sub-Containers and Facades</a>
        * <a href="#commands-and-signals">Commands And Signals</a>
        * <a href="#auto-mocking-using-moq">Auto-Mocking Using Moq</a>
* <a href="#questions">Frequently Asked Questions</a>
    * <a href="#aot-support">Does this work on AOT platforms such as iOS and WebGL?</a>
    * <a href="#faq-performance">How is Performance?</a>
    * <a href="#net-framework">Can I use .NET framework 4.0 and above?</a>
* <a href="#cheatsheet">Cheat Sheet</a>
* <a href="#further-help">Further Help</a>
* <a href="#release-notes">Release Notes</a>
* <a href="#license">License</a>

## <a id="features"></a>Features

* Injection into normal C# classes or MonoBehaviours
* Constructor injection (can tag constructor if there are multiple)
* Field injection
* Property injection
* Injection via [PostInject] method parameters
* Conditional binding (eg. by name, by parent type, etc.)
* Optional dependencies
* Support for building dynamic object graphs at runtime using factories
* Injection across different Unity scenes
* Support for global, project-wide bindings to apply to all scenes
* "Scene Decorators" which allow adding functionality to a different scene without changing it directly
* Ability to validate object graphs at editor time including dynamic object graphs created via factories
* Nested Containers / Sub-Containers
* Support for Commands and Signals
* Ability to easily define discrete 'islands' of dependencies using 'Facade' classes
* Auto-Mocking using the Moq library

## <a id="history"></a>History

Unity is a fantastic game engine, however the approach that new developers are encouraged to take does not lend itself well to writing large, flexible, or scalable code bases.  In particular, the default way that Unity manages dependencies between different game components can often be awkward and error prone.

Having worked on non-unity projects that use dependency management frameworks (such as Ninject, which Zenject takes a lot of inspiration from), the problem irked me enough that I decided a custom framework was necessary.  Upon googling for solutions, I found a series of great articles by Sebastiano Mandalà outlining the problem, which I strongly recommend that everyone read before firing up Zenject:

* [http://blog.sebaslab.com/ioc-container-for-unity3d-part-1/](http://blog.sebaslab.com/ioc-container-for-unity3d-part-1/)
* [http://blog.sebaslab.com/ioc-container-for-unity3d-part-2/](http://blog.sebaslab.com/ioc-container-for-unity3d-part-2/)

Sebastiano even wrote a proof of concept and open sourced it, which became the basis for this library.

What follows in the next section is a general overview of Dependency Injection from my perspective.  I highly recommend seeking other resources for more information on the subject, as there are many (often more intelligent) people that have written on the subject.  In particular, I highly recommend anything written by Mark Seeman on the subject - in particular his book 'Dependency Injection in .NET'.

Finally, I will just say that if you don't have experience with DI frameworks, and are writing object oriented code, then trust me, you will thank me later!  Once you learn how to write properly loosely coupled code using DI, there is simply no going back.

## <a id="theory"></a>Theory

When writing an individual class to achieve some functionality, it will likely need to interact with other classes in the system to achieve its goals.  One way to do this is to have the class itself create its dependencies, by calling concrete constructors:

```csharp
public class Foo
{
    ISomeService _service;

    public Foo()
    {
        _service = new SomeService();
    }

    public void DoSomething()
    {
        _service.PerformTask();
        ...
    }
}
```

This works fine for small projects, but as your project grows it starts to get unwieldy.  The class Foo is tightly coupled to class 'SomeService'.  If we decide later that we want to use a different concrete implementation then we have to go back into the Foo class to change it.

After thinking about this, often you come to the realization that ultimately, Foo shouldn't bother itself with the details of choosing the specific implementation of the service.  All Foo should care about is fulfilling its own specific responsibilities.  As long as the service fulfills the abstract interface required by Foo, Foo is happy.  Our class then becomes:

```csharp
public class Foo
{
    ISomeService _service;

    public Foo(ISomeService service)
    {
        _service = service;
    }

    public void DoSomething()
    {
        _service.PerformTask();
        ...
    }
}
```

This is better, but now whatever class is creating Foo (let's call it Bar) has the problem of filling in Foo's extra dependencies:

```csharp
public class Bar
{
    public void DoSomething()
    {
        var foo = new Foo(new SomeService());
        foo.DoSomething();
        ...
    }
}
```

And class Bar probably also doesn't really care about what specific implementation of SomeService Foo uses.  Therefore we push the dependency up again:

```csharp
public class Bar
{
    ISomeService _service;

    public Bar(ISomeService service)
    {
        _service = service;
    }

    public void DoSomething()
    {
        var foo = new Foo(_service);
        foo.DoSomething();
        ...
    }
}
```

So we find that it is useful to push the responsibility of deciding which specific implementations of which classes to use further and further up in the 'object graph' of the application.  Taking this to an extreme, we arrive at the entry point of the application, at which point all dependencies must be satisfied before things start.  The dependency injection lingo for this part of the application is called the 'composition root'.

## <a id="misconceptions"></a>Misconceptions

There are many misconceptions about DI, due to the fact that it can be tricky to fully wrap your head around at first.  It will take time and experience before it fully 'clicks'.

As shown in the above example, DI can be used to easily swap different implementations of a given interface (in the example this was ISomeService).  However, this is only one of many benefits that DI offers.

More important than that is the fact that using a dependency injection framework like Zenject allows you to more easily follow the '[Single Responsibility Principle](http://en.wikipedia.org/wiki/Single_responsibility_principle)'.  By letting Zenject worry about wiring up the classes, the classes themselves can just focus on fulfilling their specific responsibilities.

Another common mistake that people new to DI make is that they extract interfaces from every class, and use those interfaces everywhere instead of using the class directly.  The goal is to make code more loosely coupled, so it's reasonable to think that being bound to an interface is better than being bound to a concrete class.  However, in most cases the various responsibilities of an application have single, specific classes implementing them, so using an interfaces in these cases just adds unnecessary maintenance overhead.  Also, concrete classes already have an interface defined by their public members.  A good rule of thumb instead is to only create interfaces when the class has more than one implementation.  This is known, by the way, as the [Reused Abstraction Principle](http://codemanship.co.uk/parlezuml/blog/?postid=934))

Other benefits include:

* Testability - Writing automated unit tests or user-driven tests becomes very easy, because it is just a matter of writing a different 'composition root' which wires up the dependencies in a different way.  Want to only test one subsystem?  Simply create a new composition root.  Zenject also has some support for avoiding code duplication in the composition root itself (using Installers - described below). In cases where you can't easily separate out a specific sub-system to test, you can also creates 'mocks' for the sub-systems that you don't care about. (more detail <a href="#auto-mocking-using-moq">below</a>)
* Refactorability - When code is loosely coupled, as is the case when using DI properly, the entire code base is much more resilient to changes.  You can completely change parts of the code base without having those changes wreak havoc on other parts.
* Encourages modular code - When using a DI framework you will naturally follow better design practices, because it forces you to think about the interfaces between classes.

## <a id="overview-of-the-zenject-api"></a>Overview Of The Zenject API

What follows is a general overview of how DI patterns are applied using Zenject.  For further documentation I highly recommend the sample project itself (a kind of asteroids clone, which you can find by opening "Extras/SampleGame/Asteroids.unity").  I would recommend using that for reference after reading over these concepts.

You may also find the <a href="#cheatsheet">cheatsheet</a> at the bottom of this page helpful in understanding some typical usage scenarios.

The unit tests may also be helpful to show usage for each specific feature (which you can find by extracting Extras/ZenjectUnitTests.zip)

## <a id="hello-world-example"></a>Hello World Example

```csharp
using Zenject;
using UnityEngine;
using System.Collections;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ITickable>().ToSingle<TestRunner>();
        Container.Bind<IInitializable>().ToSingle<TestRunner>();
    }
}

public class TestRunner : ITickable, IInitializable
{
    public void Initialize()
    {
        Debug.Log("Hello World");
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Exiting!");
            Application.Quit();
        }
    }
}
```

You can run this example by doing the following:

* Copy and paste the above code into a file named 'TestInstaller'
* Create a new scene in Unity
* Right Click inside the Hierarchy tab and select Zenject -> Scene Composition Root
* Add your TestInstaller script to the scene as well (as its own GameObject or on the same GameObject as the CompositionRoot, it doesn't matter)
* Add a reference to your TestInstaller to the properties of the CompositionRoot by adding a new row in the inspector of the "Installers" property (Increase "Size" to 1) and then dragging the TestInstaller GameObject to it
* Validate your scene by either selecting Edit -> Zenject -> Validate Current Scene or hitting CTRL+SHIFT+V.  (note that this step isn't necessary but good practice to get into)
* Run
* Observe unity console for output

The CompositionRoot MonoBehaviour is the entry point of the application, where Zenject sets up all the various dependencies before kicking off your scene.  To add content to your Zenject scene, you need to write what is referred to in Zenject as an 'Installer', which declares all the dependencies used in your scene and their relationships with each other.  If the above doesn't make sense to you yet, keep reading!

## <a id="binding"></a>Binding

Every dependency injection framework is ultimately just a framework to bind types to instances.

In Zenject, dependency mapping is done by adding bindings to something called a container.  The container should then 'know' how to create all the object instances in our application, by recursively resolving all dependencies for a given object.

When the container is asked to construct an instance of a given type, it uses C# reflection to find the list of constructor arguments, and all fields/properties that are marked with an [Inject] attribute.  It then attempts to resolve each of these required dependencies, which it uses to call the constructor and create the new instance.

Each Zenject application therefore must tell the container how to resolve each of these dependencies, which is done via Bind commands.  For example, given the following class:

```csharp
public class Foo
{
    IBar _bar;

    public Foo(IBar bar)
    {
        _bar = bar;
    }
}
```

You can wire up the dependencies for this class with the following bindings:

```csharp
Container.Bind<Foo>().ToSingle();
Container.Bind<IBar>().ToSingle<Bar>();
```

This tells Zenject that every class that requires a dependency of type Foo should use the same instance, which it will automatically create when needed.  And similarly, any class that requires the IBar interface (like Foo) will be given the same instance of type Bar.

## <a id="inject-methods"></a>Inject Methods

There are many different ways of binding types on the container, which are documented in the <a href="#bind-methods">next section</a>.  There are also several ways of having these dependencies injected into your classes. These are:

1 - **Constructor Injection**

```csharp
public class Foo
{
    IBar _bar;

    public Foo(IBar bar)
    {
        _bar = bar;
    }
}
```

Here, the IBar dependency is injected via the constructor.

2 - **Field Injection**

```csharp
public class Foo
{
    [Inject]
    IBar _bar;
}
```

Field injection occurs immediately after the constructor is called.  All fields that are marked with the [Inject] attribute are looked up in the container and given a value.  Note that these fields can be private or public and injection will still occur.

3 - **Property Injection**

```csharp
public class Foo
{
    [Inject]
    public IBar Bar
    {
        get;
        private set;
    }
}
```

Property injection works the same as field injection except is applied to C# properties.  Just like fields, the setter can be private or public in this case.

4 - **PostInject Injection**

```csharp
public class Foo
{
    IBar _bar;

    [PostInject]
    public Init(IBar bar)
    {
        _bar = bar;
    }
}
```

PostInject injection works very similar to constructor injection.  The PostInject methods are called after all other dependencies have been resolved, and can be used to execute initialization logic.  Unlike constructor injection, PostInject methods can make use of any fields that are marked as [Inject] because all other injection methods are guaranteed to have completed before the PostInject methods are called.

You may also pass dependencies in as parameters to the PostInject method similar to how constructor injection works.

Note that there can be any number of PostInject methods.  In this case, they are called in the order of Base class to Derived class.  This can be useful to avoid the need to forward many dependencies from derived classes to the base class via constructor parameters, while also guaranteeing that the base class PostInject completes first, just like how constructors work.

Using [PostInject] to inject dependencies is the recommended approach for MonoBehaviours, since MonoBehaviours cannot have constructors.

**Recommendations**

* Best practice is to prefer constructor injection or [PostInject] injection to field or property injection.
    * Constructor/[PostInject] injection forces the dependency to only be resolved once, at class creation, which is usually what you want.  In many cases you don't want to expose a public property with your internal dependencies (although you can also [Inject] on private fields/properties)
    * Constructor injection guarantees no circular dependencies between classes, which is generally a bad thing to do.  You can do this however using [PostInject] or field injection if necessary.
    * Constructor/[PostInject] injection is more portable for cases where you decide to re-use the code without a DI framework such as Zenject.  You can do the same with public properties but it's more error prone (it's easier to forget to initialize one field and leave the object in an invalid state)
    * Finally, Constructor/[PostInject] injection makes it clear what all the dependencies of a class are when another programmer is reading the code.  They can simply look at the parameter list of the method.

## <a id="bind-methods"></a>Bind Methods

See the <a href="#binding">binding section</a> above for a general overview of binding.  The format for the bind command can be any of the following:

Note that you can find more examples in the <a href="#cheatsheet">cheatsheet</a> section below.

1 - **ToSingle** - Inject as singleton

```csharp
Container.Bind<Foo>().ToSingle();
```

When a type is bound using ToSingle this will construct one and only one instance of Foo and use that everywhere that has Foo as a dependency

You may also bind the singleton instance to one or more interfaces:

```csharp
Container.Bind<IFoo>().ToSingle<Foo>();
Container.Bind<IBar>().ToSingle<Foo>();
```

This will cause any dependencies of type IFoo or IBar to use the same instance of Foo.  Of course, Foo must implement both IFoo and IBar for this to compile.  However, with only the above two lines the Foo singleton will not be accessible directly.  You can achieve this by using another line to uses ToSingle directly:

```csharp
Container.Bind<Foo>().ToSingle();
Container.Bind<IFoo>().ToSingle<Foo>();
Container.Bind<IBar>().ToSingle<Foo>();
```

Note again that the same instance will be used for all dependencies that take Foo, IFoo, or IBar.

2 - **ToInstance** - Inject as a specific instance

```csharp
Container.Bind<Foo>().ToInstance(new Foo());
Container.Bind<string>().ToInstance("foo");
Container.Bind<int>().ToInstance(42);

// Or with shortcut:
Container.BindInstance(new Bar());
```

In this case the given instance will be used for every dependency with the given type

3 - **ToTransient** - Inject as newly created object

```csharp
Container.Bind<Foo>().ToTransient();
```

In this case a new instance of Foo will be generated each time it is injected. Similar to ToSingle, you can bind via an interface as well:

```csharp
Container.Bind<IFoo>().ToTransient<Foo>();
```

4 - **ToSinglePrefab** - Inject by instantiating a unity prefab once and using that everywhere

```csharp
Container.Bind<FooMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
```

This will instantiate a new instance of the given prefab, and then search the newly created game object for the given component (in this case FooMonoBehaviour).

Also, because it is ToSingle it will only instantiate the prefab once, and otherwise use the same instance of FooMonoBehaviour.

You can also bind multiple singletons to the same prefab.  For example:

```csharp
Container.Bind<FooMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
Container.Bind<BarMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
```

This will result in the prefab `PrefabGameObject` being instantiated once, and then searched for monobehaviour's `FooMonoBehaviour` and `BarMonoBehaviour`

5 - **ToSinglePrefabResource** - Load prefab via resources folder

Same as ToSinglePrefab except loads the prefab using a path in Resources folder

```csharp
Container.Bind<FooMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
```

In this example, I've placed my prefab at Assets/Resources/MyDirectory/MyPrefab.prefab.  By doing this I don't have to pass in a GameObject and can refer to it by the path within the resources folder.

Note that you can re-use the same singleton instance for multiple monobehaviours that exist on the prefab.

```csharp
Container.Bind<FooMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
Container.Bind<BarMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
```

In the above example, the prefab will only be instantiated once.

6 - **ToTransientPrefab** - Inject by instantiating a unity prefab each time

```csharp
Container.Bind<FooMonoBehaviour>().ToTransientPrefab(PrefabGameObject);
```

This works similar to ToSinglePrefab except it will instantiate a new instance of the given prefab every time the dependency is injected.

7 - **ToTransientPrefabResource** - Load prefab via resources folder

Same as ToTransientPrefab except loads the prefab using a path in Resources folder

```csharp
Container.Bind<FooMonoBehaviour>().ToTransientPrefabResource("MyDirectory/MyPrefab");
```

In the above example, I've placed my prefab at Assets/Resources/MyDirectory/MyPrefab.prefab.  By doing this I don't have to pass in a GameObject and can refer to it by the path within the resources folder.

8 - **ToSingleGameObject** - Inject by instantiating a new game object and using that everywhere

```csharp
Container.Bind<FooMonoBehaviour>().ToSingleGameObject();
```

This binding will create a new game object and attach the given FooMonoBehaviour.  Also note that since it is ToSingle that it will use the same instance everywhere that has FooMonoBehaviour as a dependency

9 - **ToMethod** - Inject using a custom method

This binding allows you to customize creation logic yourself by defining a method:

```csharp
Container.Bind<IFoo>().ToMethod(SomeMethod);
```

```csharp
public IFoo SomeMethod(InjectContext context)
{
    ...
    return new Foo();
}
```

10 - **ToSingleMethod** - Inject using a custom method but only call that method once

This binding works similar to `ToMethod` except that the given method will only be called once.  The value returned from the method will then be used for every subsequent request for the given dependency.

```csharp
Container.Bind<IFoo>().ToSingleMethod(SomeMethod);
```

```csharp
public IFoo SomeMethod(InjectContext context)
{
    ...
    return new Foo();
}
```

11 - **ToGetter** - Inject by getter.

This method can be useful if you want to bind to a property of another object.

```csharp
Container.Bind<IFoo>().ToSingle<Foo>()
Container.Bind<Bar>().ToGetter<IFoo>(x => x.GetBar())
```

12 - **ToLookup** - Inject by recursive resolve.

```csharp
Container.Bind<IFoo>().ToLookup<IBar>()
Container.Bind<IBar>().ToLookup<Foo>()
```

In some cases it is useful to be able to bind an interface to another interface.  However, you cannot use ToSingle or ToTransient because they both require concrete types.

In the example code above we assume that Foo inherits from IBar, which inherits from IFoo.  The result here will be that all dependencies for IFoo will be bound to whatever IBar is bound to (in this case, Foo).

You can also supply an identifier to the ToLookup() method.  See <a href="#identifiers">here</a> section for details on identifiers.

13 - **Rebind** - Override existing binding

```csharp
Container.Rebind<IFoo>().To<Foo>();
```

The Rebind function can be used to override any existing bindings that were added previously.  It will first clear all previous bindings and then add the new binding.  This method is especially useful for tests, where you often want to use almost all the same bindings used in production, except override a few specific bindings.

14 - **BindAllInterfacesToSingle**

This function can be used to automatically bind any interfaces that it finds on the given type.

```csharp
public class Foo : ITickable, IInitializable
{
    ...
}

...

Container.Bind<Foo>().ToSingle();
Container.BindAllInterfacesToSingle<Foo>();
```

The above is equivalent to the following:

```csharp
Container.Bind<Foo>().ToSingle();
Container.Bind<ITickable>().ToSingle<Foo>();
Container.Bind<IInitializable>().ToSingle<Foo>();
```

15 - **BindAllInterfacesToInstance**

This function works very similar to BindAllInterfacesToSingle.

```csharp
public class Foo : ITickable, IInitializable
{
    ...
}

...

Container.BindAllInterfacesToInstance(new Foo());
```

The above is equivalent to the following:

```csharp
var foo = new Foo();
Container.Bind<ITickable>().ToInstance(foo);
Container.Bind<IInitializable>().ToInstance(foo);
```

16 - **ToSingleInstance** - Use the given instance as a singleton.

This is the same as `ToInstance` except it will ensure that there is only ever one instance for the given type.

When using `ToInstance` you can do the following:

    Container.Bind<Foo>().ToInstance(new Foo());
    Container.Bind<Foo>().ToInstance(new Foo());
    Container.Bind<Foo>().ToInstance(new Foo());

Or, equivalently:

    Container.BindInstance(new Foo());
    Container.BindInstance(new Foo());
    Container.BindInstance(new Foo());

And then have a class that takes all of them as a list like this:

    public class Bar
    {
        public Bar(List<Foo> foos)
        {
        }
    }

Whereas, if you use ToSingleInstance this would trigger an error.

17 - **ToSingleFactory** - Define a custom factory for a singleton

```csharp
Container.Bind<IFoo>().ToSingleFactory<MyCustomFactory>();

class MyCustomFactory : IFactory<IFoo>
{
    Bar _bar;

    public MyCustomFactory(Bar bar)
    {
        _bar = bar;
    }

    public IFoo Create()
    {
        ...
    }
}
```

The `ToSingleFactory` binding can be useful when you want to define a singleton, but it has complex construction logic that you want to define yourself.  You could use `ToSingleMethod`, but this can get ugly if your construction logic itself has its own dependencies that it needs.  Using `ToSingleFactory` for this case it is nice because any dependencies that you require for construction can be simply added to the factory constructor

18 - **Untyped Bindings**

```csharp
Container.Bind(typeof(IFoo)).ToSingle(typeof(Foo));
```

In some cases it is not possible to use the generic versions of the Bind<> functions.  In these cases a non-generic version is provided, which works by taking in a Type value as a parameter.

19 - **BindIFactory** - Bind type `IFactory<>` to a construction method

```csharp
Container.BindIFactory<IFoo>().ToFactory<Foo>();
```

This bind method is used to create abstract factories.

The above line results in all dependencies of type `IFactory<IFoo>` being bound to an object that returns type `Foo` when its Create() method is called.

See the <a href="#abstract-factories">abstract factories section</a> for more information on abstract factories.

20 - **BindFacadeFactory** - Declare a nested container facade factory.

See <a href="#sub-containers-and-facades">this section</a> for more details on this binding.

21 - **BindFacade** - Declare a nested container facade.

See <a href="#sub-containers-and-facades">this section</a> for more details on this binding.


## <a id="list-bindings"></a>List Bindings

When Zenject finds multiple bindings for the same type, it interprets that to be a list.  So, in the example code below, Bar would get a list containing a new instance of Foo1, Foo2, and Foo3:

```csharp
// In an installer somewhere
Container.Bind<IFoo>().ToSingle<Foo1>();
Container.Bind<IFoo>().ToSingle<Foo2>();
Container.Bind<IFoo>().ToSingle<Foo3>();

...

public class Bar
{
    public Bar(List<IFoo> foos)
    {
    }
}
```

Also worth noting is that if you try and declare a single dependency of IFoo (like Bar below) and there are multiple bindings for it, then Zenject will throw an exception, since Zenject doesn't know which instance of IFoo to use.

```csharp
public class Bar
{
    public Bar(IFoo foo)
    {
    }
}
```

Also, if the empty list is valid, then you should mark your List constructor parameter (or [Inject] field) as optional (see <a href="#optional-binding">here</a> for details).

## <a id="optional-binding"></a>Optional Binding

You can declare some dependencies as optional as follows:

```csharp
public class Bar
{
    public Bar(
        [InjectOptional]
        IFoo foo)
    {
        ...
    }
}
...

// Can leave this commented or not and it will still work
// Container.Bind<IFoo>().ToSingle();
```

In this case, if IFoo is not bound in any installers, then it will be passed as null.

Note that when declaring dependencies with primitive types as optional, they will be given their default value (eg. 0 for ints).  You may also assign an explicit default using the standard C# way such as:

```csharp
public class Bar
{
    public Bar(int foo = 5)
    {
        ...
    }
}
...

// Can leave this commented or not and it will still work
// Container.Bind<int>().ToInstance(1);
```

Note also that the [InjectOptional] is not necessary in this case, since it's already implied by the default value.

Alternatively, you can define the primitive parameter as nullable, and perform logic depending on whether it is supplied or not, such as:

```csharp
public class Bar
{
    int _foo;

    public Bar(
        [InjectOptional]
        int? foo)
    {
        if (foo == null)
        {
            // Use 5 if unspecified
            _foo = 5;
        }
        else
        {
            _foo = foo.Value;
        }
    }
}

...

// Can leave this commented or not and it will still work
// Container.Bind<int>().ToInstance(1);
```

## <a id="identifiers"></a>Identifiers

You can also give a name to your binding by supplying it as a parameter to the Bind() method.  For example:

```csharp
Container.Bind<IFoo>("foo").ToSingle<Foo1>();
Container.Bind<IFoo>().ToSingle<Foo2>();

...

public class Bar1
{
    [Inject("foo")]
    IFoo _foo;
}

public class Bar2
{
    [Inject]
    IFoo _foo;
}
```

In this example, the Bar1 class will be given an instance of Foo1, and the Bar2 class will use the default version of IFoo which is bound to Foo2.

Note also that you can add a name to constructor arguments as well, for example:

```csharp
public class Bar
{
    Foo _foo;

    public Bar(
        [Inject("foo")] Foo foo)
    {
    }
}
```

You can also do the same with [PostInject] parameters:

```csharp
public class Bar
{
    Foo _foo;

    public Bar()
    {
    }

    [PostInject]
    public Init(
        [Inject("foo")] Foo foo)
    {
    }
}
```

It is also possible to supply an identifier to the ToSingle methods as well.  This allows you to force Zenject to create multiple singletons instead of just one.  Normally, the singleton is uniquely identified based on the type given as generic argument to the ToSingle<> method.  So for example:

```csharp
Container.Bind<IFoo>().ToSingle<Foo>();
Container.Bind<IBar>().ToSingle<Foo>();
Container.Bind<IQux>().ToSingle<Qux>();
```

In the above code, both uses of `ToSingle<Foo>()` will be bound to the same instance.  Only one instance of Foo will be created.

```csharp
Container.Bind<IFoo>().ToSingle<Foo>("foo1");
Container.Bind<IBar>().ToSingle<Foo>("foo2");
```

In this case, two instances will be created.

Something else worth noting is that the behaviour of ToSinglePrefab works similarly.  Given the following:

```csharp
Container.Bind<IFoo>().ToSingleFromPrefab<Foo>(MyPrefab);
Container.Bind<IBar>().ToSingleFromPrefab<Bar>(MyPrefab);
```

It will only instantiate the prefab MyPrefab once.  The generic parameter given to ToSingleFromPrefab can be interpreted as "Search the instantiated prefab for this component".  If instead, you want Zenject to instantiate a new instance of the prefab for each ToSingleFromPrefab binding, then you can do that as well by supplying an identifier to the ToSingleFromPrefab function like so:

```csharp
Container.Bind<IFoo>().ToSingleFromPrefab<Foo>("foo", MyPrefab);
Container.Bind<IBar>().ToSingleFromPrefab<Bar>("bar", MyPrefab);
```

Now two instances of the prefab will be created.

## <a id="conditional-bindings"></a>Conditional Bindings

In many cases you will want to restrict where a given dependency is injected.  You can do this using the following syntax:

```csharp
Container.Bind<IFoo>().ToSingle<Foo1>().WhenInjectedInto<Bar1>();
Container.Bind<IFoo>().ToSingle<Foo2>().WhenInjectedInto<Bar2>();
```

Note that `WhenInjectedInto` is simple shorthand for the following.  You can use the more general `When()` method for more complex conditionals.  For example, this is equivalent to WhenInjectedInto:

```csharp
Container.Bind<IFoo>().ToSingle<Foo>().When(context => context.ObjectType == typeof(Bar));
```

The InjectContext class (which is passed as the `context` parameter above) contains the following information that you can use in your conditional:

* `Type ObjectType` - The type of the newly instantiated object, which we are injecting dependencies into.  Note that this is null for root calls to Resolve<> or Instantiate<>
* `object ObjectInstance` - The newly instantiated instance that is having its dependencies filled.  Note that this is only available when injecting fields or into [PostInject] and null for constructor parameters
* `string Identifier` - This will be null in most cases and set to whatever is given as a parameter to the [Inject] attribute.  For example, `[Inject("foo")] _foo` will result in `Identifier` being equal to the string "foo".
* `string ConcreteIdentifier` - This will be null in most cases and set to whatever is given as the string identifier to the ToSingle<> (or similar) methods.
* `string MemberName` - The name of the field or parameter that we are injecting into.  This can be used, for example, in the case where you have multiple constructor parameters that are strings.  However, using the parameter or field name can be error prone since other programmers may refactor it to use a different name.  In many cases it's better to use an explicit identifier
* `Type MemberType` - The type of the field or parameter that we are injecting into.
* `InjectContext ParentContext` - This contains information on the entire object graph that precedes the current class being created.  For example, dependency A might be created, which requires an instance of B, which requires an instance of C.  You could use this field to inject different values into C, based on some condition about A.  This can be used to create very complex conditions using any combination of parent context information.  Note also that ParentContext.MemberType is not necessarily the same as ObjectType, since the ObjectType could be a derived type from ParentContext.MemberType
* `bool Optional` - True if the [InjectOptional] parameter is declared on the field being injected

## <a id="itickable"></a>ITickable

I personally prefer to avoid the extra weight of MonoBehaviours when possible in favour of just normal C# classes.  Zenject allows you to do this much more easily by providing interfaces that mirror functionality that you would normally need to use a MonoBehaviour for.

For example, if you have code that needs to run per frame, then you can implement the ITickable interface:

```csharp
public class Ship : ITickable
{
    public void Tick()
    {
        // Perform per frame tasks
    }
}
```

Then it's just a matter of including the following in one of your installers:

```csharp
Container.Bind<ITickable>().ToSingle<Ship>();
```

Note that the order that Tick() is called on all ITickables is also configurable, as outlined <a href="#update--initialization-order">here</a>.

## <a id="iinitializable-and-postinject"></a>IInitializable and PostInject

If you have some initialization that needs to occur on a given object, you can include this code in the constructor.  However, this means that the initialization logic would occur in the middle of the object graph being constructed, so it may not be ideal.

One alternative is to implement IInitializable, and then perform initialization logic in an Initialize() method.  This method would be called immediately after the entire object graph is constructed.  This is also nice because the initialization order is customizable in a similar way to ITickable, as explained <a href="#update--initialization-order">here</a>.

```csharp
public class Ship : IInitializable
{
    public void Initialize()
    {
        // Initialize your object here
    }
}
```

IInitializable works well for start-up initialization, but what about for objects that are created dynamically via factories?  (see <a href="#creating-objects-dynamically">this section</a> for what I'm referring to here).  For these cases you will most likely want to use a [PostInject] method:

```csharp
public class Foo
{
    [Inject]
    IBar _bar;

    [PostInject]
    public void Initialize()
    {
        ...
        _bar.DoStuff();
        ...
    }
}
```

## <a id="implementing-idisposable"></a>Implementing IDisposable

If you have external resources that you want to clean up when the app closes, the scene changes, or for whatever reason the composition root object is destroyed, you can do the following:

```csharp
public class Logger : IInitializable, IDisposable
{
    FileStream _outStream;

    public void Initialize()
    {
        _outStream = File.Open("log.txt", FileMode.Open);
    }

    public void Log(string msg)
    {
        _outStream.WriteLine(msg);
    }

    public void Dispose()
    {
        _outStream.Close();
    }
}
```

Then in your installer you can include:

```csharp
Container.Bind<Logger>().ToSingle();
Container.Bind<IInitializable>().ToSingle<Logger>();
Container.Bind<IDisposable>().ToSingle<Logger>();
```

Or you can use the following shortcut:

```csharp
Container.Bind<Logger>().ToSingle();
Container.BindAllInterfacesToSingle<Logger>();
```

This works because when the scene changes or your unity application is closed, the unity event OnDestroy() is called on all MonoBehaviours, including the CompositionRoot class, which then triggers all objects that are bound to IDisposable

Note that this example may or may not be a good idea (for example, the file will be left open if your app crashes), but illustrates the point  :)

## <a id="installers"></a>Installers

Often, there is some collections of related bindings for each sub-system and so it makes sense to group those bindings into a re-usable object.  In Zenject this re-usable object is called an Installer.  You can define a new installer as follows:

```csharp
public class FooInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ITickable>().ToSingle<Foo>();
        Container.Bind<IInitializable>().ToSingle<Foo>();
    }
}
```

You add bindings by overriding the InstallBindings method, which is called by the CompositionRoot when your scene starts up.  MonoInstaller is a MonoBehaviour so you can add FooInstaller by attaching it to a GameObject.  Since it is a GameObject you can also add public members to it to configure your installer from the Unity inspector, to add references within the scene, references to assets, or simply tuning data.

Note that in order for your installer to be triggered it must be attached to the Installers property of the CompositionRoot object.  This is necessary to be able to control the order that installers are called in (which you can do by dragging rows around in the Installers property).  The order should not usually matter (since nothing is instantiated during the install process) however it can matter in some cases, such as when you configure an Installer from an existing installer (eg: `Container.BindInstance("mysetting").WhenInjectedInto<MyOtherInstaller>()`).

In many cases you want to have your installer derive from MonoInstaller, so that you can have inspector settings.  There is also another base class called simply `Installer` which you can use in cases where you do not need it to be a MonoBehaviour.

You can also call installers from an existing installer.  For example:

```csharp
public class BarInstaller : Installer
{
    public override void InstallBindings()
    {
        ...
    }
}

public class FooInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Install<BarInstaller>();
    }
}
```

Note that in this case BarInstaller is of type Installer and not MonoInstaller, which is why we can simply call `Container.Install<BarInstaller>`.  By using Installer for BarInstaller instead of MonoInstaller, we don't need an instance of BarInstaller in our scene to use it.  Any calls to Container.Install will immediately instantiate the given installer type and then call InstallBindings on it.  This will repeat for any installers that this installer installs.

One of the main reasons we use installers as opposed to just having all our bindings declared all at once for each scene, is to make them re-usable.  This is not a problem for installers of type `Installer` because you can simply call `Container.Install` as described above for every scene you wish to use it in, but then how would we re-use a MonoInstaller in multiple scenes?

There are two ways to do this.

1. Prefabs.  After attaching your MonoInstaller to a gameobject in your scene, you can then create a prefab out of it.  This is nice because it allows you to share any configuration that you've done in the inspector on the MonoInstaller across scenes (and also have per-scene overrides if you want).

2. `Container.Install<>`.  You can also call Container.Install just as we did with BarInstaller above for MonoInstallers.  However, unlike with installers of type `Installer`, Zenject cannot simply create a new instance and then install that, because the MonoInstaller could have inspector settings on it.  To address this, when Container.Install is called with a MonoInstaller, Zenject will follow a naming convention to find the prefab for the MonoInstaller.  For example:

```csharp
// Note that this is a MonoInstaller and has inspector settings
public class QuxInstaller : MonoInstaller
{
    public string MyConfigurationSetting;

    public override void InstallBindings()
    {
        ...
    }
}

public class FooInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // When this is called, Zenject will look for a prefab at Resources/Installers/QuxInstaller.prefab and load that
        Container.Install<QuxInstaller>();
    }
}
```

As mentioned in the above code, Zenject will search for a prefab named QuxInstaller.prefab in all the Resources/Installer directories in your project.  This is sometimes a useful alternative to adding installer prefabs to every scene because it allows you to keep the objects in your scenes extremely light.

## <a id="zenject-order-of-operations"></a>Zenject Order Of Operations

A Zenject driven application is executed by the following steps:

1. Unity Awake() phase begins
1. CompositionRoot.Awake() method is called.  NOTE: This should always be the first thing executed in your scene.  By default this should work this way out of the box, because the executionOrder property is set to -9999 for the CompositionRoot classes.  You can also verify that this is the case by selecting `Edit -> Project Settings -> Script Execution Order` and making sure that GlobalCompositionRoot and SceneCompositionRoot are at the top.  (GlobalCompositionRoot goes first because global bindings should alway get updated before scene bindings)
1. Composition Root creates a new DiContainer object to be used to contain all instances used in the scene
1. Composition Root iterates through all the Installers that have been added to it via the Unity Inspector, and updates them to point to the new DiContainer.  It then calls InstallBindings() on each installer.
1. Each Installer then registers different sets of dependencies directly on to the given DiContainer by calling one of the Bind<> methods.  Note that the order that this binding occurs should not generally matter. Each installer may also include other installers by calling Container.Install<>. Each installer can also add bindings to configure other installers, however note that in this case order might actually matter, since you will have to make sure that code configuring other installers is executed before the installers that you are configuring! You can control the order by simply re-ordering the Installers property of the CompositionRoot
1. The Composition Root then injects all MonoBehaviours that are in the scene with their dependencies. Note that since MonoBehaviours are instantiated by Unity we cannot use constructor injection in this case and therefore [PostInject] injection, field injection or property injection must be used instead.
1. After filling in the scene dependencies the Composition Root then executes a single resolve for 'IFacade'.  This class represents the root of the object graph.  The Facade class by default has dependencies on TickableManager, InitializableManager, and DisposableManager classes, and therefore Zenject constructs instances of those as well before creating the main dependency root.  Those classes contains dependencies for lists of ITickable, IInitializable, and IDisposables.  So once again Zenject resolves all instances bound to any of these interfaces before constructing the manager classes.  This is important to know because it is why when you bind something to ITickable/IInitializable/IDisposable, it is always created at startup.
1. If any required dependencies cannot be resolved, a ZenjectResolveException is thrown
1. All other MonoBehaviour's in your scene has their Awake() method called
1. Unity Start() phase begins
1. CompositionRoot.Start() method is called.  This will trigger the Initialize() method on all IInitializable objects in the order specified in the installers.  The execution order mentioned above should guarantee that IInitializable.Initialize() occurs before any Start() method in your scene
1. All other MonoBehaviour's in your scene has their Start() method called
1. Unity Update() phase begins
1. Facade.Update() is called, which results in Tick() being called for all ITickable objects (in the order specified in the installers)
1. All other MonoBehaviour's in your scene has their Update() method called
1. Steps 13 - 15 is repeated for LateUpdate
1. At the same time, Steps 13 - 15 is repeated for FixedUpdate according to the physics timestep
1. App is exited
1. Dispose() is called on all objects mapped to IDisposable (see <a href="#implementing-idisposable">here</a> for details)

## <a id="di-guidelines--recommendations"></a>DI Guidelines / Recommendations / Gotchas / Tips and Tricks

* **Do not use GameObject.Instantiate if you want your objects to have their dependencies injected**
    * If you want to create a prefab yourself, you can use either IInstantiator interface (which is automatically included in every container) or the DiContainer directly, which will automatically fill in any fields that are marked with the [Inject] attribute.  The IInstantiator interface and DiContainer contain various methods to create from prefabs or create from empty game objects, etc.  
    **In other words:  If you want your dynamically created game object to have its fields injected, do not use GameObject.Instantiate and use DiContainer.InstantiatePrefab or GameObjectFactory instead**
    * For more information on GameObjectFactory see <a href="#game-object-factories">this section</a>

* **The container should *only* be referenced in the composition root "layer"**.
    * Note that factories are part of this layer and the container can be referenced there (which is necessary to create objects at runtime).  For example, see ShipStateFactory in the sample project.  See <a href="#creating-objects-dynamically">here</a> for more details on this.

* **Do not use IInitializable, ITickable and IDisposable for dynamically created objects**
    * Objects that are of type IInitializable are only initialized once, at startup.  If you create an object through a factory, and it derives from IInitializable, the Initialize() method will not be called.  You should use [PostInject] in this case.
    * The same applies to ITickable and IDisposable.  Deriving from these will do nothing unless they are part of the original object graph created at startup
    * If you have dynamically created objects that have an Update() method, it is usually best to call Update() on those manually, and often there is a higher level manager-like class in which it makes to do this from.  If however you prefer to use ITickable for dynamically objects you can declare a dependency to TickableManager and add/remove it explicitly as well.

* **Using multiple constructors**
    * Zenject does not support injecting into multiple constructors currently.  You can have multiple constructors however you must mark one of them with the [Inject] attribute so Zenject knows which one to use.

* **Injecting into MonoBehaviours**
    * One issue that often arises when using Zenject is that a game object is instantiated dynamically, and then one of the monobehaviours on that game object attempts to use one of its injected field dependencies in its Start() or Awake() methods.  Often in these cases the dependency will still be null, as if it was never injected.  The issue here is that Zenject cannot fill in the dependencies until after the call to GameObject.Instantiate completes, and in most cases GameObject.Instantiate will call the Start() and Awake() methods.  The solution is to use neither Start() or Awake() and instead define a new method and mark it with a [PostInject] attribute.  This will guarantee that all dependencies have been resolved before executing the method.

* **Using Zenject outside of Unity**
    * Zenject is primarily designed to work within Unity3D.  However, it can also be used as a general purpose DI framework outside of Unity3D.  In order to do this, you just have to build Zenject with the define ZEN_NOT_UNITY3D enabled.  Note also that if multi-threading support is needed (for eg. if used with ASP.NET MVC) then you will also have to define ZEN_MULTITHREADING

* **Lazily instantiated objects and the object graph**
    * Zenject will only instantiate any objects that are referenced in the object graph that is generated based on the bindings that you've invoked in your installer.  Internally, how it works is that Zenject has one single class that represents the root of the entire object graph (aka IDependencyRoot).  For unity projects this is typically the 'UnityDependencyRoot' class.  This class has a dependency on all ITickable, IInitializable, and IDisposable objects.  This is important to understand because it means that any class that you bind to ITickable, IInitializable, or IDisposable will always be created as part of the initial object graph of your application.  And only otherwise will your class be lazily instantiated when referenced by another class.

* **The order that things occur in is wrong, like injection is occurring too late, or Initialize() event is not called at the right time, etc.**
    * It may be because the execution order of the Zenject classes 'SceneCompositionRoot' and 'GlobalCompositionRoot' may be incorrect.  These classes should always have the earliest or near earliest execution order.  This should already be set by default (since this settings is in the SceneCompositionRoot.cs.meta and GlobalCompositionRoot.cs.meta files). However, if you are compiling Zenject yourself or have a unique configuration you may want to make sure, which you can do by going to "Edit -> Project Settings -> Script Execution Order" and confirming that GlobalCompositionRoot and SceneCompositionRoot are at the top, before the default time.

Please feel free to submit any other sources of confusion to sfvermeulen@gmail.com and I will add it here.

## <a id="global-bindings"></a>Global Bindings

This all works great for each individual scene, but what if you have dependencies that you wish to persist permanently across all scenes?  In Zenject you can do this by adding installers to a global container.

To do this, first add a global installers asset to your project directory then add installers to it.  Create or find a folder named Resources in your project tab, then right click and select Create -> Zenject -> Global Installers Asset.  You should then see a new asset in the selected Resources folder called 'ZenjectGlobalInstallers'.

If you click on this it will display a property for the list of Installers in the same way that it does for the composition root object that is placed in each scene.  The only difference is that the installers you add here must exist in the project as prefabs and cannot exist in any specific scene.  You can then directly reference those prefabs by dragging them into the Installers property of the global installers asset.

Note that you can add multiple Global Installers asset's in different resource folders and they will all be installed.

When you start any scene that contains a Scene Composition Root, this will cause every Global Installers Asset in your project to be installed.  This will always occur before any scene specific installers are called.  Note also that this only occurs once.  If you load another scene from the first scene, your global installers will not be called again and the bindings that it added previously will persist into the new scene.  You can declare ITickable / IInitializable / IDisposable objects in your global installers in the same way you do for your scene installers with the result being IInitializable.Initialize is called only once across each play session and IDisposable.Dispose is only called once the application is fully stopped.

This works because the container defined for each scene is nested inside the global container that your global installers bind into.  For more information on nested containers see <a href="#sub-containers-and-facades">here</a>.

## <a id="update--initialization-order"></a>Update / Initialization Order

In many cases, especially for small projects, the order that classes update or initialize in does not matter.  However, in larger projects update or initialization order can become an issue.  This can especially be an issue in Unity, since it is often difficult to predict in what order the Start(), Awake(), or Update() methods will be called in.  Unfortunately, Unity does not have an easy way to control this (besides in Edit -> Project Settings -> Script Execution Order, though that is pretty awkward to use)

In Zenject, by default, ITickables and IInitializables are updated in the order that they are added, however for cases where the update or initialization order matters, there is a much better way:  By specifying their priorities explicitly in the installer.  For example, in the sample project you can find this code in the scene installer:

```csharp
public class AsteroidsInstaller : MonoInstaller
{
    ...

    // We don't need to include these bindings but often its nice to have
    // control over initialization-order and update-order
    void InitExecutionOrder()
    {
        Container.Install<ExecutionOrderInstaller>(
            new List<Type>()
            {
                // Re-arrange this list to control update order
                // These classes will be initialized and updated in this order and disposed of in reverse order
                typeof(AsteroidManager),
                typeof(GameController),
            });
    }

    ...

    public override void InstallBindings()
    {
        ...
        InitExecutionOrder();
        ...
    }

}
```

This way, you won't hit a wall at the end of the project due to some unforeseen order-dependency.

You can also assign priorities one class at a time using the following helper method:

```csharp
ExecutionOrderInstaller.BindPriority<Foo>(Container, -100);
Container.Bind<IInitializable>().ToSingle<Foo>();
Container.Bind<ITickable>().ToSingle<Foo>();
Container.Bind<IInitializable>().ToSingle<Bar>();
```

Note that any ITickables, IInitializables, or IDisposable's that are not assigned a priority are automatically given the priority of zero.  This allows you to have classes with explicit priorities executed either before or after the unspecified classes.  For example, the above code would result in 'Foo.Initialize' being called before 'Bar.Initialize'.  If you instead gave it 100 instead of -100, it would be executed afterwards.

Note also that classes are disposed of in the opposite order.  This is similar to how Unity handles explicit script execution order.

## <a id="object-graph-validation"></a>Object Graph Validation

The usual workflow when setting up bindings using a DI framework is something like this:

* Add some number of bindings in code
* Execute your app
* Observe a bunch of DI related exceptions
* Modify your bindings to address problem
* Repeat

This works ok for small projects, but as the complexity of your project grows it is often a tedious process.  The problem gets worse if the startup time of your application is particularly bad, or when the resolve errors only occur from factories at various points at runtime.  What would be great is some tool to analyze your object graph and tell you exactly where all the missing bindings are, without requiring the cost of firing up your whole app.

You can do this in Zenject out-of-the-box by executing the menu item `Edit -> Zenject -> Validate Current Scene` or simply hitting CTRL+SHIFT+V with the scene open that you want to validate.  This will execute all installers for the current scene and construct a fully bound container (with null values for all instances).   It will then iterate through the object graphs and verify that all bindings can be found (without actually instantiating any of them).

Also, if you happen to be a fan of automated testing (as I am) then you can include calls to this menu item in unity batch mode as part of your testing suite.

## <a id="using-the-unity-inspector-to-configure-settings"></a>Using the Unity Inspector To Configure Settings

One implication of writing most of your code as normal C# classes instead of MonoBehaviour's is that you lose the ability to configure data on them using the inspector.  You can however still take advantage of this in Zenject by using the following pattern, as seen in the sample project:

```csharp
public class AsteroidsInstaller : MonoInstaller
{
    public Settings SceneSettings;

    public override void InstallBindings()
    {
        ...
        Container.BindInstance(SceneSettings.StateMoving);
        ...
    }

    [Serializable]
    public class Settings
    {
        ...
        public ShipStateMoving.Settings StateMoving;
        ...
    }
}
```

Note that if you follow this method, you will have to make sure to always include the [Serializable] attribute on your settings wrappers, otherwise they won't show up in the Unity inspector.

To see this in action, start the asteroids scene and try adjusting `Ship -> State Moving -> Move Speed` setting and watch live as your ship changes speed.

## <a id="creating-objects-dynamically"></a>Creating Objects Dynamically

One of the things that often confuses people new to dependency injection is the question of how to create new objects dynamically, after the app/game has fully started up and all the IInitializable objects have had their Initialize() method called.  For example, if you are writing a game in which you are spawning new enemies throughout the game, then you will want to construct a new object graph for the 'enemy' class.  How to do this?  The answer: Factories.

Remember that an important part of dependency injection is to reserve use of the container to strictly the "Composition Root Layer".  The container class (DiContainer) is included as a dependency in itself automatically so there is nothing stopping you from ignoring this rule and injecting the container into any classes that you want.  For example, the following code will work:

```csharp
public class Enemy
{
    DiContainer Container;

    public Enemy(DiContainer container)
    {
        Container = container;
    }

    public void Update()
    {
        ...
        var player = Container.Resolve<Player>();
        WalkTowards(player.Position);
        ...
        etc.
    }
}
```

HOWEVER, the above code is an example of an anti-pattern.  This will work, and you can use the container to get access to all other classes in your app, however if you do this you will not really be taking advantage of the power of dependency injection.  This is known, by the way, as [Service Locator Pattern](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/).

Of course, the dependency injection way of doing this would be the following:

```csharp
public class Enemy
{
    Player _player;

    public Enemy(Player player)
    {
        _player = player;
    }

    public void Update()
    {
        ...
        WalkTowards(_player.Position);
        ...
    }
}
```

The only exception to this rule is within factories and installers.  Again, factories and installers make up what we refer to as the "composition root layer".

For example, if you have a class responsible for spawning new enemies, before DI you might do something like this:

```csharp
public class EnemySpawner
{
    List<Enemy> _enemies = new List<Enemy>();

    public void Update()
    {
        if (ShouldSpawnNewEnemy())
        {
            var enemy = new Enemy();
            _enemies.Add(enemy);
        }
    }
}
```

This will not work however, since in our case the Enemy class requires a reference to the Player class in its constructor.  We could add a dependency to the Player class to the EnemySpawner class, but then we have the problem described <a href="#theory">above</a>.  The EnemySpawner class doesn't care about filling in the dependencies for the Enemy class.  All the EnemySpawner class cares about is getting a new Enemy instance.

The recommended way to do this in Zenject is the following:

```csharp
public class Enemy
{
    Player _player;

    public Enemy(Player player)
    {
        _player = player;
    }

    ...

    public class Factory : Factory<Enemy>
    {
    }
}

public class EnemySpawner
{
    Enemy.Factory _enemyFactory;
    List<Enemy> _enemies = new List<Enemy>();

    public EnemySpawner(Enemy.Factory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void Update()
    {
        if (ShouldSpawnNewEnemy())
        {
            var enemy = _enemyFactory.Create();
            _enemies.Add(enemy);
        }
    }
}
```

Then in your installer, you would include something like:

```csharp
Container.Bind<Enemy.Factory>().ToSingle();
Container.Bind<EnemySpawner>().ToSingle();
```

By using Enemy.Factory above, all the dependencies for the Enemy class (such as the Player) will be automatically filled in.

There is no requirement that the Enemy.Factory class be a nested class within Enemy, however we have found this to be a very useful convention.  Enemy.Factory is empty and simply derives from the built-in Zenject Factory<> class, which handles the work of using the DiContainer to construct a new instance of Enemy.

Also note that by using the built-in Zenject Factory<> class, the Enemy class will be automatically validated as well.  So if the constructor of the Enemy class includes a type that is missing a binding, this error can be caught before running your app, by simply running validation.  Validation can be especially useful for dynamically created objects, because otherwise you may not catch the error until the factory is invoked at some point during runtime.  See <a href="#object-graph-validation">this section</a> for more details on Validation.

However, in more complex examples, the EnemySpawner class may wish to pass in custom constructor arguments as well. For example, let's say we want to randomize the speed of each Enemy to add some interesting variation to our game.  Our enemy class becomes:

```csharp
public class Enemy
{
    Player _player;
    float _runSpeed;

    public Enemy(Player player, float runSpeed)
    {
        _player = player;
        _runSpeed = runSpeed;
    }

    public class Factory : Factory<float, Enemy>
    {
    }
}

public class EnemySpawner
{
    Enemy.Factory _enemyFactory;
    List<Enemy> _enemies = new List<Enemy>();

    public EnemySpawner(Enemy.Factory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void Update()
    {
        if (ShouldSpawnNewEnemy())
        {
            var newSpeed = Random.Range(MIN_ENEMY_SPEED, MAX_ENEMY_SPEED);
            var enemy = _enemyFactory.Create(newSpeed);
            _enemies.Add(enemy);
        }
    }
}
```

The dynamic parameters that are provided to the Enemy constructor are declared by using generic arguments to the Factory<> base class of Enemy.Factory.  This will add a method to Enemy.Factory that takes the parameters with the given types.

## <a id="game-object-factories"></a>Game Object Factories

You can also use the same approach as described <a href="#creating-objects-dynamically">above</a> to create factories that construct game objects.  For example:

```csharp
public class FooMonoBehaviour : MonoBehaviour
{
    ...

    public class Factory : GameObjectFactory<FooMonoBehaviour>
    {
    }
}
```

The only difference here is that this factory requires a prefab to be installed on it.  There is a convenience method that you can use to handle both installing the prefab and also declaring FooMonoBehaviour.Factory as a singleton:

```csharp
public override void InstallBindings()
{
    ...

    Container.BindGameObjectFactory<FooMonoBehaviour.Factory>(_prefab);

    ...
}
```

Now classes can simply declare a constructor parameter of type FooMonoBehaviour.Factory and by calling the Create() method, construct new instances of a given prefab.

## <a id="abstract-factories"></a>Abstract Factories

The above description of factories is great for most cases, however, there are times you do not want to depend directly on a concrete class and instead want your factory to return an interface instead.  This kind of factory is called an Abstract Factory, and it works a bit differently in Zenject from the standard factory as described above.

Let's create an example scenario, where we have multiple different implementations of a given interface:

```csharp

public interface IPathFindingStrategy
{
    ...
}

public class AStarPathFindingStrategy : IPathFindingStrategy
{
    ...
}

public class RandomPathFindingStrategy : IPathFindingStrategy
{
    ...
}

public class GameController
{
    IFactory<IPathFindingStrategy> _strategyFactory;
    IPathFindingStrategy _pathFindingStrategy;

    public GameController(IFactory<IPathFindingStrategy> strategyFactory)
    {
        _strategyFactory = strategyFactory;
    }

    public void InitPathFinding()
    {
        _pathFindingStrategy = _strategyFactory.Create();

        ...
    }
}

```

For the sake of this example, let's also assume that we have to create the instance of IPathFindingStrategy at runtime.  Otherwise it would be as simple as executing `Container.Bind<IPathFindingStrategy>().ToSingle<TheImplementationWeWant>();` in one of our installers.

So instead, what we want to create is a dependency of type `IFactory<IPathFindingStrategy>` that creates an instance of the implementation that we want to use when its Create() method is called, for use in the `GameController` class above.

Let's say we want to use the `AStarPathFindingStrategy` implementation. The installer here looks like this:

```csharp

public class GameInstaller : Installer
{
    public void InstallBindings()
    {
        ...

        Container.BindIFactory<IPathFindingStrategy>().ToFactory<AStarPathFindingStrategy>();

        ...
    }
}

```

Because of the fact that there are a number of different ways to define what is returned by a given `IFactory<>`, it has it's own bind method called `BindIFactory<>`.  This method takes one generic parameter that defines the generic parameter that you want to use for `IFactory<>`, and then provides a number of different methods that you can choose from to define what is returned by the `IFactory<>` that you're creating.

In the example installer above, our bind command maps our abstract factory `IFactory<IPathFindingStrategy>` to a concrete factory that returns a new instance of type `AStarPathFindingStrategy`.

Note that you can also pass runtime parameters by supplying extra generic arguments to the `BindIFactory<>` method.  For example, if our implementations of `IPathFindingStrategy` both required a constructor parameter of type `string`, our binding would look like this instead:

```csharp
Container.BindIFactory<string, IPathFindingStrategy>().ToFactory<AStarPathFindingStrategy>();
```

This would result in a dependency of type `IFactory<string, IPathFindingStrategy>` with a Create() method that takes a string parameter.

Included below are the other methods that you can choose from when using the `BindIFactory<>` method:

1 - **ToMethod([method])** - Create dynamic dependency from a method

Results in a dependency of type `IFactory<TContract>` that invokes the given method.  Method must return a new instance of type `TContract`.

```csharp
// Examples
Container.BindIFactory<IFoo>().ToMethod(MyMethod);

// Using a lambda:
Container.BindIFactory<IFoo>().ToMethod(c => new Foo());

// With a parameter:
Container.BindIFactory<string, IFoo>().ToMethod((text, c) => new Foo(text));
```

2 - **ToInstance<TContract>** - Create dynamic dependency from an existing instance

Results in a dependency of type `IFactory<TContract>` that just returns the given instance

```csharp
// Example:
Container.BindIFactory<IFoo>().ToInstance(new Foo());
```

3 - **ToFactory** - Create dynamic dependency for a concrete type

Results in a dependency of type `IFactory<TContract>` that will create a new instance of type `TContract`.  `TContract` must be a concrete class in this case.

```csharp
// Example:
Container.BindIFactory<Foo>().ToFactory();

// With some parameters:
Container.BindIFactory<string, int, int, Foo>().ToFactory();
```

In this example, any calls to `IFactory<Foo>.Create()` will return a new instance of type `Foo`.  Again, this of course requires that the generic argument to `BindIFactory<>` be non-abstract.

4 - **ToFactory&lt;TConcrete&gt;** - Create dynamic dependency for an abstract type

Results in a dependency of type `IFactory<TContract>` that will create a new instance of type `TConcrete`.  `TConcrete` must derive from `TContract` in this case.

```csharp
// Example:
Container.BindIFactory<IFoo>().ToFactory<Foo>();

// With some parameters
Container.BindIFactory<string, int, IFoo>().ToFactory<Foo>();
```

5 - **ToIFactory&lt;TConcrete&gt;** - Create dynamic dependency via lookup on another factory

Results in a dependency of type IFactory<TContract> that will return an instance of type TConcrete.  It does this by looking up IFactory<TConcrete> and calling Create() to create an instance of type TConcrete.  TConcrete must derive from TContract for this binding.  Also, it is assumed that IFactory<TConcrete> is declared in a separate binding.

```csharp
// First create a simple binding for IFactory<Foo>
Container.BindIFactory<Foo>().ToFactory();

// Now create a binding for IFactory<IFoo> that uses the above binding
Container.BindIFactory<IFoo>().ToIFactory<Foo>();
```

6 - **ToCustomFactory<TContract, TConcrete, TFactory>** - Create dynamic dependency using user created factory class

Results in a dependency of type IFactory<TContract> that will return an instance of type TConcrete using the given factory of type TFactory.  It is assumed that TFactory is declared in another binding.  TFactory must also derive from IFactory<TConcrete> for this to work

```csharp
// Map IFoo to our custom factory Foo.Factory
Container.BindIFactory<IFoo>().ToCustomFactory<Foo, MyCustomFooFactory>();

public class MyCustomFooFactory : IFactory<IFoo>
{
    ...
}
```

7 - **ToPrefab<TMonoBehaviour>(prefab)** - Create dynamic MonoBehaviour using given prefab

TMonoBehaviour = Derives from MonoBehaviour

Results in a dependency of type IFactory<TMonoBehaviour> that can be used to create instances of the given prefab.  After instantiating the given prefab, the factory will search it for a component of type 'TMonoBehaviour' and then return that from the Create() method

```csharp
Container.BindIFactory<IFoo>().ToPrefab<FooMonoBehaviour>(prefab);
```

## <a id="custom-factories"></a>Custom Factories

If you do not want to use the abstract factory method as described above, or if you want to have 100% control of how your object is instantiated, you could also create a custom factory.  For example:

```csharp
public enum Difficulties
{
    Easy,
    Hard,
}

public interface IEnemy
{
    ...
}

public class Demon : IEnemy
{
    ...
}

public class Dog : IEnemy
{
    ...
}

public class EnemyFactory
{
    DiContainer _container;
    Difficulties _difficulty;

    public EnemyFactory(DiContainer container, Difficulties difficulty)
    {
        _container = container;
        _difficulty = difficulty;
    }

    public IEnemy Create(float speed)
    {
        if (_difficulty == Difficulties.Hard)
        {
            return _container.Instantiate<Demon>(speed);
        }

        return _container.Instantiate<Dog>(speed);
    }
}
```

And then in our installer we would include:

```csharp
Container.Bind<EnemyFactory>().ToSingle();
Container.Bind<Difficulties>().ToInstance(Difficulties.Easy);
```

This way we can change the type of enemy we spawn by simply changing the difficulty bound in the installer.

Or, if you do not want your code to depend on `EnemyFactory` directly, you could change it to this instead:

```csharp
public class EnemyFactory : IFactory<IEnemy>
{
    ...
}

...

Container.BindIFactory<IEnemy>().ToCustomFactory<EnemyFactory>();
```

Note also that this is equivalent to the following:

```csharp
Container.Bind<IFactory<IEnemy>>().ToTransient<EnemyFactory>();
```

One issue with the above implementations is that they will not be validated properly.  Any constructor parameters added to the Dog or Demon classes that cannot be resolved will not be detected until runtime.  If you wish to address this you can implement it the following way:

```csharp
public class EnemyFactory : IValidatable
{
    DiContainer _container;
    Difficulties _difficulty;

    public EnemyFactory(DiContainer container, Difficulties difficulty)
    {
        _container = container;
        _difficulty = difficulty;
    }

    public IEnemy Create(float speed)
    {
        if (_difficulty == Difficulties.Hard)
        {
            return _container.Instantiate<Demon>(speed);
        }

        return _container.Instantiate<Dog>(speed);
    }

    public IEnumerable<ZenjectResolveException> Validate()
    {
        return _container.ValidateObjectGraph<Dog>(typeof(float))
            .Concat(_container.ValidateObjectGraph<Demon>(typeof(float)));
    }
}
```

This is optional but can be nice if you are fan of validation.  The parameters provided to the ValidateObjectGraph method indicate the dependencies that can be skipped for validation.  This should include any runtime parameters.

Note that we are injecting the DiContainer directly into the EnemyFactory class, which is generally a bad thing to do but ok in this case because it is a factory (and therefore part of the "composition root layer")

For another real world example on factories, see [this reddit thread](https://www.reddit.com/r/Zenject/comments/3brroe/questionsimple_ftg_with_mvc_pattern/)

## <a id="injecting-data-across-scenes"></a>Injecting data across scenes

In some cases it's useful to pass arguments from one scene to another.  The way Unity allows us to do this by default is fairly awkward.  Your options are to create a persistent GameObject and call DontDestroyOnLoad() to keep it alive when changing scenes, or use global static classes to temporarily store the data.

Let's pretend you want to specify a 'level' string to the next scene.  You have the following class that requires the input:

```csharp
public class LevelHandler : IInitializable
{
    readonly string _startLevel;

    public LevelHandler(
        [InjectOptional("StartLevelName")]
        string startLevel)
    {
        if (startLevel == null)
        {
            _startLevel = "default_level";
        }
        else
        {
            _startLevel = startLevel;
        }
    }

    public void Initialize()
    {
        ...
        [Load level]
        ...
    }
}
```

You can load the scene containing `LessonStandaloneStart` and specify a particular level by using the following syntax:

```csharp
ZenUtil.LoadScene("NameOfSceneToLoad",
    delegate (DiContainer container)
    {
        container.Bind<string>("StartLevelName").ToInstance("custom_level").WhenInjectedInto<LevelHandler>();
    });
```

Note that you can still run the scene directly, in which case it will default to using "level01".  This is possible because we are using the InjectOptional flag.

An alternative and arguably cleaner way to do this would be to customize the installer itself rather than the LevelHandler class.  In this case we can write our LevelHandler class like this (without the [InjectOptional] flag)

```csharp
public class LevelHandler : IInitializable
{
    readonly string _startLevel;

    public LevelHandler(string startLevel)
    {
        _startLevel = startLevel;
    }

    public void Initialize()
    {
        ...
        [Load level]
        ...
    }
}
```

Then, in the installer for our scene we can include the following:

```csharp
public class GameInstaller : Installer
{
    [InjectOptional]
    public string LevelName = "default_level";

    ...

    public override void InstallBindings()
    {
        ...
        Container.Bind<string>().ToInstance(LevelName).WhenInjectedInto<LevelHandler>();
        ...
    }
}
```

Then, instead of injecting directly into the LevelHandler we can inject into the installer instead.

```csharp
ZenUtil.LoadScene("NameOfSceneToLoad",
    delegate (DiContainer container)
    {
        container.Bind<string>().ToInstance("level02").WhenInjectedInto<GameInstaller>();
    });
```

Note that in this case I didn't need to use the "LevelName" identifier since there is only one string injected into the GameInstaller class.

Some people have also found it useful to separate out content into different scenes and then load each scene additively using the Unity method `Application.LoadLevelAdditive`.  In some cases it's useful to have the dependencies in the new scene resolved using the container of the original scene.  To achieve this, you can call `ZenUtil.LoadSceneAdditiveWithContainer` and pass in your scene's container.  Note however that it is assumed in this case that the new scene does not have its own container + Composition Root.

## <a id="scenes-decorator"></a>Scene Decorators

Scene Decorators can be used to add behaviour to another scene without actually changing the installers in that scene.  The usual way to achieve this is to use flags on MonoInstallers to conditionally add different bindings within the scene itself.  However the scene decorator approach can be cleaner sometimes because it doesn't involve changing the main scene.

For example, let's say we want to add some special keyboard shortcuts to our main production scene for testing purposes.  In order to do this using decorators, you can do the following:

* Create a new scene
* Add an empty GameObject and name it 'CompositionRoot'
* Add a 'SceneDecoratorCompositionRoot' MonoBehaviour to it
* Type in the scene you want to 'decorate' in the 'Scene Name' field of SceneDecoratorCompositionRoot
* Create a new C# script with the following contents, then add this MonoBehaviour to your scene as a gameObject, then drag it to the Installers property of SceneDecoratorCompositionRoot

```csharp
public class ExampleDecoratorInstaller : DecoratorInstaller
{
    public override void PostInstallBindings()
    {
        // Add bindings here that you want added AFTER installing the main scene

        Container.Bind<ITickable>().ToSingle<TestHotKeysAdder>();
    }

    public override void PreInstallBindings()
    {
        // Add bindings here that you want added BEFORE installing the main scene
    }
}

public class TestHotKeysAdder : ITickable
{
    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Hotkey triggered!");
        }
    }
}
```

If you run your scene it should now behave exactly like the scene you entered in 'Scene Name' except with the added functionality in your decorator installer.  NOTE: If the scene fails to load, it might be because the scene that you're decoratoring has not been added to the list of levels in build settings.

The PostInstallBindings method is useful when you want to override a binding in the main scene using 'Rebind'.  And PreInstallBindings is necessary if you want to inject data into the installers in the main scene. For a better example see the asteroids project that comes with Zenject (open 'AsteroidsDecoratorExample' scene).  NOTE:  If installing from asset store version, you need to add the 'Asteroids' scene to your build settings so that the scene decorator can find it.

Note also that Zenject validate (using CTRL+SHIFT+V or the menu item via Edit->Zenject->Validate Current Scene) also works with decorator scenes.

## <a id="advanced-factory-construction-using-subcontainers"></a>Advanced Factory Construction Using SubContainers

In the real world there can sometimes be complex construction that needs to occur in your custom factory classes.  One way to deal with this is to use a temporary sub-container.

For example, suppose one day we decide to add further runtime constructor arguments to the Enemy class:

```csharp
public class Enemy
{
    public Enemy(EnemyWeapon weapon)
    {
        ...
    }
}

public class EnemyWeapon
{
    public EnemyWeapon(float damage)
    {
        ...
    }
}
```

And let's say we want the damage of the EnemyWeapon class to be specified by the EnemySpawner class.  How do we pass that argument down to EnemyWeapon?  In this case it might be easiest to create the EnemyWeapon class first and then pass it to the factory.  However, for the sake of this example let's pretend we want to create the EnemyClass in one call to Instantiate

```csharp
public class EnemyFactory
{
    DiContainer _container;

    public EnemyFactory(DiContainer container)
    {
        _container = container;
    }

    public Enemy Create(float weaponDamage)
    {
        DiContainer subContainer = Container.CreateSubContainer();
        subContainer.BindInstance(weaponDamage).WhenInjectedInto<EnemyWeapon>();
        return subContainer.Instantiate<Enemy>();
    }
}
```

We can use Sub-Containers here to achieve this.  Sub-containers can be used in factories to add complex bindings to the object graph that you are instantiating.  The Container.Instantiate method does allow passing in a list of constructor arguments, but it is limited to just that.  Using Sub-Containers in this way can be thought of a more powerful version of that.  Though of course, SubContainers have a lot more uses than simply factory construction, as explained in the <a href="#sub-containers-and-facades">next section</a>

## <a id="sub-containers-and-facades"></a>Sub-Containers And Facades

In some cases is can be very useful to use multiple containers in the same application.  For example, if you are creating a word processor it may be useful to have a sub-container for each tab that represents a separate document.  This way, you could bind a bunch of classes `ToSingle()` within the sub-container and they could all easily reference each other as if they were all singletons.  Then you could instantiate multiple sub-containers to be used for each document, with each sub-container having unique instances of all the classes that handle each specific document.

Another example might be if you are designing an open-world space ship game, you might want each space ship to have it's own container that contains all the class instances responsible for running that specific spaceship.

This is actually how global bindings work.  There is one global container for the entire application, and when a unity scene starts up, it creates a new sub-container "underneath" the global container.  All the bindings that you add in your scene MonoInstaller are bound to your subcontainer.  This allows the dependencies in your scene to automatically get injected with global bindings, because sub-containers automatically inherit all the bindings in its parent (and grandparent, etc.).

A common design pattern that we like to use in relation to sub-containers is the <a href="https://en.wikipedia.org/wiki/Facade_pattern">Facade pattern</a>.  This pattern is used to abstract away a related group of dependencies so that it can be used at a more higher-level when used by other modules in the code base.  This is relevant here because often when you are defining sub-containers in your application it is very useful to also define a Facade class that is used to interact with this sub-container as a whole.  So, to apply it to the spaceship example above, you might have a SpaceshipFacade class that represents very high-level operations on a spaceship such as "Start Engine", "Take Damage", "Fly to destination", etc.  And then internally, the SpaceshipFacade class can delegate the specific handling of all the parts of these requests to the relevant single-responsibility dependencies that exist within the sub-container.  Let's see what the code would look like in this example (read the comments for an explanation)

```csharp
// First we define our facade class to represent all the classes associated with a ship
// Facade is a class that is built into zenject
public class ShipFacade : Facade
{
    public class Factory : FacadeFactory<ShipFacade>
    {
    }
}

// Now we define a few classes that will be installed into our ship sub-container
// This class will just move the ship in a specific direction
public class ShipMoveHandler : ITickable
{
    readonly Settings _settings;

    ShipView _view;
    Vector3 _direction = Vector3.forward;

    public ShipMoveHandler(
        ShipView view,
        Settings settings)
    {
        _settings = settings;
        _view = view;
    }

    public void Tick()
    {
        _view.transform.position += _direction * _settings.Speed * Time.deltaTime;
    }

    [Serializable]
    public class Settings
    {
        public float Speed;
    }
}

// We define an empty monobehaviour that will allow use to manipulate the game object
// that contains the ship mesh, animations, etc.
// This would normally contain references to all the different parts of the model that we're
// interested in
// It is attached to the prefab that we install below in GameInstaller
public class ShipView : MonoBehaviour
{
}

// We also need to define a class that will contain our ship facade
public class GameController : IInitializable, ITickable
{
    readonly ShipFacade.Factory _shipFactory;
    ShipFacade _ship;

    public GameController(ShipFacade.Factory shipFactory)
    {
        _shipFactory = shipFactory;
    }

    public void Initialize()
    {
        // Note that all the IInitializable's will automatically have their Initialize
        // methods call here so there is no need to do it ourselves
        _ship = _shipFactory.Create();
    }

    public void Tick()
    {
        // However, the facade class must have its Tick() called manually
        // in order to have any ITickable's in the container get their Tick() methods called
        _ship.Tick();
    }
}

// Finally, we install everything!
public class GameInstaller : MonoInstaller
{
    [SerializeField]
    Settings _settings;

    public override void InstallBindings()
    {
        // Add our main game class
        Container.Bind<IInitializable>().ToSingle<GameController>();
        Container.Bind<ITickable>().ToSingle<GameController>();
        Container.Bind<GameController>().ToSingle();

        // This will result in a binding of type ShipFacade.Factory
        // Note that you can add conditions to this just like for other bindings
        Container.BindFacadeFactory<ShipFacade, ShipFacade.Factory>(InstallShipFacade);

        // This could be either bound in the subcontainer or in the main container, it doesn't matter
        Container.BindInstance(_settings.ShipMoveHandler);
    }

    // This method will be called to initialize the container every time a new ship is created
    void InstallShipFacade(DiContainer subContainer)
    {
        // Note here that we are using subContainer and NOT Container
        subContainer.Bind<ITickable>().ToSingle<ShipMoveHandler>();
        subContainer.Bind<ShipView>().ToSinglePrefab(_settings.ShipPrefab);
    }

    [Serializable]
    public class Settings
    {
        public GameObject ShipPrefab;
        public ShipMoveHandler.Settings ShipMoveHandler;
    }
}
```

By doing the above we can now easily create multiple ship facade's and this will result in new instances of ship-handling classes such as `ShipMoveHandler` being created for each individual ship.  And within the ship subcontainer, all these classes can treat each other as singletons.  This is very nice, because just like in other examples above, when writing new classes to handle the ship, we don't need to think about where our dependencies come from, we just need to worry about fulfilling our own single responsibilities and then ask for whatever other dependencies we need in our constructor.

Finally, we can also operate on each individual ship from the main game code by interacting with the ShipFacade, which abstracts away the low level functionality from within the ship.

This is a simple example.  In the real-world, almost all facades will take some parameters.  This can be done in a similar way to how parameters are added to the other kinds of factories.   For example, let's say we want to construct the "ShipView" class outside of the facade and then pass that into it.  To make things a bit more interesting, let's also add a high-level method on the ShipFacade.

The code would then just need to be changed to the following:

```csharp
public class ShipFacade : Facade
{
    ShipMoveHandler _moveHandler;

    // Note that the facade class is itself created from inside the subcontainer
    // and therefore can add dependencies for any of its contents
    public ShipFacade(ShipMoveHandler moveHandler)
    {
        _moveHandler = moveHandler;
    }

    public void StartEngine()
    {
        // Delegate to the engine handler
        _moveHandler.StartEngine();
    }

    // Add any parameters that we want to take from outside to the list here
    public class Factory : FacadeFactory<ShipView, ShipFacade>
    {
    }
}

public class ShipView : MonoBehaviour
{
    public class Factory : GameObjectFactory<ShipView>
    {
    }
}

public class GameController : IInitializable, ITickable
{
    readonly ShipFacade.Factory _shipFactory;
    readonly ShipView.Factory _shipViewFactory;
    ShipFacade _ship;

    public GameController(ShipFacade.Factory shipFactory, ShipView.Factory shipViewFactory)
    {
        _shipFactory = shipFactory;
        _shipViewFactory = shipViewFactory;
    }

    public void Initialize()
    {
        var shipView = _shipViewFactory.Create();

        ...

        _ship = _shipFactory.Create(shipView);

        ....

        _ship.StartEngine();
    }

    public void Tick()
    {
        _ship.Tick();
    }
}

public class GameInstaller : MonoInstaller
{
    [SerializeField]
    Settings _settings;

    public override void InstallBindings()
    {
        Container.Bind<IInitializable>().ToSingle<GameController>();
        Container.Bind<ITickable>().ToSingle<GameController>();
        Container.Bind<GameController>().ToSingle();

        Container.BindGameObjectFactory<ShipView.Factory>(_settings.ShipPrefab);
        Container.BindFacadeFactory<ShipView, ShipFacade, ShipFacade.Factory>(InstallShipFacade);

        Container.BindInstance(_settings.ShipMoveHandler);
    }

    // Any parameters passed to the factory are then forwarded to this method
    // This allows us to bind the parameters however we want
    // This also allows us to conditionally add different bindings depending on the parameter values
    void InstallShipFacade(DiContainer subContainer, ShipView view)
    {
        subContainer.BindInstance(view);
        subContainer.Bind<ITickable>().ToSingle<ShipMoveHandler>();
    }

    [Serializable]
    public class Settings
    {
        public GameObject ShipPrefab;
        public ShipMoveHandler.Settings ShipMoveHandler;
    }
}
```

## <a id="commands-and-signals"></a>Commands And Signals

Zenject also includes an optional extension that allows you to define "Commands" and "Signals".

A signal can be thought of as a single event, that when triggered will notify a number of listeners.  A command is an object with a single method named `Execute`, that will forward the request to a specific handler.

The advantage of using Signals and Commands is that the result will often be more loosely coupled code.  Given two classes A and B that need to communicate, your options are usually:

1. Directly call a method on B from A.  In this case, A is strongly coupled with B.
2. Inverse the dependency by having B observe an event on A.  In this case, B is strongly coupled with A.

Both cases result in the classes being coupled in some way.  Now if instead you create a command object, which is called by A and which invokes a method on B, then the result is less coupling.  Granted, A is still coupled to the command class, but in some cases that is better than being directly coupled to B.  Using signals works similarly, in that you can remove the coupling by having A trigger a signal, which is observed by B.

Signals are defined like this:

```csharp
public class GameLoadedSignal : Signal
{
    public class Trigger : TriggerBase { }
}

public class GameLoadedSignalWithParameter : Signal<string>
{
    public class Trigger : TriggerBase { }
}
```

The trigger class is used to invoke the signal event.  Note that the Signal base class is defined within the Zenject.Commands namespace.

Signals are declared in an installer like this:

```csharp

public override void InstallBindings()
{
    ...
    Container.BindSignalTrigger<GameLoadedSignal.Trigger, GameLoadedSignal>().WhenInjectedInto<Foo>();
    ...
    Container.BindSignalTrigger<GameLoadedSignalWithParameter.Trigger, GameLoadedSignalWithParameter, string>().WhenInjectedInto<Foo>();
}

```

This statement will do the following:
* Bind the class `GameLoadedSignal` as `ToSingle<>` without a condition.  This means that any class can declare `GameLoadedSignal` as a dependency.
* Bind the class `GameLoadedSignal.Trigger` as `ToSingle<>` as well, except it will limit its usage strictly to class `Foo`.

Commands are defined like this

```csharp
public class ResetSceneCommand : Command { }

public class ResetSceneCommandWithParameter : Command<string> { }
```

Note again that the Command base class is defined within the Zenject.Commands namespace here.

Unlike with signals, there are several different ways of declaring a command in an installer.  Perhaps the simplest way would be the following:

```csharp
public override void InstallBindings()
{
    ...
    Container.BindCommand<ResetSceneCommand>().HandleWithSingle<ResetSceneHandler>();
    ...
    Container.BindCommand<ResetSceneCommandWithParameter, string>().HandleWithSingle<ResetSceneHandler>();
    ...
}

public class ResetSceneHandler : ICommandHandler
{
    public void Execute()
    {
        ... [reset scene] ...
    }
}
```

This bind statement will result in an object of type `ResetSceneCommand` being added to the container.  Any time a class calls Execute on `ResetSceneCommand`, it will trigger the Execute method on the `ResetSceneHandler` class as well.  For example:

```csharp
public class Foo : ITickable
{
    readonly ResetSceneCommand _command;

    public Foo(ResetSceneCommand command)
    {
        _command = command;
    }

    public void Tick()
    {
        ...
        _command.Execute();
        ...
    }
}
```

We might also want to restrict usage of our command to the Foo class only, which we could do with the following

```csharp
public override void InstallBindings()
{
    ...
    Container.BindCommand<ResetSceneCommand>().HandleWithSingle<ResetSceneHandler>().WhenInjectedInto<Foo>();
    ...
}
```

Note that in this case we are using `HandleWithSingle<>` - this means that the same instance of `ResetSceneHandler` will be used every time the command is executed.  Alternatively, you could declare it using `HandleWithTransient<>` which would instantiate a new instance of `ResetSceneHandler` every time Execute() is called.  For example:

```csharp
Container.BindCommand<ResetSceneCommand>().HandleWithTransient<ResetSceneHandler>();
```

This might be useful if the `ResetSceneCommand` class involves some long-running operations that require unique sets of member variables/dependencies.

You can also bind commands directly to methods instead of classes by doing the following:

```csharp
public override void InstallBindings()
{
    ...
    Container.BindCommand<ResetSceneCommand>().HandleWithSingle<MyOtherHandler>(x => x.ResetScene);
    ...
}

public class ResetSceneHandler
{
    public void ResetScene()
    {
        ... [reset scene] ...
    }
}
```

This approach does not require that you derive from `ICommandHandler` at all.  There is also a `HandleWithTransient` version of this which works similarly (instantiates a new instance of MyOtherHandler).

```csharp
Container.BindCommand<ResetSceneCommand>().HandleWithTransient<MyOtherHandler>(x => x.ResetScene);
```

## <a id="auto-mocking-using-moq"></a>Auto-Mocking using Moq

One of the really cool features of DI is the fact that it makes testing code much, much easier.  This is because you can easily substitute one dependency for another by using a different Composition Root.  For example, if you only want to test a particular class (let's call it Foo) and don't care about testing its dependencies, you might write 'mocks' for them so that you can isolate Foo specifically.

```csharp
public class Foo
{
    IWebServer _webServer;

    public Foo(IWebServer webServer)
    {
        _webServer = webServer;
    }

    public void Initialize()
    {
        ...
        var x = _webServer.GetSomething();
        ...
    }
}
```

In this example, we have a class Foo that interacts with a web server to retrieve content.  This would normally be very difficult to test for the following reasons:

* You would have to set up an environment where it can properly connect to a web server (configuring ports, urls, etc.)
* Running the test could be slower and limit how much testing you can do
* The web server itself could contain bugs so you couldn't with certainty isolate Foo as the problematic part of the test
* You can't easily configure the values returned from the web server to test sending various inputs to the Foo class

However, if we create a mock class for IWebServer then we can address all these problems:

```csharp
public class MockWebServer : IWebServer
{
    ...
}
```

Then hook it up in our installer:

```csharp
Container.Bind<IWebServer>().ToSingle<MockWebServer>();
```

Then you can implement the fields of the IWebServer interface and configure them based on what you want to test on Foo. Hopefully You can see how this can make life when writing tests much easier.

Zenject also allows you to even avoid having to write the MockWebServer class in favour of using a library called "Moq" which does all the work for you.

Note that by default, Auto-mocking is not enabled in Zenject.  If you wish to use the auto-mocking feature then you need to go to your Zenject install directory and extract the contents of "Extras/ZenjectAutoMocking.zip".  Note also that AutoMocking is incompatible with webplayer builds, and you will also need to change your "Api Compatibility Level" from ".NET 2.0 Subset" to ".NET 2.0" (you can find this in PC build settings)

After extracting the auto mocking package it is just a matter of using the following syntax to mock out various parts of your project:

```csharp
Container.Bind<IFoo>().ToMock();
```

However, this approach will not allow you to take advantage of the advanced features of Moq.  For more advanced usages, see the documentation for Moq

## <a id="questions"></a>Frequently Asked Questions

* **<a id="aot-support"></a>Does this work on AOT platforms such as iOS and WebGL?**

    Yes.  However, there are a few things that you should be aware of for WebGL.  One of the things that Unity's IL2CPP compiler does is strip out any code that is not used.  It calculates what code is used by statically analyzing the code to find usage.  This is great, except that this will miss any methods/types that are not used explicitly.  In particular, any classes that are created solely through Zenject will have their constructors ignored by the IL2CPP compiler.  In order to address this, the [Inject] attribute that is sometimes applied to constructors also serves to automatically mark the constructor to IL2CPP to not strip out.   In other words, to fix this issue all you have to do is mark every constructor that you create through Zenject with an [Inject] attribute when compiling for WebGL.

* **<a id="faq-performance"></a>How is performance?**

    DI can affect start-up time when it builds the initial object graph. However it can also affect performance any time you instantiate new objects at run time.

    Zenject uses C# reflection which is typically slow, but in Zenject this work is cached so any performance hits only occur once for each class type.  In other words, Zenject avoids costly reflection operations by making a trade-off between performance and memory to ensure good performance.

    For some benchmarks on Zenject versus other DI frameworks, see [here](https://github.com/svermeulen/IocPerformance/tree/Zenject).

    Zenject should also produce zero per-frame heap allocations.

* **<a id="net-framework"></a>Can I use .NET framework 4.0 and above?**

    By default Unity uses .NET framework 3.5 and so Zenject assumes that this is what you want.  If you are compiling Zenject with a version greater than this, this is fine, but you'll have to either delete or comment out the contents of Func.cs.

## <a id="cheatsheet"></a>Installers Cheat-Sheet

Below are a bunch of randomly assorted examples of bindings that you might include in one of your installers.

```csharp

///////////// ToTransient

// Create a new instance of Foo for every class that asks for it
Container.Bind<Foo>().ToTransient();

// Create a new instance of Foo for every class that asks for an IFoo
Container.Bind<IFoo>().ToTransient<Foo>();

// Non generic versions
Container.Bind(typeof(IFoo)).ToTransient();
Container.Bind(typeof(IFoo)).ToTransient(typeof(Foo));

///////////// ToSingle

// Create one definitive instance of Foo and re-use that for every class that asks for it
Container.Bind<Foo>().ToSingle();

// Create one definitive instance of Foo and re-use that for every class that asks for IFoo
Container.Bind<IFoo>().ToSingle<Foo>();

// In this example, the same instance of Foo will be used for all three cases
Container.Bind<Foo>().ToSingle();
Container.Bind<IFoo>().ToSingle<Foo>();
Container.Bind<IBar>().ToSingle<Foo>();

// Non generic versions
Container.Bind(typeof(Foo)).ToSingle();
Container.Bind(typeof(IFoo)).ToSingle(typeof(Foo));

///////////// BindAllInterfacesToSingle

// Bind all interfaces that Foo implements to a new singleton of type Foo
Container.BindAllInterfacesToSingle<Foo>();
// So for example if Foo implements ITickable and IInitializable then the above
// line is equivalent to this:
Container.Bind<ITickable>().ToSingle<Foo>();
Container.Bind<IInitializable>().ToSingle<Foo>();

///////////// ToInstance

// Use the given instance everywhere that Foo is used
Container.Bind<Foo>().ToInstance(new Foo());

// This is simply a shortcut for the above binding
// This can be a bit nicer since the type argument can be deduced from the parameter
Container.BindInstance(new Foo());

// Note that ToInstance is different from ToSingle because it does allow multiple bindings
// and you can't re-use the same instance in multiple bindings like you can with ToSingle
// For example, the following is allowed and will match any constructor parameters of type List<Foo>
// (and throw an exception for parameters that ask for a single Foo)
Container.Bind<Foo>().ToInstance(new Foo());
Container.Bind<Foo>().ToInstance(new Foo());

///////////// ToSingleInstance

// Use the given instance everywhere Foo is requested and ensure that it is the only Foo that is created
Container.Bind<Foo>().ToSingleInstance(new Foo());

// We assume here that Foo implements both IFoo and IBar
// This will result in the given instance of Foo used for all three cases
Container.Bind<IFoo>().ToSingleInstance(new Foo());
Container.Bind<IBar>().ToSingle<Foo>();
Container.Bind<Foo>().ToSingle();

///////////// Binding primitive types

// Use the number 10 every time an int is requested
// You'd never really want to do this, you should almost always use a When condition for primitive values (see conditions section below)
Container.Bind<int>().ToInstance(10);
Container.Bind<bool>().ToInstance(false);

// These are the same as above
// This can be a bit nicer though since the type argument can be deduced from the parameter
// Again though, be careful to use conditions to limit the scope of usage for values
// or consider using a Settings object as described above
Container.BindInstance(10);
Container.BindInstance(false);

///////////// ToMethod

// Create instance of Foo when requested, using the given method
// Note that for more complex construction scenarios, you might consider using a factory
// instead
Container.Bind<Foo>().ToMethod(GetFoo);

Foo GetFoo(InjectContext ctx)
{
    return new Foo();
}

// Randomly return one of several different implementations of IFoo
// We use Instantiate here instead of just new so that Foo1 gets its members injected
Container.Bind<IFoo>().ToMethod(GetFoo);

IFoo GetFoo(InjectContext ctx)
{
    switch (Random.Range(0, 3))
    {
        case 0:
            return ctx.Container.Instantiate<Foo1>();

        case 1:
            return ctx.Container.Instantiate<Foo2>();
    }

    return ctx.Container.Instantiate<Foo3>();
}

// Using lambda syntax
Container.Bind<Foo>().ToMethod((ctx) => new Foo());

// This is equivalent to ToTransient
Container.Bind<Foo>().ToMethod((ctx) => ctx.Container.Instantiate<Foo>());

///////////// ToGetter

// Bind to a property on another dependency
// This can be helpful to reduce coupling between classes
Container.Bind<Foo>().ToSingle();

Container.Bind<Bar>().ToGetter<Foo>(foo => foo.GetBar());

// Another example using values
Container.Bind<string>().ToGetter<Foo>(foo => foo.GetTitle());

///////////// ToSingleGameObject

// Create a new game object at the root of the scene, add the Foo MonoBehaviour to it, and name it "Foo"
Container.Bind<Foo>().ToSingleGameObject("Foo");

// Bind to an interface instead
Container.Bind<IFoo>().ToSingleGameObject<Foo>("Foo");

///////////// ToSinglePrefab

// Create a new game object at the root of the scene using the given prefab
// It is assumed that the Foo is a MonoBehaviour here and that Foo has been
// previously added to the prefab
// After zenject creates a new GameObject from the given prefab, it will
// search the prefab for a component of type 'Foo' and return that
GameObject fooPrefab;
Container.Bind<Foo>().ToSinglePrefab(fooPrefab);

// Bind to interface instead
Container.Bind<IFoo>().ToSinglePrefab<Foo>(fooPrefab);

// Note that in this case only one prefab will be instantiated and re-used
// for all three bindings
// (Prefab singletons are uniquely identified by their prefab)
Container.Bind<Foo>().ToSinglePrefab(fooPrefab);
Container.Bind<IInitializable>().ToSinglePrefab<Foo>(fooPrefab);
Container.Bind<ITickable>().ToSinglePrefab<Foo>(fooPrefab);

///////////// ToTransientPrefab

// Instantiate a new copy of 'fooPrefab' every time an instance of Foo is
// requested by a constructor parameter, injected field, etc.
GameObject fooPrefab;
Container.Bind<Foo>().ToTransientPrefab(fooPrefab);

// Bind to interface instead
Container.Bind<IFoo>().ToTransientPrefab<Foo>(fooPrefab);

///////////// Identifiers

// By default this will use 'Qux' for every place that requires an instance of IFoo
// But also allow for classes to use FooA or FooB by using identifiers
Container.Bind<IFoo>().ToSingle<Qux>();
Container.Bind<IFoo>("FooA").ToSingle<Bar>();
Container.Bind<IFoo>("FooB").ToSingle<Baz>();

public class Norf
{
    // Uses Qux
    [Inject]
    IFoo _foo;

    // Uses Bar
    [Inject("FooA")]
    IFoo _foo;

    // Uses Baz if it exists, otherwise leaves it as null
    [InjectOptional("FooB")]
    IFoo _foo;
}

// Bind a globally accessible string with the name 'PlayerName'
// A better option might be to create a Settings object and bind that
// instead however
Container.Bind<string>("PlayerName").ToInstance("name of the player");

///////////// Conditions

// This will only allow dependencies on Foo by the Bar class
Container.Bind<Foo>().ToSingle().WhenInjectedInto<Bar>();

// Use different implementations of IFoo dependending on which
// class is being injected
Container.Bind<IFoo>().ToSingle<Foo1>().WhenInjectedInto<Bar>();
Container.Bind<IFoo>().ToSingle<Foo2>().WhenInjectedInto<Qux>();

// Use "Foo1" as the default implementation except when injecting into
// class Qux, in which case use Foo2
Container.Bind<IFoo>().ToSingle<Foo1>();
Container.Bind<IFoo>().ToSingle<Foo2>().WhenInjectedInto<Qux>();

// Allow depending on Foo in only a few select classes
Container.Bind<Foo>().ToSingle().WhenInjectedInto(typeof(Bar), typeof(Qux), typeof(Baz));

// Supply "my game" for any strings that are injected into the Gui class with the identifier "Title"
Container.BindInstance("Title", "my game").WhenInjectedInto<Gui>();

// Supply 5 for all ints that are injected into the Gui class
Container.BindInstance(5).WhenInjectedInto<Gui>();

// Supply 5 for all ints that are injected into a parameter or field
// inside type Gui that is named 'width'
// This is usually not a good idea since the name of a field can change
// easily and break the binding but shown here as an example  :)
Container.BindInstance(5.0f).When(ctx =>
    ctx.ObjectType == typeof(Gui) && ctx.MemberName == "width");

// Create a new 'Foo' for every class that is created as part of the
// construction of the 'Bar' class
// So if Bar has a constructor parameter of type Qux, and Qux has
// a constructor parameter of type IFoo, a new Foo will be created
// for that case
Container.Bind<IFoo>().ToTransient<Foo>().When(
    ctx => ctx.AllObjectTypes.Contains(typeof(Bar)));

///////////// Complex conditions example

var foo1 = new Foo();
var foo2 = new Foo();

Container.Bind<Bar>("Bar1").ToTransient();
Container.Bind<Bar>("Bar2").ToTransient();

// Here we use the 'ParentContexts' property of inject context to sync multiple corresponding identifiers
Container.BindInstance(foo1).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && x.Identifier == "Bar1").Any());
Container.BindInstance(foo2).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && x.Identifier == "Bar2").Any());

// This results in:
// Container.Resolve<Bar>("Bar1").Foo == foo1
// Container.Resolve<Bar>("Bar2").Foo == foo2

///////////// ToLookup

// This will result in IBar, IFoo, and Foo, all being bound to the same instance of
// Foo which is assume to exist somewhere on the given prefab
GameObject fooPrefab;
Container.Bind<Foo>().ToSinglePrefab(fooPrefab);
Container.Bind<IBar>().ToLookup<Foo>()
Container.Bind<IFoo>().ToLookup<IBar>()

// This is result in the same as the above
GameObject fooPrefab;
Container.Bind<Foo>().ToSinglePrefab(fooPrefab);
Container.Bind<IBar>().ToSinglePrefab<Foo>(fooPrefab);
Container.Bind<IFoo>().ToSinglePrefab<Foo>(fooPrefab);

///////////// Rebind

// Rebind can be used to override previous bindings
// This will result in IFoo being bound to only Bar
// The binding to Foo will have been removed
// Normally the order that the bindings occur in doesn't
// matter at all, but Rebind does create an order-dependency
// so use with caution
Container.Bind<IFoo>().ToSingle<Foo>();
Container.Rebind<IFoo>().ToSingle<Bar>();

///////////// Installing Other Installers

// Immediately call InstallBindings() on FooInstaller
Container.Install<FooInstaller>();

// Before calling FooInstaller, configure a property of it
Container.BindInstance("foo").WhenInjectedInto<FooInstaller>();
Container.Install<FooInstaller>();

// After calling FooInstaller, override one of its bindings
// We assume here that FooInstaller binds IFoo to something
Container.Install<FooInstaller>();
Container.Rebind<IFoo>().ToSingle<Bar>();

///////////// Manual Use of Container

// This will fill in any parameters marked as [Inject] and also call the [PostInject]
// function
foo = new Foo();
Container.Inject(foo);

// Return an instance for IFoo, using the bindings that have been added previously
// Internally it is what is triggered when you fill in a constructor parameter of type IFoo
// Note: It will throw an exception if it cannot find a match
Container.Resolve<IFoo>();

// Same as the above except returns null when it can't find the given type
Container.TryResolve<IFoo>();

// Return a list of 2 instances of type Foo
Container.BindInstance(new Foo());
Container.BindInstance(new Foo());
var foos = Container.ResolveAll<IFoo>();

// Instantiate a new instance of Foo and inject on any of its members
Container.Instantiate<Foo>();

// Instantiate a new prefab and have any injectables filled in on the prefab
GameObject go = Container.InstantiatePrefab(prefab);

// Instantiate a new prefab and return a specific monobehaviour
Foo foo = Container.InstantiatePrefabForComponent<Foo>(prefab);

// Add a new component to an existing game object
Foo foo = Container.InstantiateComponent<Foo>(gameObject);

```

## <a id="further-help"></a>Further Help

For general troubleshooting / support, please use the [zenject subreddit](http://www.reddit.com/r/zenject) or the [zenject google group](https://groups.google.com/forum/#!forum/zenject/).  If you have found a bug, you are also welcome to create an issue on the [github page](https://github.com/modesttree/Zenject), or a pull request if you have a fix / extension.  You can also follow [@Zenject](https://twitter.com/Zenject) on twitter for updates.  Finally, you can also email me directly at sfvermeulen@gmail.com

## <a id="release-notes"></a>Release Notes

3.4 (Jan 7, 2015)
- Cleaned up directory structure
- Fixed bug with Global bindings not getting their Tick() called in the correct order
- Fixes to the releases automation scripts

3.2 (December 20, 2015)
- Added the concept of "Commands" and "Signals".  See documentation for details.
- Fixed validation for decorator scenes that open decorator scenes.
- Changed to be more strict when using a combination of differents kinds of ToSingle<>, since there should only be one way to create the singleton.
- Added ToSingleFactory bind method, for cases where you have complex construction requirements and don't want to use ToSingleMethod
- Removed the InjectFullScene flag on SceneCompositionRoot.  Now always injects on the full scene.
- Renamed AllowNullBindings to IsValidating so it can be used for other kinds of validation-only logic
- Renamed BinderUntyped to UntypedBinder and BinderGeneric to GenericBinder
- Added the ability to install MonoInstaller's directly from inside other installers by calling Container.Install<MyCustomMonoInstaller>().  In this case it tries to load a prefab from Resources/Installers/MyCustomMonoInstaller.prefab before giving up.  This can be helpful to keep scenes incredibly small instead of having many installer prefabs.
- Added the ability to install MonoInstaller's directly from inside other installers.  In this case it tries to load a prefab from the resources directory before giving up.
- Added some better error output in a few places
- Fixed some iOS AOT issues
- Added BindFacade<> method to DiContainer, to allow creating nested containers without needing to use a factory.
- Added an Open button in scene decorator comp root for easily jumping to the decorated scene
- Removed support for object graph visualization since I hadn't bothered maintaining it
- Got the optional Moq extension method ToMock() working again
- Fixed scene decorators to play more nicely with Unity's own way of handling LoadLevelAdditive.  Decorated scenes are now organized in the scene heirarchy under scene headings just like when calling LoadLevelAdditive normally

3.1
- Changes related to upgrading to Unity 5.3
- Fixed again to make zero heap allocations per frame

3.0
- Added much better support for nested containers.  It now works more closely to what you might expect:  Any parent dependencies are always inherited in sub-containers, even for optional injectables.  Also removed BindScope and FallbackContainer since these were really just workarounds for this feature missing.  Also added [InjectLocal] attribute for cases where you want to inject dependencies only from the local container.
- Changed the way execution order is specified in the installers.  Now the order for Initialize / Tick / Dispose are all given by one property similar to how unity does it, using ExecutionOrderInstaller
- Added ability to pass arguments to Container.Install<>
- Added support for using Facade pattern in combination with nested containers to allow easily created distinct 'islands' of dependencies.  See documentation for details
- Changed validation to be executed on DiContainer instead of through BindingValidator for ease of use
- Added automatic support for WebGL by marking constructors as [Inject]

2.8
* Fixed to properly use explicit default parameter values in Constructor/PostInject methods.  For eg: public Foo(int bar = 5) should consider bar to be optional and use 5 if not resolved.

2.7
* Bug fix to ensure global composition root always gets initialized before the scene composition root
* Changed scene decorators to use LoadLevelAdditive instead of LoadLevel to allow more complex setups involving potentially several decorators within decorators

2.6
* Added new bind methods: ToResource, ToTransientPrefabResource, ToSinglePrefabResource
* Added ability to have multiple sets of global installers
* Fixed support for using zenject with .NET 4.5
* Created abstract base class CompositionRoot for both SceneCompositionRoot and GlobalCompositionRoot
* Better support for using the same DiContainer from multiple threads
* Added back custom list inspector handler to make it easier to re-arrange etc.
* Removed the extension methods on DiContainer to avoid a gotcha that occurs when not including 'using Zenject
* Changed to allow having a null root transform given to DiContainer
* Changed to assume any parameters with hard coded default values (eg: int x = 5) are InjectOptional
* Fixed bug with asteroids project which was causing exceptions to be thrown on the second run due to the use of tags

2.5
* Added support for circular dependencies in the PostInject method or as fields (just not constructor parameters)
* Fixed issue with identifiers that was occurring when having both [Inject] and [InjectOptional] attributes on a field/constructor parameter.  Now requires that only one be set
* Removed BindValue in favour of just using Bind for both reference and value types for simplicity
* Removed GameObjectInstantiator class since it was pretty awkward and confusing.  Moved methods directly into IInstantiator/DiContainer.  See IInstantiator class.
* Extracted IResolver and IBinder interfaces from DiContainer

2.4
* Refactored the way IFactory is used to be a lot cleaner. It now uses a kind of fluent syntax through its own bind method BindIFactory<>

2.3
* Added "ParentContexts" property to InjectContext, to allow very complex conditional bindings that involve potentially several identifiers, etc.
* Removed InjectionHelper class and moved methods into DiContainer to simplify API and also to be more discoverable
* Added ability to build dlls for use in outside unity from the assembly build solution

2.2
* Changed the way installers invoke other installers.  Previously you would Bind them to IInstaller and now you call Container.Install<MyInstaller> instead.  This is better because it allows you to immediately call Rebind<> afterwards

2.1
* Simplified interface a bit more by moving more methods into DiContainer such as Inject and Instantiate.  Moved all helper methods into extension methods for readability. Deleted FieldsInjector and Instantiator classes as part of this
* Renamed DiContainer.To() method to ToInstance since I had witnessed some confusion with it for new users.  Did the same with ToSingleInstance
* Added support for using Zenject outside of Unity by building with the ZEN_NOT_UNITY3D define set
* Bug fix - Validation was not working in some cases for prefabs.
* Renamed some of the parameters in InjectContext for better understandability.
* Renamed DiContainer.ResolveMany to DiContainer.ResolveAll
* Added 'InjectFullScene' flag to CompositionRoot to allow injecting across the entire unity scene instead of just objects underneath the CompositionRoot

2.0

* Added ability to inject dependencies via parameters to the [PostInject] method just like it does with constructors.  Especially useful for MonoBehaviours.
* Fixed the order that [PostInject] methods are called in for prefabs
* Changed singletons created via ToSinglePrefab to identify based on identifier and prefab and not component type. This allows things like ToSingle<Foo>(prefab1) and ToSingle<Bar>(prefab1) to use the same prefab, so you can map singletons to multiple components on the same prefab. This also works with interfaces.
* Removed '.As()' method in favour of specifying the identifier in the first Bind() statement
* Changed identifiers to be strings instead of object to avoid accidental usage
* Renamed ToSingle(obj) to ToSingleInstance to avoid conflict with specifying an identifier
* Fixed validation to work properly for ToSinglePrefab
* Changed to allow using conditions to override a default binding. When multiple providers are found it will now try and use the one with conditions.  So for example you can define a default with `Container.Bind<IFoo>().ToSingle<Foo1>()` and then override for specific classes with `Container.Bind<IFoo>().ToSingle<Foo2>().WhenInjectedInto<Bar>()`, etc.

1.19

* Upgraded to Unity 5
* Added an optional identifier to InjectOptional attribute
* Changed the way priorities are interpreted for tickables, disposables, etc. Zero is now used as default for any unspecified priorities.  This is helpful because it allows you to choose priorities that occur either before or after the unspecified priorities.
* Added some helper methods to ZenEditorUtil for use by CI servers to validate all scenes

1.18

* Added minor optimizations to reduce per-frame allocation to zero
* Fixed unit tests to be compatible with unity test tools
* Minor bug fix with scene decorators, GameObjectInstantiator.

1.17

* Bug fix.  Was not forwarding parameters correctly when instantiating objects from prefabs

1.16

* Removed the word 'ModestTree' from namespaces since Zenject is open source and not proprietary to the company ModestTree.

1.15

* Fixed bug with ToSinglePrefab which was causing it to create multiple instances when used in different bindings.

1.14

* Added flag to CompositionRoot for whether to inject into inactive game objects or ignore them completely
* Added BindAllInterfacesToSingle method to DiContainer
* Changed to call PostInject[] on children first when instantiating from prefab
* Added ILateTickable interface, which works just like ITickable or IFixedTickable for unity's LateUpdate event
* Added support for 'decorators', which can be used to add dependencies to another scene

1.13

* Minor bug fix to global composition root.  Also fixed a few compiler warnings.

1.12

* Added Rebind<> method
* Changed Factories to use strongly typed parameters by default.  Also added ability to pass in null values as arguments as well as multiple instances of the same type
* Renamed _container to Container in the installers
* Added support for Global Composition Root to allow project-wide installers/bindings
* Added DiContainer.ToSingleMonoBehaviour method
* Changed to always include the StandardUnityInstaller in the CompositionRoot class.
* Changed TickableManager to not be a monobehaviour and receive its update from the UnityDependencyRoot instead
* Added IFixedTickable class to support unity FixedUpdate method

1.11

* Removed Fasterflect library to keep Zenject nice and lightweight (it was also causing issues on WP8)
* Fixed bug related to singletons + object graph validation. Changed the way IDisposables are handled to be closer to the way IInitializable and ITickable are handled. Added method to BinderUntyped.

1.10

* Added custom editor for the Installers property of CompositionRoot to make re-ordering easier

1.09

* Added support for nested containers
* Added ability to execute bind commands using Type objects rather than a generic type
* Changed the way IDisposable bindings work to be similar to how ITickable and IInitializable work
* Bug fixes

1.08

* Order of magnitude speed improvement by using more caching
* Minor change to API to use the As() method to specify identifiers
* Bug fixes

1.07

* Simplified API by removing the concept of modules in favour of just having installers instead (and add support for installers installing other installers)
* Bug fixes

1.06

* Introduced concept of scene installer, renamed installers 'modules'
* Bug fixes

## <a id="license"></a>License

    The MIT License (MIT)

    Copyright (c) 2010-2015 Modest Tree Media  http://www.modesttree.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
