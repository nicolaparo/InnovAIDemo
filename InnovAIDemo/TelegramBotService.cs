using InnovAIDemo.Ollama;
using OllamaSharp;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InnovAIDemo
{

    public class TelegramBotService(ITelegramBotClient botClient, AgentInstancesService agentsService, ILogger<TelegramBotService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                var offset = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var updates = await botClient.GetUpdates(offset, timeout: 30, cancellationToken: stoppingToken);
                        foreach (var update in updates)
                        {
                            offset = update.Id + 1;

                            if (update.Type == UpdateType.Message && update.Message?.Text != null)
                            {
                                await OnMessageReceivedAsync(update, stoppingToken);
                            }
                        }

                        if (updates.Length == 0)
                        {
                            await Task.Delay(1000, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error receiving Telegram messages");
                        await Task.Delay(5000, stoppingToken);
                    }
                }
            }, stoppingToken);
        }

        private async Task OnMessageReceivedAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null || update.Message.Text == null)
                return;
            var chatId = update.Message.Chat.Id;
            var userMessage = update.Message.Text;

            var agent = agentsService.GetAgentInstance(update.Message.Chat);

            await botClient.SendChatAction(chatId, ChatAction.Typing);

            logger.LogInformation("Received message from {ChatId}: {Message}", chatId, userMessage);

            var response = await agent.SendAsync(userMessage, cancellationToken: cancellationToken)
                .StreamToEndAsync();

            logger.LogInformation("Responding to {ChatId}: {Response}", chatId, response);

            await botClient.SendMessage(chatId, response, ParseMode.Markdown, cancellationToken: cancellationToken);
        }
    }
}
