using Lin.Core;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lin.WorkerHost;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IBotClientHendler _botClientHendler;

    private readonly ITelegramBotClient _botClient;

    private readonly ReceiverOptions _receiverOptions;

    public Worker(
        ILogger<Worker> logger,
        IBotClientHendler botClientHendler,
        ITelegramBotClient botClient,
        ReceiverOptions receiverOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botClientHendler = botClientHendler ?? throw new ArgumentNullException(nameof(botClientHendler));
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _receiverOptions = receiverOptions ?? throw new ArgumentNullException(nameof(receiverOptions));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botClient.StartReceiving(
            UpdateHandler,
            ErrorHandler,
            _receiverOptions,
            stoppingToken);

        _logger.LogInformation("Бот запущен!");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(200, stoppingToken);
        }
        await Task.CompletedTask;
    }

    private async Task UpdateHandler(
        ITelegramBotClient client,
        Update update,
        CancellationToken token)
    {
        try
        {
            var task = update.Type switch
            {
                UpdateType.Message => _botClientHendler.UpdateMessageHandle(update, token),
                _ => throw new NotImplementedException()
            };
            await task;
            return;
        }
        catch (LogicException ex)
        {
            _logger.LogError(ex.Message);

            if (update?.Message?.Chat is not null)
            {
                await client.SendMessage(
                    update.Message.Chat,
                    $"{ex.Message}. Please try again",
                    cancellationToken: token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            if (update?.Message?.Chat is not null)
            {
                await client.SendMessage(
                    update.Message.Chat,
                    "Internal error. Please try again",
                    cancellationToken: token);
            }
        }
    }

    private Task ErrorHandler(
        ITelegramBotClient botClient,
        Exception error,
        HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        _logger.LogError(ErrorMessage);
        return Task.CompletedTask;
    }
}