namespace SunsetOffsetCalculator
{
    using System.Reflection;
    using System.Windows;

    public partial class Calculator : Window
    {
        public Calculator()
        {
            this.InitializeComponent();
            this.Title += " " + Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor;
        }
    }
}
