namespace SunsetOffsetCalculator
{
    using SunsetOffsetCalculator.Properties;
    using System;
    using System.Diagnostics;
    using System.Windows;

    public partial class App : Application
    {
        public static bool IsShuttingDown = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Check if we need to upgrade the user settings file
            if (Settings.Default.SettingsUpdateRequired)
            {
                try
                {
                    Settings.Default.Upgrade();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unable to upgrade existing user settings file to this version: " + ex);
                }

                Settings.Default.SettingsUpdateRequired = false;
                Settings.Default.Save();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            IsShuttingDown = true;
        }
    }
}
