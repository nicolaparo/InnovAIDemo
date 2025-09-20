using InnovAIDemo.Components;
using InnovAIDemo.Services;
using InnovAIDemo.Services.Led;
using InnovAIDemo.Services.Nominatim;
using InnovAIDemo.Services.Weather;
using OllamaSharp;
using System.Threading.Tasks;
using Telegram.Bot;

namespace InnovAIDemo
{
    /// <summary>
    /// Main program class for the InnovAI Demo application.
    /// This is a Blazor Server application that integrates with Ollama AI and Telegram Bot services.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Configures services, sets up dependency injection, and starts the web application.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Not really proud of this, but...
            // The using var httpClient = new() pattern, on every invocation, can lead to socket exhaustion or port exhaustion,
            // especially on resource-constrained devices like a Raspberry Pi.
            // When HttpClient is disposed, the underlying TCP connections go into a TIME_WAIT state and aren't immediately released.
            // If you make multiple requests in quick succession, the system can run out of available sockets.
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("InnovAIDemoApp/1.0");
            builder.Services.AddSingleton(httpClient);

            builder.Services.AddSingleton<IOllamaApiClient>(new OllamaApiClient(new OllamaApiClient.Configuration()
            {
                Model = "qwen2.5:3b",
                Uri = new Uri("http://localhost:11434")
            }));

            if (LedService.IsSupported())
                builder.Services.AddSingleton<ILedService, LedService>();
            else
                builder.Services.AddSingleton<ILedService, FakeLedService>();

            builder.Services.AddSingleton<NominatimService>();
            builder.Services.AddSingleton<WeatherService>();
            builder.Services.AddSingleton<OllamaQuickSummarizer>();
            builder.Services.AddSingleton<WikipediaSearchService>();
            builder.Services.AddTransient<AgentToolsService>();
            builder.Services.AddSingleton<AgentInstancesService>();
            builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(builder.Configuration["TelegramBotToken"], httpClient));

            // Register TelegramBotService as a hosted service
            builder.Services.AddHostedService<TelegramBotService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
