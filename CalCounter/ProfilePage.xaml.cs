using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;



namespace CalCounter
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage
    {
        readonly SqlConnection con = new SqlConnection();
        readonly SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        string userPicture;
        string userBio;
        bool edited = false;
        public ProfilePage()
        {
            InitializeComponent();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
            con.Open();

            com.Connection = con;
            showPicture();
            usernameDisplay.Text = $"Welcome {LoginPage.currentUser}";
        }
        private void showBio()
        {
            bioDisplay.Text = bioInput.Text;
        }
        private void showPicture()
        {
            com.CommandText = "select * from users where username='" + LoginPage.currentUser + "'";
            dr = com.ExecuteReader();
            BitmapImage myBitmapImage = new BitmapImage();

            if (dr.Read() && !dr.IsDBNull(dr.GetOrdinal("Image")))
            {
                userPicture = dr.GetValue(dr.GetOrdinal("Image")).ToString();
                userBio = dr.GetValue(dr.GetOrdinal("Bio")).ToString();

                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(userPicture);

                myBitmapImage.DecodePixelHeight = 200;
                myBitmapImage.EndInit();

                profilePicture.Source = myBitmapImage;
                bioDisplay.Text = userBio;
            }
            else
            {

                BitmapImage defaultImage = new BitmapImage(new Uri("C:\\Users\\kuba0\\Downloads\\panda\\defaultUser.png"))
                {
                    DecodePixelHeight = 200
                };
                profilePicture.Source = defaultImage;
            }
            dr.Close();
        }
        public void addPicture(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                DefaultExt = ".png", // Required file extension 
                Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*" // Optional file extensions
            };

            bool? res = fileDialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                string imageData = fileDialog.FileName;
                com.CommandText = $"update users set Image = '{imageData}' where username='{LoginPage.currentUser}'";
                com.ExecuteNonQuery();
                showPicture();
            }
        }

        public void updateBio(object sender, RoutedEventArgs e)
        {
            if (!edited)
            {
                bioInput.Text = bioDisplay.Text;
                bioDisplay.Visibility = Visibility.Collapsed;
                bioInput.Visibility = Visibility.Visible;
                edited = true;
            }
            else
            {
          
                com.CommandText = $"update users set bio = '{bioInput.Text}' where username='{LoginPage.currentUser}'";
                com.ExecuteNonQuery();
                showBio();
                bioDisplay.Visibility = Visibility.Visible;
                bioInput.Visibility = Visibility.Collapsed;
                edited = false;
            }

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
