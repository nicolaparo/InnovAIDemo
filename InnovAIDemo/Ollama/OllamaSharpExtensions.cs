using OllamaSharp;
using OllamaSharp.Models;

namespace InnovAIDemo.Ollama
{
    /// <summary>
    /// Extension methods for the Ollama API client to simplify model management operations.
    /// </summary>
    public static class OllamaSharpExtensions
    {
        /// <summary>
        /// Ensures the default model is available, downloading it if necessary.
        /// </summary>
        /// <param name="client">The Ollama API client</param>
        /// <param name="onprogress">Optional progress callback for download progress</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task EnsureDefaultModelAsync(this IOllamaApiClient client, Func<PullModelResponse?, Task>? onprogress = null)
        {
            await client.EnsureModelAsync(client.SelectedModel, onprogress);
        }
        
        /// <summary>
        /// Ensures the default model is available, downloading it if necessary.
        /// </summary>
        /// <param name="client">The Ollama API client</param>
        /// <param name="onprogress">Optional progress callback for download progress</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task EnsureDefaultModelAsync(this IOllamaApiClient client, Action<PullModelResponse?>? onprogress)
        {
            await client.EnsureModelAsync(client.SelectedModel, onprogress);
        }
        
        /// <summary>
        /// Ensures a specific model is available, downloading it if necessary.
        /// </summary>
        /// <param name="client">The Ollama API client</param>
        /// <param name="modelName">The name of the model to ensure</param>
        /// <param name="onprogress">Optional progress callback for download progress</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task EnsureModelAsync(this IOllamaApiClient client, string modelName, Action<PullModelResponse?>? onprogress)
        {
            await client.EnsureModelAsync(modelName, onprogress is null ? null : (update => Task.Run(() => onprogress(update))));
        }
        
        /// <summary>
        /// Ensures a specific model is available, downloading it if necessary.
        /// Checks if the model exists first, and only downloads if it's not available.
        /// </summary>
        /// <param name="client">The Ollama API client</param>
        /// <param name="modelName">The name of the model to ensure</param>
        /// <param name="onprogress">Optional progress callback for download progress</param>
        /// <returns>A task representing the asynchronous operation</returns>
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
