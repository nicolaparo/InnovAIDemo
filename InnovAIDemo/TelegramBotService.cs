using InnovAIDemo.Ollama;
using OllamaSharp;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InnovAIDemo
{
    /// <summary>
    /// Background service that handles Telegram bot operations.
    /// Continuously polls for incoming messages and processes them using AI agents.
    /// </summary>
    /// <param name="botClient">The Telegram bot client</param>
    /// <param name="agentsService">Service for managing AI agent instances</param>
    /// <param name="logger">Logger for this service</param>
    public class TelegramBotService(ITelegramBotClient botClient, AgentInstancesService agentsService, ILogger<TelegramBotService> logger) : BackgroundService
    {
        /// <summary>
        /// Main execution method for the background service.
        /// Continuously polls for Telegram updates and processes incoming messages.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token to stop the service</param>
        /// <returns>A task representing the asynchronous operation</returns>
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

        /// <summary>
        /// Handles incoming Telegram messages.
        /// Processes the message with the appropriate AI agent and sends a response.
        /// </summary>
        /// <param name="update">The Telegram update containing the message</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A task representing the asynchronous operation</returns>
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
