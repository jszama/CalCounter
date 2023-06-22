using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
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

            // Create Image Element

            // Create source
            BitmapImage myBitmapImage = new BitmapImage();

            if (dr.Read() && !dr.IsDBNull(dr.GetOrdinal("Image")))
            {
                userPicture = dr.GetValue(dr.GetOrdinal("Image")).ToString();


                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(userPicture);

                // To save significant application memory, set the DecodePixelWidth or
                // DecodePixelHeight of the BitmapImage value of the image source to the desired
                // height or width of the rendered image. If you don't do this, the application will
                // cache the image as though it were rendered as its normal size rather than just
                // the size that is displayed.
                // Note: In order to preserve aspect ratio, set DecodePixelWidth
                // or DecodePixelHeight but not both.
                myBitmapImage.DecodePixelHeight = 200;
                myBitmapImage.EndInit();
                //set image source
                profilePicture.Source = myBitmapImage;
            }
            else
            {

                BitmapImage defaultImage = new BitmapImage(new Uri("C:\\Users\\kuba0\\Downloads\\panda\\defaultUser.png"));
                defaultImage.DecodePixelHeight = 200;
                profilePicture.Source = defaultImage;
            }
            dr.Close();
        }
        public void addPicture(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".png"; // Required file extension 
            fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"; // Optional file extensions

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
