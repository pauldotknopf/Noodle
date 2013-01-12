namespace Noodle.Settings
{
    public interface IConfigurationProvider<TSettings> where TSettings : ISettings, new()
    {
        TSettings Settings { get; }
        void SaveSettings(TSettings settings);
    }
}
