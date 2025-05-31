using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TGParser.Configuration;
using TGParser.DAL;
using static TGParser.API.Extensions.IServiceCollectionExtensions;

namespace TGParser.API;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureLogger();
        builder.Services.AddServiceCollections();
        builder.Services.ConfigureCors();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseCors("default");

        var webhook = ConfigurationStorage.GetWebhookApi();
        var secretToken = ConfigurationStorage.GetTelegramSecretToken();
        var tClient = app.Services.GetRequiredService<ITelegramBotClient>()!;
        await tClient.SetWebhook(webhook, 
            allowedUpdates: 
            [
                UpdateType.Message, 
                UpdateType.CallbackQuery,
                UpdateType.ShippingQuery
            ], dropPendingUpdates: true,
            secretToken: secretToken);


        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            await context.Database.MigrateAsync();
        }

        await app.RunAsync();
    }
}
