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
Noodle.EngineContext.Resolve<ILanguageService>();
```