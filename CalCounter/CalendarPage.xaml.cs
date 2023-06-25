
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalCounter
{
    /// <summary>
    /// Interaction logic for CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage
    {
        readonly SqlConnection con = new SqlConnection();
        readonly SqlCommand com = new SqlCommand();
        readonly SqlCommand com2 = new SqlCommand();
        SqlDataReader dr;
        public CalendarPage()
        {
            InitializeComponent();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
            con.Open();

            com.Connection = con;
            com2.Connection = con;
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

        private DateTime currentDate(System.Windows.Controls.Calendar calendar)
        {
            return calendar.SelectedDate ?? DateTime.MinValue;
        }

        private void ShowCalories(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = currentDate(calendar);
            com.CommandText = "select * from calendar where username='" + LoginPage.currentUser + "' and date='" + selectedDate.Date.ToString() + "'";
            dr = com.ExecuteReader();
            int calories;

            if (dr.Read())
            {
                calories = (int)dr.GetValue(dr.GetOrdinal("Calories"));
            }
            else
            {
                calories = 0;
            }
            dr.Close();
            caloriesDisplay.Text = $"Current Calories: {calories}";
        }

        private void AddCalories(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = currentDate(calendar);
            int calories = int.Parse(calorieInput.Text);
            int currentCal = 0;
            com.CommandText = "select * from calendar where username='" + LoginPage.currentUser + "' and date='" + selectedDate.Date.ToString() + "'";
            dr = com.ExecuteReader();

            if (dr.Read())
            {
                currentCal = (int)dr.GetValue(dr.GetOrdinal("Calories"));
                com2.CommandText = $"update calendar set calories={currentCal + calories} where username='{LoginPage.currentUser}' and date='{selectedDate.Date.ToString()}'";
                com2.ExecuteNonQuery();
            }
            else
            {
                com2.CommandText = $"insert into calendar (username,date,calories) values ('{LoginPage.currentUser}', '{selectedDate.Date.ToString()}',{calories})";
                com2.ExecuteNonQuery();
            }
            dr.Close();
            caloriesDisplay.Text = $"Current Calories: {calories + currentCal}";
        }

        private void RemoveCalories(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = currentDate(calendar);
            int calories = int.Parse(calorieInput.Text);
            int currentCal;
            com.CommandText = "select * from calendar where username='" + LoginPage.currentUser + "' and date='" + selectedDate.Date.ToString() + "'";
            dr = com.ExecuteReader();

            if (dr.Read())
            {
                currentCal = (int)dr.GetValue(dr.GetOrdinal("Calories"));
                if (calories > currentCal)
                {
                    error.Text = "Can not remove more calories than present";
                }
                else
                {
                    com2.CommandText = $"update calendar set calories={currentCal - calories} where username='{LoginPage.currentUser}' and date='{selectedDate.Date.ToString()}'";
                    com2.ExecuteNonQuery();
                    caloriesDisplay.Text = $"Current Calories: {currentCal - calories}";
                }
            }
            else
            {
                error.Text = "Unable to remove non-existent calories";

            }
            dr.Close();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;

            listBox.ScrollIntoView(listBox.Items[0]);

            if (selectionMenu.SelectedIndex == 0)
            {
                removeCalories.Visibility = Visibility.Collapsed;
                addCalories.Visibility = Visibility.Visible;
            }
            else if (selectionMenu.SelectedIndex == 1)
            {
                removeCalories.Visibility = Visibility.Visible;
                addCalories.Visibility = Visibility.Collapsed;
            }
        }
    }
}
