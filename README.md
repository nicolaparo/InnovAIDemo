# InnovAI Demo

A .NET 9 Blazor Server application that integrates AI capabilities with Telegram Bot functionality, powered by Ollama and equipped with various tools for weather information, LED control, Wikipedia search, and geolocation services.

## 🚀 Features

- **AI-Powered Telegram Bot**: Interactive bot that responds to messages using local AI models
- **Local AI Integration**: Uses Ollama for running AI models locally
- **Smart Tools**: The AI agent can use various tools including:
  - Weather information (OpenMeteo API)
  - Wikipedia search
  - Geolocation services (Nominatim)
  - LED control (Raspberry Pi GPIO support)
  - Quick text summarization
- **Real-time Communication**: Live updates and chat functionality

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Ollama](https://ollama.ai/) installed and running
- **Required AI Models**: `qwen2.5:3b` and `gemma3:270m`
- Telegram Bot Token (see setup instructions below)
- Windows/Linux/macOS (Raspberry Pi supported for LED features)

## 🔧 Quick Start Guide

### 1. Install Ollama

#### Windows
1. Visit [https://ollama.ai/](https://ollama.ai/)
2. Download the Windows installer
3. Run the installer and follow the setup wizard
4. Open Command Prompt or PowerShell and verify installation:
   ```powershell
   ollama --version
   ```

#### macOS
```bash
# Using Homebrew
brew install ollama

# Or download from https://ollama.ai/
```

#### Linux
```bash
curl -fsSL https://ollama.ai/install.sh | sh
```

### 2. Download and Start AI Models

Once Ollama is installed, download the required AI models:

```powershell
# Pull the required models for this solution
ollama pull qwen2.5:3b     # Main AI model for chat and reasoning
ollama pull gemma3:270m      # Lightweight model for quick tasks

# Start Ollama service (if not already running)
ollama serve
```

**Note**: Both `qwen2.5:3b` and `gemma3:270m` models are required for this solution to run properly. The application uses different models for different tasks to optimize performance.

### 3. Create a Telegram Bot Token

1. **Start a chat with BotFather**:
   - Open Telegram and search for `@BotFather`
   - Start a conversation by clicking "Start"

2. **Create a new bot**:
   ```
   /newbot
   ```

3. **Choose a name for your bot**:
   - Enter a display name (e.g., "InnovAI Demo Bot")

4. **Choose a username**:
   - Must end with "bot" (e.g., "innovai_demo_bot")
   - Must be unique across all Telegram

5. **Get your token**:
   - BotFather will provide you with a token like: `123456789:ABCdefGHIjklMNOpqrsTUVwxyz`
   - **Keep this token secure!**

6. **Optional - Set bot description and commands**:
   ```
   /setdescription
   /setcommands
   ```

### 4. Configure the Application

1. **Clone or download the project**
2. **Update the Telegram Bot Token**:
   
   **Option A: Using appsettings.json (for development)**
   ```json
   {
     "TelegramBotToken": "YOUR_BOT_TOKEN_HERE"
   }
   ```
   
   **Option B: Using User Secrets (recommended)**
   ```powershell
   cd InnovAIDemo
   dotnet user-secrets set "TelegramBotToken" "YOUR_BOT_TOKEN_HERE"
   ```
   
   **Option C: Using Environment Variables**
   ```powershell
   $env:TelegramBotToken = "YOUR_BOT_TOKEN_HERE"
   ```

### 5. Build and Run

```powershell
# Navigate to project directory
cd InnovAIDemo

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The application will start and be available at:
- Web Interface: `https://localhost:5001` or `http://localhost:5000`
- Telegram Bot: Ready to receive messages

## 🤖 Using the Bot

1. **Find your bot** on Telegram using the username you created
2. **Start chatting**: Send any message to begin
3. **Try commands**:
   - Ask about weather: "What's the weather in London?"
   - Search Wikipedia: "Tell me about artificial intelligence"
   - Get location info: "Where is the Eiffel Tower?"
   - General conversation: The AI can chat about various topics

## 🛠️ Configuration

### Ollama Settings
Update the Ollama configuration in `Program.cs`:

```csharp
builder.Services.AddSingleton<IOllamaApiClient>(new OllamaApiClient(new OllamaApiClient.Configuration()
{
    Model = "qwen2.5:3b", // Change model here
    Uri = new Uri("http://localhost:11434") // Change Ollama URL if needed
}));
```

### Available Models
You can use different Ollama models by pulling them first:

```powershell
# Smaller, faster models
ollama pull llama3.2:1b
ollama pull qwen2.5:1.5b

# Larger, more capable models
ollama pull llama3.2:3b
ollama pull qwen2.5:7b
ollama pull llama3.1:8b
```

## 📁 Project Structure

```
InnovAIDemo/
├── Components/           # Blazor components
│   ├── Pages/           # Web pages
│   └── Layout/          # Layout components
├── Services/            # Business logic services
│   ├── Weather/         # Weather service integration
│   ├── Led/            # LED control (Raspberry Pi)
│   └── Nominatim/      # Geolocation services
├── Ollama/             # AI agent implementation
├── Program.cs          # Application entry point
├── TelegramBotService.cs # Telegram bot service
└── README.md           # This file
```

## 🔌 Available Tools/Services

The AI agent has access to several tools:

- **WeatherService**: Get current weather and forecasts
- **WikipediaSearchService**: Search and retrieve Wikipedia articles
- **NominatimService**: Geocoding and reverse geocoding
- **LedService**: Control LEDs on Raspberry Pi (GPIO)
- **OllamaQuickSummarizer**: Summarize long texts

## 🐛 Troubleshooting

### Common Issues

1. **"Ollama not found" error**:
   - Ensure Ollama is installed and running
   - Check if the service is accessible at `http://localhost:11434`

2. **"Model not found" error**:
   - Pull the required models: `ollama pull qwen2.5:3b` and `ollama pull gemma3:270m`
   - Verify both models are available by running `ollama list`
   - Check the model names in configuration

3. **Telegram bot not responding**:
   - Verify the bot token is correct
   - Check if the token is properly configured in settings
   - Ensure the application is running

4. **Port conflicts**:
   - The app uses ports 5000/5001 by default
   - Modify `Properties/launchSettings.json` to change ports

### Logs and Debugging

The application logs important information to the console. Look for:
- Telegram message receipts
- Tool invocations
- AI responses
- Error messages

## 🚀 Deployment

### Local Development
```powershell
dotnet run --environment Development
```

### Production
```powershell
dotnet publish -c Release
# Deploy the contents of bin/Release/net9.0/publish/
```

### Docker (Optional)
Create a `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY . .
EXPOSE 8080
ENTRYPOINT ["dotnet", "InnovAIDemo.dll"]
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is open source. Please check the license file for details.

## 🆘 Support

For issues and questions:
1. Check the troubleshooting section above
2. Review application logs for error details
3. Ensure all prerequisites are properly installed
4. Verify configuration settings

## 🔗 Useful Links

- [Ollama Documentation](https://ollama.ai/docs)
- [Telegram Bot API](https://core.telegram.org/bots/api)
- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/)
