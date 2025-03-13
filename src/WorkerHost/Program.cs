using Lin.Calculator;
using Lin.Core;
using Lin.Logic;
using Lin.WorkerHost.IoC;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Lin.WorkerHost;

class Program
{
    public static void Main(string[] args)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message],
            DropPendingUpdates = true,
        };

        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services
                    .AddLogging()
                    .AddSingleton(receiverOptions)
                    .AddTelegramBotClient(context.Configuration)
                    .AddSingleton<ILinCalculator, LinCalculator>()
                    .AddSingleton<ICalculationMapper, CalculationMapper>()
                    .AddTransient<ILinService, LinService>()
                    .AddTransient<IBotClientHendler, BotClientHendler>()
                    .AddHostedService<Worker>();
            })
            .Build();

        host.Run();
    }
}
