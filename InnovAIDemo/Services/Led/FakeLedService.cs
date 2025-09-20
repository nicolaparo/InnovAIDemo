namespace InnovAIDemo.Services.Led
{
    /// <summary>
    /// Fake LED service implementation for testing and development purposes.
    /// This service simulates LED control without requiring actual GPIO hardware.
    /// </summary>
    public class FakeLedService : ILedService
    {
        /// <summary>
        /// Gets a value indicating whether the LED is currently on
        /// </summary>
        public bool IsLedOn { get; private set; } = false;
        
        /// <summary>
        /// Simulates turning on the LED
        /// </summary>
        public void TurnOn()
        {
            IsLedOn = true;
        }
        
        /// <summary>
        /// Simulates turning off the LED
        /// </summary>
        public void TurnOff()
        {
            IsLedOn = false;
        }
        
        /// <summary>
        /// Simulates disposing of the LED service
        /// </summary>
        public void Dispose()
        {
            IsLedOn = false;
        }
    }
}
