Noodle
======

Noodle is a core library that is designed to glue together frameworks as well as provide functionality common throughout most apps.

At it's heart, is a container management system. It handles auto-wiring of services so that implementations "just work".

```c#
/// <summary>
/// Register dependencies for the language service
/// </summary>
public class DependencyRegistrar : IDependencyRegistrar
{
    /// <summary>
    /// Register your services with the container. You are given a type finder to help you find anything you need.
    /// </summary>
    /// <param name="container"></param>
    public void Register(Container container)
    {
        container.RegisterSingle<ILanguageService, LanguageService>();
    }

    /// <summary>
    /// The lower numbers will be registered first. Higher numbers the latest.
    /// If you are registering fakes, give a high integer (int.Max ?) so that that it will be registered last,
    /// and the container will use it instead of the previously registered services.
    /// </summary>
    public int Importance
    {
        get { return 0; }
    }
}
```

This class will get auto-found and invoked. After everything is said and done, the only thing implementations need to worry about is...

```c#
// singleton container for current appdomain
var container = Noodle.EngineContext.Current;

// retrieve services from the container
var languageService = Noodle.EngineContext.Resolve<ILanguageService>();
```

Of course, this library does much more than this. Check the [wiki](https://github.com/theonlylawislove/Noodle/wiki) for the documentation.

* [Container/IDependencyRegistrars](https://github.com/theonlylawislove/Noodle/wiki/Container) - The container (SimpleInjector) and how to register/consume services.
* [Imaging/resizing](https://github.com/theonlylawislove/Noodle/wiki/Imaging) - Resize/manipulate/rotate images with an easy to use implementation.
* [Plugins](https://github.com/theonlylawislove/Noodle/wiki/Plugins) - Framework for building a plugin ecosystem. Create your own services that can support it's own plugins with minimal effort.
* [MongoDB](https://github.com/theonlylawislove/Noodle/wiki/MongoDB) - Services for common usages of MongoDB. Most of the modules used that require persistence (settings, localization, etc) use MongoDB, because you should!
* [Scheduled Tasks](https://github.com/theonlylawislove/Noodle/wiki/Scheduled-Tasks) - Create background tasks that runonce/indefinitely at scheduled intervals.

There are some smaller usable pieces that you may find which haven't been documented just yet :)