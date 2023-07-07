using System.Windows;
using System.Windows.Input;

namespace CalCounter
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void goCalendar(object sender, RoutedEventArgs e)
        {
            CalendarPage calendarPage = new CalendarPage();
            calendarPage.Show();
            Close();
        }
        private void goStats(object sender, RoutedEventArgs e)
        {
            StatsPage statsPage = new StatsPage();
            statsPage.Show();
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void goProfile(object sender, RoutedEventArgs e)
        {
            ProfilePage profilePage = new ProfilePage();
            profilePage.Show();
            Close();
        }
    }
}
