using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UltraborgLib;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using Microsoft.IoT.Lightning.Providers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UltraBorgUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page,INotifyPropertyChanged
    {
        private Ultraborg ultraborg { get; set; }
        public String ProviderName { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public String Distance { get; set; }
        public String RawDistance { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            ultraborg = new Ultraborg();

        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MessageDialog dlg;
            try
            {
                var useLightningProvider = LightningProvider.IsLightningEnabled;
                if (useLightningProvider)
                    ProviderName = "Controller Driver = Lightning";
                else
                    ProviderName = "Controller Driver = Default";

                var initResult = await ultraborg.Init(useLightningProvider);
                if (!initResult)
                {
                    dlg = new MessageDialog("PIBorg Init failure");
                    await dlg.ShowAsync();
                }

                this.cntlServo1.Init(ultraborg);
                this.cntlServo2.Init(ultraborg);
                this.cntlServo3.Init(ultraborg);
                this.cntlServo4.Init(ultraborg);

                this.cntlSonic1.Init(ultraborg);
                this.cntlSonic2.Init(ultraborg);
                this.cntlSonic3.Init(ultraborg);
                this.cntlSonic4.Init(ultraborg);
            }
            catch (Exception ex)
            {
                dlg = new MessageDialog("PIBorg initialization exception:" + ex.Message);
                await dlg.ShowAsync();
            }

            RaisePropertyChanged("ProviderName");
        }

        private async  void BtnLGBOn_Click(object sender, RoutedEventArgs e)
        {
            var gpio = GpioController.GetDefault();
            var pwm = await PwmController.GetDefaultAsync();

            var pinR = gpio.OpenPin(26);
            var pinG = gpio.OpenPin(6);
            var pinB = gpio.OpenPin(13);

            pinR.SetDriveMode(GpioPinDriveMode.Output);
            pinG.SetDriveMode(GpioPinDriveMode.Output);
            pinB.SetDriveMode(GpioPinDriveMode.Output);

            pinR.Write(GpioPinValue.High);
            pinG.Write(GpioPinValue.High);
            pinB.Write(GpioPinValue.High);
        }

        private async void BtnLGBOff_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dlg;
            try
            {
                var gpio = GpioController.GetDefault();
                var pinR = gpio.OpenPin(26);
                //var pinG = gpio.OpenPin(19);
                var pinB = gpio.OpenPin(13);

                pinR.SetDriveMode(GpioPinDriveMode.Output);
                //pinG.SetDriveMode(GpioPinDriveMode.Output);
                pinB.SetDriveMode(GpioPinDriveMode.Output);

                pinR.Write(GpioPinValue.High);
                //pinG.Write(GpioPinValue.High);
                pinB.Write(GpioPinValue.High);
            }
            catch (Exception ex)
            {
                dlg = new MessageDialog("Unable to open pin: " + ex.Message);
                await dlg.ShowAsync();
            }
        }
    }
}
