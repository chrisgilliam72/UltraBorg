using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UltraborgLib;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UltraBorgUI
{
    public sealed partial class MotorTuningControl : UserControl, INotifyPropertyChanged
    {
        public int ServoNo { get; set; }
        public Ultraborg Ultraborg { get; set; }
        private string _minValue;


        public String MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
                RaisePropertyChanged("MinValue");
            }
        }

        private string _maxValue;
        public String MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
                RaisePropertyChanged("MaxValue");
            }
        }

        private string _startValue;
        public String StartValue
        {
            get
            {
                return _startValue;
            }
            set
            {
                _startValue = value;
                RaisePropertyChanged("StartValue");
            }
        }

        private int _sliderMin;
        public int SliderMin
        {
            get
            {
                return _sliderMin;
            }
            set
            {
                _sliderMin = value;
                RaisePropertyChanged("SliderMin");
            }
        }

        private int _sliderMax;
        public int SliderMax
        {
            get
            {
                return _sliderMax;
            }
            set
            {
                _sliderMax = value;
                RaisePropertyChanged("SliderMax");
            }
        }

        private int _currentPosition;
        public int CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                _currentPosition = value;
                RaisePropertyChanged("CurrentPosition");
            }
        }


        private String _titleText;
        public String TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                _titleText = value;
                RaisePropertyChanged("TitleText");
            }
        }
        
        public MotorTuningControl()
        {
           this.InitializeComponent();
           SliderMin = 0;
           SliderMax = 6000;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            if (Ultraborg!=null)
                Ultraborg.CalibrateServoPosition(ServoNo,Convert.ToInt32(e.NewValue));
        }

        private void BtnMax_Click(object sender, RoutedEventArgs e)
        {
            if (Ultraborg != null)
            {
                Ultraborg.SetServoMaximum(ServoNo,CurrentPosition);
                MaxValue = CurrentPosition.ToString();
            }

        }

        private void btnBoot_Click(object sender, RoutedEventArgs e)
        {
            if (Ultraborg != null)
            {
                Ultraborg.SetServoBoot(ServoNo,CurrentPosition);
                StartValue= CurrentPosition.ToString();
            }

        }

        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            if (Ultraborg != null)
            {
                Ultraborg.SetServoMinimum(ServoNo,CurrentPosition);
                MinValue = CurrentPosition.ToString();
            }

        }

        private void Slider_ValueChanged_1(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Ultraborg != null)
            {
                double servoPos = 0.0;
                int pwmMin = Convert.ToInt32(MinValue);
                int pwmMax = Convert.ToInt32(MaxValue);
                var pos = e.NewValue;

                servoPos = (pos-50.0 ) / 50.0;               

                Ultraborg.SetServoPosition(ServoNo,servoPos, pwmMin, pwmMax);
          
            }
        }

        public void Init(Ultraborg ultraborg)
        {
            Ultraborg = ultraborg;
            var limitsServo = Ultraborg.GetServoLimits(ServoNo);

            MaxValue = limitsServo.Maximum.ToString();
            MinValue = limitsServo.Minimum.ToString();
            StartValue = limitsServo.Start.ToString();
            CurrentPosition = limitsServo.Start;

            
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
    }
}
