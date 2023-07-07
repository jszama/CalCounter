
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace CalCounter
{
    /// <summary>
    /// Interaction logic for StatsPage.xaml
    /// </summary>
    public partial class StatsPage
    {
        private readonly SqlConnection con = new SqlConnection();
        private readonly SqlCommand com = new SqlCommand();
        private readonly SqlCommand com2 = new SqlCommand();
        private readonly SqlDataReader dr;
        public StatsPage()
        {
            InitializeComponent();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
            con.Open();

            com.Connection = con;
            com2.Connection = con;

            com.CommandText = $"SELECT AVG(calories) AS average_calories FROM (SELECT TOP 30 calories FROM Calendar WHERE Username = '{LoginPage.currentUser}' ORDER BY CONVERT(datetime, date, 103) DESC) AS latest_records;";
            dr = com.ExecuteReader();

            if (dr.Read() && !dr.IsDBNull(dr.GetOrdinal("average_calories")))
            {
                double monthlyAvg = Convert.ToDouble(dr.GetValue(0));
                monthly.Text = $"Monthly Average:{monthlyAvg}";
            }
            else
            {
                monthly.Text = "Monthly Average: N/A";
            }

            dr.Close();
            com2.CommandText = $"SELECT AVG(calories) AS average_calories FROM (SELECT TOP 7 calories FROM Calendar WHERE Username = '{LoginPage.currentUser}' ORDER BY CONVERT(datetime, date, 103) DESC) AS latest_records;";
            dr = com2.ExecuteReader();

            if (dr.Read() && !dr.IsDBNull(0))
            {
                double weeklyAvg = Convert.ToDouble(dr.GetValue(0));
                weekly.Text = $"Weekly Average:{weeklyAvg}";
            }
            else
            {
                weekly.Text = "Weekly Average: N/A";
            }
            dr.Close();
        }
        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void goMain(object sender, RoutedEventArgs e)
        {
            MainPage mainpage = new MainPage();
            mainpage.Show();
            Close();
        }
    }
}
