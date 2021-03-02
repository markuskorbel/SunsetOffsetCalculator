namespace SunsetOffsetCalculator
{
    using System;
    using System.ComponentModel;

    public class Airport : INotifyPropertyChanged
    {
        private string icao;

        private string lat;

        private string lon;

        private TimeSpan ete;

        private DateTime? sunrise;

        private DateTime? sunset;

        private string sunriseOffset;

        private string sunsetOffset;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ICAO
        {
            get => this.icao;

            set
            {
                if (Equals(this.icao, value))
                {
                    return;
                }

                this.icao = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ICAO)));
            }
        }

        public string Lat
        {
            get => this.lat;

            set
            {
                if (Equals(this.lat, value))
                {
                    return;
                }

                this.lat = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Lat)));
            }
        }

        public string Lon
        {
            get => this.lon;

            set
            {
                if (Equals(this.lon, value))
                {
                    return;
                }

                this.lon = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Lon)));
            }
        }

        public TimeSpan ETE
        {
            get => this.ete;

            set
            {
                if (Equals(this.ete, value))
                {
                    return;
                }

                this.ete = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ETE)));
            }
        }

        public DateTime? Sunrise
        {
            get => this.sunrise;

            set
            {
                if (Equals(this.sunrise, value))
                {
                    return;
                }

                this.sunrise = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Sunrise)));
            }
        }

        public DateTime? Sunset
        {
            get => this.sunset;

            set
            {
                if (Equals(this.sunset, value))
                {
                    return;
                }

                this.sunset = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Sunset)));
            }
        }

        public string SunriseOffset
        {
            get => this.sunriseOffset;

            set
            {
                if (Equals(this.sunriseOffset, value))
                {
                    return;
                }

                this.sunriseOffset = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SunriseOffset)));
            }
        }

        public string SunsetOffset
        {
            get => this.sunsetOffset;

            set
            {
                if (Equals(this.sunsetOffset, value))
                {
                    return;
                }

                this.sunsetOffset = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SunsetOffset)));
            }
        }
    }
}
