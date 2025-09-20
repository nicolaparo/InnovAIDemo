namespace InnovAIDemo.Ollama
{
    /// <summary>
    /// Attribute used to mark methods as agent tools that can be called by AI agents.
    /// Methods marked with this attribute will be automatically discovered and made available to agents.
    /// </summary>
    public class AgentToolAttribute() : Attribute
    {
        /// <summary>
        /// Gets or sets the description of what the tool does.
        /// This description is provided to the AI to help it understand when to use the tool.
        /// </summary>
        public string? Description { get; set; }
    }
}
