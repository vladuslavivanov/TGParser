using Telegram.Bot;
using TGParser.Configuration;
using Microsoft.EntityFrameworkCore;
using TGParser.DAL;
using TGParser.BLL.Interfaces;
using TGParser.BLL.Implementations;
using MassTransit;
using System.Reflection;
using TGParser.API.Services.Implementations;
using TGParser.API.Services.Interfaces;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Controllers.Dialogs.Implementations;
using Serilog;
using TGParser.API.Controllers.Dialogs.Implementations.Proxy;
using TGParser.API.Controllers.Dialogs.Implementations.Preset;
using CryptoPay;
using TGParser.API.Controllers.CallbackQueries;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Controllers.CallbackQueries.Implementations;
using TGParser.API.Controllers.Messages.ChatShared.Implementations;
using TGParser.API.Controllers.Messages.ChatShared.Implementations.Parsing;
using TGParser.API.Controllers.Messages.ChatShared.Implementations.Preset;
using TGParser.API.Controllers.Messages.ChatShared.Implementations.Proxy;
using TGParser.API.Controllers.Messages;
using TGParser.API.Controllers.Messages.Interfaces;

namespace TGParser.API.Extensions;

internal static class IServiceCollectionExtensions
{
    public static void AddServiceCollections(this IServiceCollection services)
    {
        services.AddTelegramClient();

        services.AddCommandHandlers();
        services.AddCallbackQueryHandlers();
        services.AddChatSharedHandlers();
        services.AddDialogs();
        services.AddDbContext();
        services.AddManagers();
        services.AddMassTransit();
        services.AddServices();

        services.AddControllers();
        services.ConfigureTelegramBot<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt => opt.SerializerOptions);
    }

    static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDialogService, DialogService>();
        services.AddScoped<IProxyService, ProxyService>();

        services.AddScoped<ISearchWallapopService, SearchWallapopService>();
        services.AddScoped<ICryptoBotService, CryptoBotService>();

        services.AddScoped<ITelegramUserService, TelegramUserService>();

        services.AddHttpClient<ICryptoPayClient, CryptoPayClient>(client =>
        {
            var apiToken = ConfigurationStorage.GetCryptoBotSecretToken();

            //client.BaseAddress = new Uri("https://pay.crypt.bot/");
            client.BaseAddress = new Uri("https://testnet-pay.crypt.bot/");
            client.DefaultRequestHeaders.Add("Crypto-Pay-API-Token", apiToken);

            return new(client);
        });

        services.AddScoped<IUserService, UserService>();

        services.AddMemoryCache();
    }

    static void AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IMessage, DefaultCommand>();
        services.AddScoped<IMessage, ProfileCommand>();

        services.AddScoped<IMessage, PresetCommand>();
        services.AddScoped<IMessage, AddPresetCommand>();
        services.AddScoped<IMessage, RemovePresetCommand>();
        services.AddScoped<IMessage, EditPresetCommand>();
        services.AddScoped<IMessage, SetDefaultPreset>();

        services.AddScoped<IMessage, ProxiesCommand>();
        services.AddScoped<IMessage, AddProxyCommand>();
        services.AddScoped<IMessage, EditProxyCommand>();
        services.AddScoped<IMessage, RemoveProxyCommand>();
        services.AddScoped<IMessage, TestProxyCommand>();
        
        services.AddScoped<IMessage, SearchWallapopCommand>();

        services.AddScoped<IMessage, BuyDaysCommand>();

        services.AddScoped<IMessage, HelpCommand>();

        services.AddScoped<CommandExecutor>();
    }

    static void AddCallbackQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICallbackQuery, TargetParserCallbackQuery>();
        services.AddScoped<ICallbackQuery, ParseLimitCallbackQuery>();
        services.AddScoped<ICallbackQuery, SearchAdvCallbackQuery>();

        services.AddScoped<CallbackQueryExecutor>();
    }

    static void AddChatSharedHandlers(this IServiceCollection services)
    {
        services.AddScoped<IMessage, SelectUserForParseMessage>();
    }

    static void AddDialogs(this IServiceCollection services)
    {
        services.AddScoped<IDialogExecutor, DialogExecutor>();

        services.AddScoped<IDialog, AddPresetDialog>();
        services.AddScoped<IDialog, EditingPresetDialog>();
        services.AddScoped<IDialog, RemovePresetDialog>();
        services.AddScoped<IDialog, SetDefaultPresetDialog>();

        services.AddScoped<IDialog, AddProxyDialog>();
        services.AddScoped<IDialog, EditingProxyDialog>();
        services.AddScoped<IDialog, RemoveProxyDialog>();
        services.AddScoped<IDialog, TestProxyDialog>();
        
        services.AddScoped<IDialog, BuyDaysDialog>();
    }

    static void AddDbContext(this IServiceCollection services) 
    {
        var connectionString = ConfigurationStorage.GetPostgresModel().ConnectionString;
        services.AddDbContextPool<DataContext>(optionsAction => optionsAction.UseNpgsql(connectionString));
    }

    static void AddMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    static void AddTelegramClient(this IServiceCollection services)
    {
        var options = ConfigurationStorage.GetTelegramBotClientOptions();
        var telegramBotClient = new TelegramBotClient(options);
        services.AddTransient<ITelegramBotClient>(services => telegramBotClient);
    }

    public static void ConfigureLogger(this WebApplicationBuilder host)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
            outputTemplate: "[{Timestamp:dd.MM.yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("default", policy =>
            {
                policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });
    }

    static void AddManagers(this IServiceCollection services)
    {
        services.AddTransient<IPresetManager, PresetManager>();
        services.AddTransient<IProductManager, ProductManager>();
        services.AddTransient<IUserManager, UserManager>();
        services.AddTransient<IUserPresetManager, UserPresetManager>();
        services.AddTransient<IUserViewedItemsManager, UserViewedItemsManager>();
        services.AddTransient<IProxyManager, ProxyManager>();
    }
}
