namespace SunsetOffsetCalculator
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    public class HelpMeBorders : INotifyPropertyChanged
    {
        private Brush originSunrise;

        private Brush originSunset;

        private Brush destinationSunrise;

        private Brush destinationSunset;

        private Brush alternateSunrise;

        private Brush alternateSunset;

        private Thickness originSunriseThickness;

        private Thickness originSunsetThickness;

        private Thickness destinationSunriseThickness;

        private Thickness destinationSunsetThickness;

        private Thickness alternateSunriseThickness;

        private Thickness alternateSunsetThickness;

        public event PropertyChangedEventHandler PropertyChanged;

        public HelpMeBorders()
        {
            this.originSunrise = new SolidColorBrush(Colors.LightGray);
            this.originSunset = new SolidColorBrush(Colors.LightGray);
            this.destinationSunrise = new SolidColorBrush(Colors.LightGray);
            this.destinationSunset = new SolidColorBrush(Colors.LightGray);
            this.alternateSunrise = new SolidColorBrush(Colors.LightGray);
            this.alternateSunset = new SolidColorBrush(Colors.LightGray);
            this.originSunriseThickness = new Thickness(1);
            this.originSunsetThickness = new Thickness(1);
            this.destinationSunriseThickness = new Thickness(1);
            this.destinationSunsetThickness = new Thickness(1);
            this.alternateSunriseThickness = new Thickness(1);
            this.alternateSunsetThickness = new Thickness(1);
        }

        public Brush OriginSunrise
        {
            get => this.originSunrise;

            set
            {
                if (Equals(this.originSunrise, value))
                {
                    return;
                }

                this.originSunrise = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.OriginSunrise)));
            }
        }

        public Brush OriginSunset
        {
            get => this.originSunset;

            set
            {
                if (Equals(this.originSunset, value))
                {
                    return;
                }

                this.originSunset = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.OriginSunset)));
            }
        }
       
        public Brush DestinationSunrise
        {
            get => this.destinationSunrise;

            set
            {
                if (Equals(this.destinationSunrise, value))
                {
                    return;
                }

                this.destinationSunrise = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DestinationSunrise)));
            }
        }
        
        public Brush DestinationSunset
        {
            get => this.destinationSunset;

            set
            {
                if (Equals(this.destinationSunset, value))
                {
                    return;
                }

                this.destinationSunset = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DestinationSunset)));
            }
        }
        
        public Brush AlternateSunrise
        {
            get => this.alternateSunrise;

            set
            {
                if (Equals(this.alternateSunrise, value))
                {
                    return;
                }

                this.alternateSunrise = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.AlternateSunrise)));
            }
        }
        
        public Brush AlternateSunset
        {
            get => this.alternateSunset;

            set
            {
                if (Equals(this.alternateSunset, value))
                {
                    return;
                }

                this.alternateSunset = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.AlternateSunset)));
            }
        }

        public Thickness OriginSunriseThickness
        {
            get => this.originSunriseThickness;

            set
            {
                if (Equals(this.originSunriseThickness, value))
                {
                    return;
                }

                this.originSunriseThickness = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.OriginSunriseThickness)));
            }
        }

        public Thickness OriginSunsetThickness
        {
            get => this.originSunsetThickness;

            set
            {
                if (Equals(this.originSunsetThickness, value))
                {
                    return;
                }

                this.originSunsetThickness = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.OriginSunsetThickness)));
            }
        }

        public Thickness DestinationSunriseThickness
        {
            get => this.destinationSunriseThickness;

            set
            {
                if (Equals(this.destinationSunriseThickness, value))
                {
                    return;
                }

                this.destinationSunriseThickness = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DestinationSunriseThickness)));
            }
        }

        public Thickness DestinationSunsetThickness
        {
            get => this.destinationSunsetThickness;

            set
            {
                if (Equals(this.destinationSunsetThickness, value))
                {
                    return;
                }

                this.destinationSunsetThickness = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DestinationSunsetThickness)));
            }
        }

        public Thickness AlternateSunriseThickness
        {
            get => this.alternateSunriseThickness;

            set
            {
                if (Equals(this.alternateSunriseThickness, value))
                {
                    return;
                }

                this.alternateSunriseThickness = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.AlternateSunriseThickness)));
            }
        }

        public Thickness AlternateSunsetThickness
        {
            get => this.alternateSunsetThickness;

            set
            {
                if (Equals(this.alternateSunsetThickness, value))
                {
                    return;
                }

                this.alternateSunsetThickness = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.AlternateSunsetThickness)));
            }
        }

        public void Reset()
        {
            this.OriginSunrise = new SolidColorBrush(Colors.LightGray);
            this.OriginSunset = new SolidColorBrush(Colors.LightGray);
            this.DestinationSunrise = new SolidColorBrush(Colors.LightGray);
            this.DestinationSunset = new SolidColorBrush(Colors.LightGray);
            this.AlternateSunrise = new SolidColorBrush(Colors.LightGray);
            this.AlternateSunset = new SolidColorBrush(Colors.LightGray);
            this.OriginSunriseThickness = new Thickness(1);
            this.OriginSunsetThickness = new Thickness(1);
            this.DestinationSunriseThickness = new Thickness(1);
            this.DestinationSunsetThickness = new Thickness(1);
            this.AlternateSunriseThickness = new Thickness(1);
            this.AlternateSunsetThickness = new Thickness(1);
        }
    }
}
