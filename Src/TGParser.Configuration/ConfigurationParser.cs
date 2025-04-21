using Microsoft.Extensions.Configuration;

namespace TGParser.Configuration;

internal static class ConfigurationParser
{
    static readonly IConfiguration _configuration = 
        new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, false)
        .AddEnvironmentVariables()
        .Build();
        
    public static T GetValue<T>(string key, bool isRequired, T? defaultValue = default)
    {
        var value =_configuration.GetValue<T>(key);
        
        if (value == null)
        {
            if (isRequired)
                throw new Exception($"The required environment variable '{key}' is not set.");
            if (defaultValue == null)
                throw new Exception($"The default value for environment variable '{key}' is not set.");
            return defaultValue;
        }
        
        return value;
    }
}
