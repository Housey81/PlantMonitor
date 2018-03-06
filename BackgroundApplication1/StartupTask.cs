using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using System.Diagnostics;
using System.Threading;
using Windows.UI.Xaml;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BackgroundApplication1
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;

        private GpioPin _sensorPin;
        private GpioPin _yellowPin;
        private GpioPin _redPin;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            this.InitGPIO();

            var timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(500));
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            if (_sensorPin.Read() == GpioPinValue.High)
            {
                _yellowPin.Write(GpioPinValue.Low);
                _redPin.Write(GpioPinValue.High);
            }
            else
            {
                _yellowPin.Write(GpioPinValue.High);
                _redPin.Write(GpioPinValue.Low);
            }
        }

        private void InitGPIO()
        {
            _redPin = this.InitPin(22);
            _yellowPin = this.InitPin(23);   
            _sensorPin = this.InitPin(17, GpioPinDriveMode.Input);
        }

        private GpioPin InitPin(int number, 
            GpioPinDriveMode mode = GpioPinDriveMode.Output, 
            GpioPinValue initialValue = GpioPinValue.Low)
        {
            var gpio = GpioController.GetDefault();         
            var pin = gpio.OpenPin(number);
            pin.SetDriveMode(mode);

            if(mode == GpioPinDriveMode.Output)
            {
                pin.Write(initialValue);
            }

            return pin;
        }
    }


}
