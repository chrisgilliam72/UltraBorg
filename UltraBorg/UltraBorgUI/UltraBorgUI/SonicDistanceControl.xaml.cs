using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UltraborgLib;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SonicDistanceControl : UserControl, INotifyPropertyChanged
    {

        public int SensorNo { get; set; }
        private String _titletext;
        public String TitleText
        {
            get
            {
                return _titletext;
            }
            set
            {
                _titletext = value;
                RaisePropertyChanged("TitleText");
            }
        }

        private string _rawDistance;
        public String RawDistance
        {
            get
            {
                return _rawDistance;
            }
            set
            {
                _rawDistance = value;
                RaisePropertyChanged("RawDistance");
            }
        }

        private string _filteredDistance;
        public String FilteredDistance
        {
            get
            {
                return _filteredDistance;
            }
            set
            {
                _filteredDistance = value;
                RaisePropertyChanged("FilteredDistance");
            }
        }

        private Ultraborg Ultraborg { get; set; }

        private readonly DispatcherTimer dispatcherTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public SonicDistanceControl()
        {
            this.InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
        }

        public void Init(Ultraborg ultraborg)
        {
            Ultraborg = ultraborg;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            if (Ultraborg!=null)
            {
                RawDistance ="Raw distance:"+ Ultraborg.GetDistance(SensorNo).ToString();
                FilteredDistance = "Filtered distance:"+ Ultraborg.GetFilteredDistance(SensorNo).ToString();
            }
        }
    }
}
