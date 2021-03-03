namespace SunsetOffsetCalculator
{
    using Newtonsoft.Json.Linq;
    using SunsetOffsetCalculator.MVVM;
    using SunsetOffsetCalculator.Properties;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Xml.Linq;

    public class CalculatorViewModel : ViewModel
    {
        private string simBriefUsername;

        private TimeZoneInfo selectedTimeZone;

        private string localTime;

        private string utcTime;

        public CalculatorViewModel()
        {
            // Init data structures
            this.TimeZones = new ObservableCollection<TimeZoneInfo>(TimeZoneInfo.GetSystemTimeZones());
            this.selectedTimeZone = TimeZoneInfo.Local;
            this.Origin = new Airport { ETE = TimeSpan.FromMinutes(30) };
            this.Destination = new Airport();
            this.Alternate = new Airport();

            // Init commands
            this.ImportSimBriefCommand = new AsynchronousCommand(this.ImportSimBrief, false);
            this.AddDepartureTimeCommand = new Command(this.AddDepartureTime);
            this.RemoveDepartureTimeCommand = new Command(this.RemoveDepartureTime);
            this.GetGeoLocationCommand = new AsynchronousCommand(this.GetGeoLocation);
            this.GetSunriseSunsetCommand = new AsynchronousCommand(this.GetSunsetSunrise);

            // Spawn timer update thread
            new Thread(this.UpdateTimes) { Name = "UpdateTimes" }.Start();

            // Load settings
            this.SimBriefUsername = Settings.Default.SimbriefUsername;
        }

        public ObservableCollection<TimeZoneInfo> TimeZones { get; }

        public Airport Origin { get; }

        public Airport Destination { get; }

        public Airport Alternate { get; }

        public TimeZoneInfo SelectedTimeZone
        {
            get => this.selectedTimeZone;

            set
            {
                if (Equals(this.selectedTimeZone, value))
                {
                    return;
                }

                this.selectedTimeZone = value;
                this.NotifyPropertyChanged();
            }
        }

        public string SimBriefUsername
        {
            get => this.simBriefUsername;

            set
            {
                if (Equals(this.simBriefUsername, value))
                {
                    return;
                }

                this.simBriefUsername = value;
                this.NotifyPropertyChanged();
                this.ImportSimBriefCommand.CanExecute = !string.IsNullOrEmpty(this.simBriefUsername);
            }
        }

        public string LocalTime
        {
            get => this.localTime;

            private set
            {
                if (Equals(this.localTime, value))
                {
                    return;
                }

                this.localTime = value;
                this.NotifyPropertyChanged();
            }
        }

        public string UTCTime
        {
            get => this.utcTime;

            private set
            {
                if (Equals(this.utcTime, value))
                {
                    return;
                }

                this.utcTime = value;
                this.NotifyPropertyChanged();
            }
        }

        public AsynchronousCommand ImportSimBriefCommand { get; }

        public AsynchronousCommand GetGeoLocationCommand { get; }

        public AsynchronousCommand GetSunriseSunsetCommand { get; }

        public Command AddDepartureTimeCommand { get; }

        public Command RemoveDepartureTimeCommand { get; }

        private void ImportSimBrief()
        {
            using (var client = new WebClient())
            {
                try
                {
                    var xml = client.DownloadString($"https://www.simbrief.com/api/xml.fetcher.php?username={this.SimBriefUsername}");
                    Debug.WriteLine(xml);

                    var ofp = XElement.Parse(xml);
                    this.Origin.ICAO = (string)ofp.Element("origin").Element("icao_code");
                    this.Destination.ICAO = (string)ofp.Element("destination").Element("icao_code");
                    this.Alternate.ICAO = (string)ofp.Element("alternate").Element("icao_code");

                    this.Origin.Lat = (string)ofp.Element("origin").Element("pos_lat");
                    this.Destination.Lat = (string)ofp.Element("destination").Element("pos_lat");
                    this.Alternate.Lat = (string)ofp.Element("alternate").Element("pos_lat");

                    this.Origin.Lon = (string)ofp.Element("origin").Element("pos_long");
                    this.Destination.Lon = (string)ofp.Element("destination").Element("pos_long");
                    this.Alternate.Lon = (string)ofp.Element("alternate").Element("pos_long");

                    this.Destination.ETE = TimeSpan.FromSeconds((int)ofp.Element("times").Element("est_time_enroute")).Add(this.Origin.ETE);
                    this.Alternate.ETE = TimeSpan.FromSeconds((int)ofp.Element("alternate").Element("ete")).Add(this.Destination.ETE);

                    // If all of this worked, save the simbrief username and fetch the sunset/sunrise info for all airports
                    Settings.Default.SimbriefUsername = this.SimBriefUsername;
                    Settings.Default.Save();

                    this.ImportSimBriefCommand.ReportProgress(() =>
                    {
                        this.GetSunriseSunsetCommand.DoExecute(new List<Airport> { this.Origin, this.Destination, this.Alternate });
                    });
                }
                catch (Exception ex)
                {
                    this.ImportSimBriefCommand.ReportProgress(() =>
                    {
                        MessageBox.Show(ex.Message, "Error fetching from Simbrief", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }

            Debug.WriteLine("Import simbrief finished.");
        }

        private void AddDepartureTime()
        {
            this.Origin.ETE = this.Origin.ETE.Add(TimeSpan.FromMinutes(5));

            if (this.Destination.ETE != TimeSpan.Zero && this.Alternate.ETE != TimeSpan.Zero)
            {
                this.Destination.ETE = this.Destination.ETE.Add(TimeSpan.FromMinutes(5));
                this.Alternate.ETE = this.Alternate.ETE.Add(TimeSpan.FromMinutes(5));
            }
        }

        private void RemoveDepartureTime()
        {
            if (this.Origin.ETE.TotalMinutes > 5)
            {
                this.Origin.ETE = this.Origin.ETE.Subtract(TimeSpan.FromMinutes(5));
                if (this.Destination.ETE != TimeSpan.Zero && this.Alternate.ETE != TimeSpan.Zero)
                {
                    this.Destination.ETE = this.Destination.ETE.Subtract(TimeSpan.FromMinutes(5));
                    this.Alternate.ETE = this.Alternate.ETE.Subtract(TimeSpan.FromMinutes(5));
                }
            }
            else
            {
                if (this.Destination.ETE != TimeSpan.Zero && this.Alternate.ETE != TimeSpan.Zero)
                {
                    this.Destination.ETE = this.Destination.ETE.Subtract(TimeSpan.FromMinutes(this.Origin.ETE.TotalMinutes));
                    this.Alternate.ETE = this.Alternate.ETE.Subtract(TimeSpan.FromMinutes(this.Origin.ETE.TotalMinutes));
                }
                this.Origin.ETE = TimeSpan.Zero;
            }
        }

        private void GetGeoLocation(object sender)
        {
            var airport = sender as Airport;
            if (airport == null || string.IsNullOrEmpty(airport.ICAO))
            {
                return;
            }

            using (var client = new WebClient())
            {
                try
                {
                    if (!Equals(airport.ICAO, airport.ICAO.ToUpper()))
                    {
                        airport.ICAO = airport.ICAO.ToUpper();
                    }

                    var json = client.DownloadString($"http://iatageo.com/getICAOLatLng/{airport.ICAO}");
                    Debug.WriteLine(json);

                    var geoLocation = JObject.Parse(json);
                    airport.Lat = (string)geoLocation["latitude"];
                    airport.Lon = (string)geoLocation["longitude"];

                    // If that worked for this airport, fire of the command to get the sunrise/sunset data
                    this.GetGeoLocationCommand.ReportProgress(() =>
                    {
                        this.GetSunriseSunsetCommand.DoExecute(airport);
                    });
                }
                catch (Exception ex)
                {
                    this.GetGeoLocationCommand.ReportProgress(() =>
                    {
                        MessageBox.Show(ex.Message, "Error fetching geo location for ICAO code", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }
        }

        private void GetSunsetSunrise(object sender)
        {
            var airports = new List<Airport>();

            if (sender is Airport)
            {
                var airport = sender as Airport;
                if (!string.IsNullOrEmpty(airport?.Lat) && !string.IsNullOrEmpty(airport?.Lon))
                {
                    airports.Add(airport);
                }
            }

            if (sender is List<Airport>)
            {
                airports.AddRange(sender as List<Airport>);
            }

            using (var client = new WebClient())
            {
                foreach (var airport in airports)
                {
                    try
                    {
                        var json = client.DownloadString($"https://api.sunrise-sunset.org/json?lat={airport.Lat}&lng={airport.Lon}&formatted=0");
                        Debug.WriteLine(json);

                        var srss = JObject.Parse(json);
                        airport.Sunrise = ((DateTime)srss["results"]["sunrise"]);
                        airport.Sunset = ((DateTime)srss["results"]["sunset"]);
                    }
                    catch (Exception ex)
                    {
                        this.GetSunriseSunsetCommand.ReportProgress(() =>
                        {
                            MessageBox.Show(ex.Message, "Error fetching sunset/sunrise times", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                }
            }

            Debug.WriteLine("Get sunrise finished.");
        }

        private void UpdateTimes()
        {
            while (!App.IsShuttingDown)
            {
                this.LocalTime = DateTime.Now.Add(this.SelectedTimeZone.GetUtcOffset(DateTime.Now)).ToString(" HH:mm:ss");
                this.UTCTime = DateTime.UtcNow.ToString(" HH:mm:ss");

                var airports = new List<Airport> { this.Origin, this.Destination, this.Alternate };
                foreach (var airport in airports)
                {
                    if (airport.Sunrise.HasValue && airport.Sunset.HasValue)
                    {
                        var minSunRise = TimeSpan.FromHours(24);
                        var minSunSet = TimeSpan.FromHours(24);
                        var sunriseOffset = -13;
                        var sunsetOffset = -13;
                        for (int i = -12; i <= 12; i++)
                        {
                            var srDelta = DateTime.UtcNow.Add(this.SelectedTimeZone.GetUtcOffset(DateTime.Now)).Add(airport.ETE).AddHours(i) - airport.Sunrise.Value;
                            if (srDelta.TotalSeconds < 0)
                            {
                                srDelta = srDelta.Negate();
                            }

                            if (srDelta.TotalHours > 23)
                            {
                                srDelta = srDelta.Subtract(TimeSpan.FromDays(1));
                            }

                            if (srDelta.TotalSeconds < 0)
                            {
                                srDelta = srDelta.Negate();
                            }

                            if (srDelta < minSunRise)
                            {
                                minSunRise = srDelta;
                                sunriseOffset = i;
                            }

                            var ssDelta = DateTime.UtcNow.Add(this.SelectedTimeZone.GetUtcOffset(DateTime.Now)).Add(airport.ETE).AddHours(i) - airport.Sunset.Value;
                            if (ssDelta.TotalSeconds < 0)
                            {
                                ssDelta = ssDelta.Negate();
                            }

                            if (ssDelta.TotalHours > 23)
                            {
                                ssDelta = ssDelta.Subtract(TimeSpan.FromDays(1));
                            }

                            if (ssDelta.TotalSeconds < 0)
                            {
                                ssDelta = ssDelta.Negate();
                            }

                            if (ssDelta < minSunSet)
                            {
                                minSunSet = ssDelta;
                                sunsetOffset = i;
                            }
                        }

                        airport.SunriseOffset = $"{sunriseOffset:+00;-00}  Δ{minSunRise:mm\\:ss}";
                        if (sunriseOffset == -12 || sunriseOffset == 12)
                        {
                            airport.SunriseOffset = $"±12 Δ{minSunRise:mm\\:ss}";
                        }

                        airport.SunsetOffset = $"{sunsetOffset:+00;-00} Δ{minSunSet:mm\\:ss}";
                        if (sunsetOffset == -12 || sunsetOffset == 12)
                        {
                            airport.SunsetOffset = $"±12 Δ{minSunSet:mm\\:ss}";
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
