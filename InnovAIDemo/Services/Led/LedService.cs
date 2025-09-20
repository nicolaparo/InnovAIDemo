using System.Device.Gpio;

namespace InnovAIDemo.Services.Led
{
    public class LedService
    {
        private readonly int ledPin;
        private readonly GpioController gpioController;

        public bool IsLedOn { get; private set; } = false;

        public LedService(int ledPin = 18)
        {
            this.ledPin = ledPin;
            gpioController = new GpioController();
            gpioController.OpenPin(this.ledPin, PinMode.Output);
            gpioController.Write(this.ledPin, PinValue.Low);
        }

        public void TurnOn()
        {
            gpioController.Write(ledPin, PinValue.High);
            IsLedOn = true;
        }

        public void TurnOff()
        {
            gpioController.Write(ledPin, PinValue.Low);
            IsLedOn = false;
        }

        public void Dispose()
        {
            gpioController.Write(ledPin, PinValue.Low);
            gpioController.ClosePin(ledPin);
            gpioController.Dispose();
        }
    }
}
