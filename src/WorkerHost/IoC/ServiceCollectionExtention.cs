using Telegram.Bot;

namespace Lin.WorkerHost.IoC;

public static class ServiceCollectionExtention
{
    public static IServiceCollection AddTelegramBotClient(this IServiceCollection services, IConfiguration config)
    {
        var token = config.GetValue<string>("Token") ?? throw new InvalidOperationException("Token for bot not found");
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(provider =>
        {
            return new TelegramBotClient(token);
        });

        return services;
    }
}