using OllamaSharp;
using OllamaSharp.Models;

namespace InnovAIDemo.Ollama
{
    public static class OllamaSharpExtensions
    {
        public static async Task EnsureDefaultModelAsync(this IOllamaApiClient client, Func<PullModelResponse?, Task>? onprogress = null)
        {
            await client.EnsureModelAsync(client.SelectedModel, onprogress);
        }
        public static async Task EnsureDefaultModelAsync(this IOllamaApiClient client, Action<PullModelResponse?>? onprogress)
        {
            await client.EnsureModelAsync(client.SelectedModel, onprogress);
        }
        public static async Task EnsureModelAsync(this IOllamaApiClient client, string modelName, Action<PullModelResponse?>? onprogress)
        {
            await client.EnsureModelAsync(modelName, onprogress is null ? null : (update => Task.Run(() => onprogress(update))));
        }
        public static async Task EnsureModelAsync(this IOllamaApiClient client, string modelName, Func<PullModelResponse?, Task>? onprogress = null)
        {
            try
            {
                await client.ShowModelAsync(modelName);
                return;
            }
            catch { }

            await foreach (var update in client.PullModelAsync(modelName))
            {
                if (onprogress is not null)
                    await onprogress(update);
            }
        }
    }
}
