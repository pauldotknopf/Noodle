namespace Noodle
{
    /// <summary>
    /// This interface marks settings classes as settings.
    /// This allows other libraries to be capable of using the settings database business logic, but not required.
    /// Scenario 1: If you are using Noodle.Settings.dll, then your settings will resolve from the busines logic in that library
    /// Scenario 2: It is up to you to register your settings object with ninject for resolution in services that depend on them
    /// </summary>
    public interface ISettings
    {
    }
}
