using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CalCounter
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage
    {
        readonly SqlConnection con = new SqlConnection();
        readonly SqlCommand com = new SqlCommand();
        readonly SqlCommand com1 = new SqlCommand();
        SqlDataReader dr;

        public RegisterPage()
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

        private bool validateRegister(string username, string password, string conPassword)
        {
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from users where username='" + username + "'";
            dr = com.ExecuteReader();

            var converter = new BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#FF00FF00");


            if (dr.Read() && dr.HasRows)
            {
                error.Text = "Username already exists.";
                error.Visibility = Visibility.Visible;
                return false;
            }
            else if (!(username.Length > 5) || !(password.Length > 5))
            {
                error.Text = "Username/Password too short.";
                error.Visibility = Visibility.Visible;
                return false;
            }
            else if (!(conPassword == password))
            {
                error.Text = "Confirmed password does not match.";
                error.Visibility = Visibility.Visible;
                return false;
            }
            else
            {
                error.Text = "Account successfully created.";
                error.Foreground = brush;
                error.Visibility = Visibility.Visible;
                return true;
            }
        }

        private void register(object sender, RoutedEventArgs e)
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
            if (validateRegister(usernameInput.Text, passwordInput.Password.ToString(), conPasswordInput.Password.ToString()))
            {
                com1.Connection = con;
                com1.CommandText = "insert into users (Username,Password) values ('" + usernameInput.Text + "','" + passwordInput.Password.ToString() + "')";
                com1.ExecuteNonQuery();
                con.Close();
            }
        }

        public void gotoLogin(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            Close();
            loginPage.Show();
        }
    }
}
