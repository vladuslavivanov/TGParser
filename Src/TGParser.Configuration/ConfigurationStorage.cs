using TGParser.Configuration.Models;

namespace TGParser.Configuration;

public static class ConfigurationStorage
{
    public static PostgresModel GetPostgresModel()
    {
        var host = ConfigurationParser.GetValue<string>(EnvironmentNames.POSTGRES_HOST, true);
        var port = ConfigurationParser.GetValue<int>(EnvironmentNames.POSTGRES_PORT, true);
        var user = ConfigurationParser.GetValue<string>(EnvironmentNames.POSTGRES_USER, true);
        var password = ConfigurationParser.GetValue<string>(EnvironmentNames.POSTGRES_PASSWORD, true);
        var dbName = ConfigurationParser.GetValue<string>(EnvironmentNames.POSTGRES_DATABASE_NAME, true);

        return new PostgresModel(host, port, user, password, dbName);
    }

    public static string GetTelegramBotClientOptions()
    {
        var id = ConfigurationParser.GetValue<string>(EnvironmentNames.TELEGRAM_ID, true);
        var hash = ConfigurationParser.GetValue<string>(EnvironmentNames.TELEGRAM_HASH, true);

        return id + ":" + hash;
    }

    public static string GetWebhookApi()
    {
        return ConfigurationParser.GetValue<string>(EnvironmentNames.TELEGRAM_WEBHOOK, true);
    }

    public static string GetTelegramSecretToken()
    {
        return ConfigurationParser.GetValue<string>(EnvironmentNames.TELEGRAM_SECRET_TOKEN, true);
    }

    public static string GetCryptoBotSecretToken()
    {
        return ConfigurationParser.GetValue<string>(EnvironmentNames.CRYPTO_BOT_TOKEN, true);
    }

    public static int GetTrialDays()
    {
        return ConfigurationParser.GetValue(EnvironmentNames.TRIAL_PERIOD_DAYS, false, 0);
    }

    public static string GetChannelName()
    {
        return ConfigurationParser.GetValue<string>(EnvironmentNames.CHECK_SUBSCRIBE_CHANNEL_NAME, true);        
    }

    public static PriceModel GetPrice()
    {
        int priceOneDay = ConfigurationParser.GetValue<int>(EnvironmentNames.PRICE_ONE_DAY, true);
        int priceOneWeek = ConfigurationParser.GetValue<int>(EnvironmentNames.PRICE_SEVEN_DAYS, true);
        int priceOneMonth = ConfigurationParser.GetValue<int>(EnvironmentNames.PRICE_ONE_MONTH, true);

        return new(priceOneDay, priceOneWeek, priceOneMonth);
    }
}
