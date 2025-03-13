using System.Drawing;
using Lin.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lin.WorkerHost;

class BotClientHendler : IBotClientHendler
{
    private readonly ILogger<BotClientHendler> _logger;

    private readonly ILinService _linService;

    private readonly ITelegramBotClient _telegramBotClient;

    public BotClientHendler(
        ILogger<BotClientHendler> logger,
        ILinService linService,
        ITelegramBotClient telegramBotClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _linService = linService ?? throw new ArgumentNullException(nameof(linService));
        _telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
    }

    public async Task UpdateMessageHandle(Update update, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(update.Message);
        ArgumentNullException.ThrowIfNull(update.Message.Text);

        var points = MapToPoints(update.Message.Text);

        var res = await _linService.GetLinAsync(
            points,
            ct);

        await _telegramBotClient.SendMessage(
            update.Message.Chat,
            res.Solving,
            cancellationToken: ct);

        await _telegramBotClient.SendPhoto(
            update.Message.Chat,
            res.Graph,
            cancellationToken: ct);
    }

    private static List<Point> MapToPoints(string text)
    {
        var strings = text.Split(";");

        if (strings.Length != 2) throw new LogicException("Invalid data");

        var numbersX = strings[0].Split(' ', ',');
        var numbersY = strings[1].Split(' ', ',');

        if (numbersX.Length != numbersY.Length) throw new LogicException("Invalid data");

        var result = new List<Point>(numbersX.Length);
        for (int i = 0; i < numbersX.Length; i++)
        {
            result.Add(new Point(
                Convert.ToInt32(numbersX[i]),
                Convert.ToInt32(numbersY[i]))
            );
        }

        return result;
    }
}