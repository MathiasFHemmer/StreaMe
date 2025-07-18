using System.Reflection;

public sealed class Configuration
{
    public string ServiceName { get; set; }

    public string ServiceVersion { get; set; }
    public string AlloyUri { get; set; } = "http://alloy:4317";
}

public static class ConfigurationExtensions
{
    public static Configuration BindRuntimeValues(this Configuration config)
    {
        config.ServiceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown";
        config.ServiceVersion = "1.0.0";

        return config;
    }
}