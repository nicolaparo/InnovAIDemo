using System.Device.Gpio;

namespace InnovAIDemo.Services.Led
{
    /// <summary>
    /// Real LED service implementation that controls physical LED lights using GPIO.
    /// This service requires GPIO hardware support (e.g., Raspberry Pi).
    /// </summary>
    public class LedService : ILedService
    {
        /// <summary>
        /// Checks if GPIO hardware is supported on the current system.
        /// </summary>
        /// <returns>True if GPIO is supported, false otherwise</returns>
        public static bool IsSupported()
        {
            try
            {
                using var controller = new GpioController();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The GPIO pin number for the LED
        /// </summary>
        private readonly int ledPin;
        
        /// <summary>
        /// The GPIO controller for hardware access
        /// </summary>
        private readonly GpioController gpioController;

        /// <summary>
        /// Gets a value indicating whether the LED is currently on
        /// </summary>
        public bool IsLedOn { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the LedService class.
        /// </summary>
        /// <param name="ledPin">The GPIO pin number to use for the LED (default: 18)</param>
        public LedService(int ledPin = 18)
        {
            this.ledPin = ledPin;
            gpioController = new GpioController();
            gpioController.OpenPin(this.ledPin, PinMode.Output);
            gpioController.Write(this.ledPin, PinValue.Low);
        }

        /// <summary>
        /// Turns on the LED by setting the GPIO pin to high.
        /// </summary>
        public void TurnOn()
        {
            gpioController.Write(ledPin, PinValue.High);
            IsLedOn = true;
        }

        /// <summary>
        /// Turns off the LED by setting the GPIO pin to low.
        /// </summary>
        public void TurnOff()
        {
            gpioController.Write(ledPin, PinValue.Low);
            IsLedOn = false;
        }

        /// <summary>
        /// Disposes of the LED service, turns off the LED, and releases GPIO resources.
        /// </summary>
        public void Dispose()
        {
            gpioController.Write(ledPin, PinValue.Low);
            gpioController.ClosePin(ledPin);
            gpioController.Dispose();
        }
    }
}
