using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CalCounter
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        readonly SqlConnection con = new SqlConnection();
        readonly SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        public static string currentUser;

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

        public bool validateUser(string username, string password)
        {
            var converter = new BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#FF00FF00");

            con.Open();
            com.Connection = con;
            com.CommandText = "select * from users where username='" + username + "' and password='" + password + "'";
            dr = com.ExecuteReader();
            if (dr.Read() && dr.HasRows)
            {
                error.Text = "Successfully logged in.";
                error.Foreground = brush;
                error.Visibility = Visibility.Visible;
                currentUser = dr.GetValue(dr.GetOrdinal("Username")).ToString();
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
