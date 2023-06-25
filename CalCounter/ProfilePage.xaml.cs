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
        public string userPicture;
        public ProfilePage()
        {
            InitializeComponent();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
            con.Open();

            com.Connection = con;
            showPicture();
            usernameDisplay.Text = LoginPage.currentUser;
        }
        private void showPicture()
        {
            com.CommandText = "select * from users where username='" + LoginPage.currentUser + "'";
            dr = com.ExecuteReader();
            BitmapImage myBitmapImage = new BitmapImage();

            if (dr.Read() && !dr.IsDBNull(dr.GetOrdinal("Image")))
            {
                userPicture = dr.GetValue(dr.GetOrdinal("Image")).ToString();

                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(userPicture);

                myBitmapImage.DecodePixelHeight = 200;
                myBitmapImage.EndInit();

                profilePicture.Source = myBitmapImage;
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
