using Telegram.Bot;
using TGParser.Configuration;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.Controllers.Commands;
using TGParser.API.Controllers.Commands.Implementations;
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
using TGParser.API.Controllers.Commands.Implementations.Preset;
using TGParser.API.Controllers.Dialogs.Implementations.Proxy;
using TGParser.API.Controllers.Dialogs.Implementations.Preset;
using TGParser.API.Controllers.Commands.Implementations.Proxy;
using TGParser.API.Controllers.Commands.Implementations.Parsing;
using TGParser.API.Controllers.Dialogs.Implementations.Parsing;
using CryptoPay;

namespace TGParser.API.Extensions;

internal static class IServiceCollectionExtensions
{
    public static void AddServiceCollections(this IServiceCollection services)
    {
        services.AddTelegramClient();

        services.AddCommands();
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

    static void AddCommands(this IServiceCollection services)
    {
        services.AddScoped<ICommand, DefaultCommand>();
        services.AddScoped<ICommand, ProfileCommand>();

        services.AddScoped<ICommand, PresetCommand>();
        services.AddScoped<ICommand, AddPresetCommand>();
        services.AddScoped<ICommand, RemovePresetCommand>();
        services.AddScoped<ICommand, EditPresetCommand>();
        services.AddScoped<ICommand, SetDefaultPreset>();

        services.AddScoped<ICommand, ProxiesCommand>();
        services.AddScoped<ICommand, AddProxyCommand>();
        services.AddScoped<ICommand, EditProxyCommand>();
        services.AddScoped<ICommand, RemoveProxyCommand>();
        services.AddScoped<ICommand, TestProxyCommand>();
        
        services.AddScoped<ICommand, SearchWallapopCommand>();

        services.AddScoped<ICommand, BuyDaysCommand>();

        services.AddScoped<CommandExecutor>();
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
        
        services.AddScoped<IDialog, SearchWallapopDialog>();

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
