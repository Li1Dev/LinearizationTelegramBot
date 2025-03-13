using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Lin.Core;

public interface IBotClientHendler
{
    Task UpdateMessageHandle(
        Update update,
        CancellationToken token);
}