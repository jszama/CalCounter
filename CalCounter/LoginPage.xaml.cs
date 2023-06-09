using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;

namespace CalCounter
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        private async Task PutTaskDelay()
        {
            await Task.Delay(1500);
        }

        public LoginPage()
        {
            InitializeComponent();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private bool validateUser(string username, string password)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#FF00FF00");
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from users where username='" + username + "' and password='" + password + "'";
            dr = com.ExecuteReader();
            if (dr.Read() && dr.HasRows) 
            {
                error.Text = "Successfully logged in.";
                error.Foreground = brush;
                error.Visibility= Visibility.Visible;
                return true;
            }
            else
            {
                error.Text = "Incorrect login details.";
                error.Visibility = Visibility.Visible;
                return false;
            }
        }

        private async void login(object sender, RoutedEventArgs e)
        {
            if (con.State == System.Data.ConnectionState.Open) 
            {
                con.Close();
            }
            if (validateUser(usernameInput.Text, passwordInput.Password.ToString()))
            {
                await PutTaskDelay();
                gotoMain();
            }
        }

        private void gotoMain()
        {
            MainPage mainPage = new MainPage();
            Close();
            mainPage.Show();
        }

        private void gotoSignup(object sender, RoutedEventArgs e)
        {
            RegisterPage registerPage = new RegisterPage();
            Close();
            registerPage.Show();
        }
    }
}
