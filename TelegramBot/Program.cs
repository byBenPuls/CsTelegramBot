using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
class Program
{
    private static TelegramBotClient client;
    static void Main(string[] args)
    {

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
{
            new KeyboardButton[] { "Help me", "Call me ☎️" },
})
            {
                ResizeKeyboard = true
            };

            // Echo received message text
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "You said:\n" + messageText,
                cancellationToken: cancellationToken,
                replyMarkup: replyKeyboardMarkup);
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        client = new TelegramBotClient("6507706453:AAHZHCoUZ7WcgDhUZJlhtGXVUNpzkL4Rj8Y");

        using CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        client.StartReceiving(
        updateHandler: HandleUpdateAsync,
        pollingErrorHandler: HandlePollingErrorAsync,
        receiverOptions: receiverOptions,
        cancellationToken: cts.Token
);
        Console.ReadLine();
    }
    
}