namespace InnovAIDemo.Services.Led
{
    /// <summary>
    /// Interface for LED control services.
    /// Provides methods to control LED lights and check their state.
    /// </summary>
    public interface ILedService
    {
        /// <summary>
        /// Gets a value indicating whether the LED is currently on.
        /// </summary>
        bool IsLedOn { get; }

        /// <summary>
        /// Disposes of the LED service and cleans up resources.
        /// </summary>
        void Dispose();
        
        /// <summary>
        /// Turns off the LED.
        /// </summary>
        void TurnOff();
        
        /// <summary>
        /// Turns on the LED.
        /// </summary>
        void TurnOn();
    }
}