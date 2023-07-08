using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace CalCounter
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage
    {
        private readonly SqlConnection con = new SqlConnection();
        private readonly SqlCommand com = new SqlCommand();
        private SqlDataReader dr;
        private string userPicture;
        private string userBio;
        private bool edited = false;
        private bool requestShow = false;

        public ProfilePage()
        {
            InitializeComponent();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
            con.Open();

            com.Connection = con;
            showPicture();
            showFriends();
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

        public void setColour(string colour, TextBlock item)
        {
            var converter = new BrushConverter();
            var brush = (Brush)converter.ConvertFromString(colour);
            item.Foreground = brush;
        }

        private void addPicture(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
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

        private void updateBio(object sender, RoutedEventArgs e)
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

        private void showFriends()
        {
            com.CommandText = $"select * from friends where user_one='{LoginPage.currentUser}' or user_two='{LoginPage.currentUser}'";
            dr = com.ExecuteReader();

            friendsList.Items.Clear();

            if (dr.HasRows)
            {
               while (dr.Read())
                {
                    ListBoxItem itm = new ListBoxItem();
                    string displayName = dr.GetString(dr.GetOrdinal("user_two"));
                    if (displayName != LoginPage.currentUser)
                    {
                        itm.Content = displayName;
                    }
                    else
                    {
                        displayName = dr.GetString(dr.GetOrdinal("user_one"));
                        itm.Content = displayName;
                    }
                    friendsList.Items.Add(itm);
                }
            }

            dr.Close();
        }

        private void showRequests()
        {
            com.CommandText = $"select * from requests where receiver='{LoginPage.currentUser}'";
            dr = com.ExecuteReader();

            requestList.Items.Clear();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = $"{dr.GetValue(dr.GetOrdinal("sender"))}";
                    requestList.Items.Add(itm);
                }
            }
            dr.Close();
        }

        private void toggleRequests(object sender, RoutedEventArgs e)
        {
            if (!requestShow)
            {
                friendsList.Visibility = Visibility.Collapsed;
                friendActions.Visibility = Visibility.Collapsed;

                showRequests();

                requestList.Visibility = Visibility.Visible;
                requestActions.Visibility = Visibility.Visible;
                requestShow = true;
            }
            else
            {
                friendsList.Visibility = Visibility.Visible;
                friendActions.Visibility = Visibility.Visible;
                requestList.Visibility = Visibility.Collapsed;
                requestActions.Visibility = Visibility.Collapsed;
                requestShow = false;
            }
        }

        private void addFriend(object sender, RoutedEventArgs e)
        {
            // If input is empty
            if (friendInput.Text == null)
            {
                setColour("#FFFF0000", error);
                error.Text = "Empty field.";
                return;
            }

            // If trying to send to yourself
            if (friendInput.Text == LoginPage.currentUser)
            {
                setColour("#FFFF0000", error);
                error.Text = "Unable to send request to yourself.";
                return;
            }

            com.CommandText = $"select * from users where username='{friendInput.Text}'";
            dr = com.ExecuteReader();
            // If username exists
            if (dr.Read())
            {
                dr.Close();
                com.CommandText = $"select * from friends where (user_one = '{LoginPage.currentUser}' and user_two = '{friendInput.Text}') or (user_one = '{friendInput.Text}' and user_two = '{LoginPage.currentUser}')";
                dr = com.ExecuteReader();
                // If friendship exists
                if (dr.Read())
                {
                    setColour("#FFFF0000", error);
                    error.Text = "Already friends.";
                    dr.Close();
                }                
                else
                {
                    dr.Close();
                    com.CommandText = $"select * from requests where sender = '{LoginPage.currentUser}' and receiver = '{friendInput.Text}'";
                    dr = com.ExecuteReader();
                    // If request exists
                    if (dr.Read())
                    {
                        setColour("#FFFF0000", error);
                        error.Text = "Request already sent.";
                        dr.Close();
                    }
                    else
                    {
                        dr.Close();
                        com.CommandText = $"select * from requests where sender = '{friendInput.Text}' and receiver = '{LoginPage.currentUser}'";
                        dr = com.ExecuteReader();
                        // If target sent request
                        if (dr.Read())
                        {
                            dr.Close();
                            setColour("#FF00FF00", error);
                            error.Text = "Mutual friend request, added friend.";
                            com.CommandText = $"delete from requests where sender='{friendInput.Text}' and receiver='{LoginPage.currentUser}'";
                            com.ExecuteNonQuery();
                            com.CommandText = $"insert into friends(user_one, user_two) values('{LoginPage.currentUser}', '{friendInput.Text}')";
                            com.ExecuteNonQuery();
                            showRequests();
                            showFriends();
                        }
                        else
                        {
                            dr.Close();
                            setColour("#FF00FF00", error);
                            error.Text = "Sent friend request.";
                            com.CommandText = $"insert into requests(sender, receiver) values('{LoginPage.currentUser}', '{friendInput.Text}')";
                            com.ExecuteNonQuery();
                            showRequests();
                        }
                    }            
                }             
            }
            else
            {
                dr.Close();
                setColour("#FFFF0000", error);
                error.Text = "User not found.";
            }
        }

        private void removeFriend(object sender, RoutedEventArgs e)
        {
            if ((friendsList.SelectedItem as ListBoxItem) != null) 
            {
                if ((friendsList.SelectedItem as ListBoxItem).Content != null)
                {
                    string selectedFriend = (friendsList.SelectedItem as ListBoxItem).Content.ToString();
                    com.CommandText = $"delete from friends where (user_one='{LoginPage.currentUser}' and user_two='{selectedFriend}') or (user_one = '{selectedFriend}' and user_two = '{LoginPage.currentUser}')";
                    com.ExecuteNonQuery();

                    setColour("#FF00FF00", error);
                    error.Text = "Friend removed.";
                    showFriends();
                }
                else
                {
                    error.Text = "Select friend.";
                }
            }
        }

        private void accept(object sender, RoutedEventArgs e)
        {
            if ((requestList.SelectedItem as ListBoxItem) != null)
            {
                if ((requestList.SelectedItem as ListBoxItem).Content.ToString() != null)
                {
                    string selectedSender = (requestList.SelectedItem as ListBoxItem).Content.ToString();

                    com.CommandText = $"delete from requests where sender='{selectedSender}' and receiver='{LoginPage.currentUser}'";
                    com.ExecuteNonQuery();
                    com.CommandText = $"insert into friends(user_one, user_two) values('{LoginPage.currentUser}', '{selectedSender}')";
                    com.ExecuteNonQuery();
                    requestResult.Text = "Request accepted.";
                    showRequests();
                    showFriends();
                }
                else
                {
                    requestResult.Text = "Select request.";
                }
            }
        }

        private void decline(object sender, RoutedEventArgs e)
        {
            if ((requestList.SelectedItem as ListBoxItem) != null)
            {
                if ((requestList.SelectedItem as ListBoxItem).Content.ToString() != null)
                {
                    string selectedSender = (requestList.SelectedItem as ListBoxItem).Content.ToString();

                    com.CommandText = $"delete from requests where sender='{selectedSender}' and receiver='{LoginPage.currentUser}'";
                    com.ExecuteNonQuery();
                    requestResult.Text = "Request declined.";
                    showRequests();
                }
                else
                {
                    requestResult.Text = "Select request.";
                }
            }
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
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
