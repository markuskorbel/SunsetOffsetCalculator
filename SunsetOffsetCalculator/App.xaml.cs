namespace SunsetOffsetCalculator
{
    using System.Windows;

    public partial class App : Application
    {
        public static bool IsShuttingDown = false;

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            IsShuttingDown = true;
        }
    }
}
