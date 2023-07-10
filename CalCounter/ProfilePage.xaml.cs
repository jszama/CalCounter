using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
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
        private readonly SqlCommand com2 = new SqlCommand();
        private SqlDataReader dr;
        private SqlDataReader dr2;
        private string userPicture;
        private string userBio;
        private bool edited = false;
        private bool requestShow = false;

        public ProfilePage()
        {
            InitializeComponent();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
            con.Open();

            com2.Connection = con;
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
                if (bioInput.Text.Length < 45)
                {
                    com.CommandText = $"update users set bio = '{bioInput.Text}' where username='{LoginPage.currentUser}'";
                    com.ExecuteNonQuery();
                    showBio();
                    bioDisplay.Visibility = Visibility.Visible;
                    bioInput.Visibility = Visibility.Collapsed;
                    edited = false;
                }   
                else
                {
                    bioInput.Text = "Bio must be less than 45 characters.";
                } 
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
                friendTitle.Visibility = Visibility.Collapsed;

                showRequests();

                requestTitle.Visibility = Visibility.Visible;
                requestList.Visibility = Visibility.Visible;
                requestActions.Visibility = Visibility.Visible;
                requestShow = true;
            }
            else
            {
                friendsList.Visibility = Visibility.Visible;
                friendActions.Visibility = Visibility.Visible;
                friendTitle.Visibility = Visibility.Visible;

                requestTitle.Visibility = Visibility.Collapsed;
                requestList.Visibility = Visibility.Collapsed;
                requestActions.Visibility = Visibility.Collapsed;
                requestShow = false;
            }
        }

        private void viewFriend(object sender, RoutedEventArgs e)
        {
            if ((friendsList.SelectedItem as ListBoxItem) != null)
            {
                if ((friendsList.SelectedItem as ListBoxItem).Content != null)
                {
                    BitmapImage myBitmapImage = new BitmapImage();
                    string selectedFriend = (friendsList.SelectedItem as ListBoxItem).Content.ToString();
                    com.CommandText = $"select * from Users where username='{selectedFriend}'";
                    dr = com.ExecuteReader();
                    Int32 publicType = 0;
                    if (dr.Read() && Convert.ToInt32(dr.GetValue(dr.GetOrdinal("Privacy"))) == publicType)
                    {
                        friendUsernameDisplay.Text = dr.GetString(dr.GetOrdinal("Username"));

                        myBitmapImage.BeginInit();
                        myBitmapImage.UriSource = new Uri(dr.GetValue(dr.GetOrdinal("Image")).ToString());

                        myBitmapImage.DecodePixelHeight = 200;
                        myBitmapImage.EndInit();

                        com2.CommandText = $"SELECT AVG(calories) AS average_calories FROM (SELECT TOP 30 calories FROM Calendar WHERE Username = '{selectedFriend}' ORDER BY CONVERT(datetime, date, 103) DESC) AS latest_records;";
                        dr2 = com2.ExecuteReader();

                        if (dr2.Read() && !dr2.IsDBNull(dr2.GetOrdinal("average_calories")))
                        {
                            double monthlyAvg = Convert.ToDouble(dr2.GetValue(0));
                            monthly.Text = $"Monthly Average:{monthlyAvg}";
                        }
                        else
                        {
                            monthly.Text = "Monthly Average: N/A";
                        }

                        dr2.Close();
                        com2.CommandText = $"SELECT AVG(calories) AS average_calories FROM (SELECT TOP 7 calories FROM Calendar WHERE Username = '{selectedFriend}' ORDER BY CONVERT(datetime, date, 103) DESC) AS latest_records;";
                        dr2 = com2.ExecuteReader();

                        if (dr2.Read() && !dr2.IsDBNull(0))
                        {
                            double weeklyAvg = Convert.ToDouble(dr2.GetValue(0));
                            weekly.Text = $"Weekly Average:{weeklyAvg}";
                        }
                        else
                        {
                            weekly.Text = "Weekly Average: N/A";
                        }
                        dr2.Close();


                        friendProfilePicture.Source = myBitmapImage;
                        friendBioDisplay.Text = dr.GetString(dr.GetOrdinal("Bio"));

                        userInfo.Visibility = Visibility.Collapsed;
                        friendInfo.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // Privat profile, dont show bio or statistics
                        friendUsernameDisplay.Text = dr.GetString(dr.GetOrdinal("Username"));

                        myBitmapImage.BeginInit();
                        myBitmapImage.UriSource = new Uri(dr.GetValue(dr.GetOrdinal("Image")).ToString());

                        myBitmapImage.DecodePixelHeight = 200;
                        myBitmapImage.EndInit();

                        friendProfilePicture.Source = myBitmapImage;

                        friendBioDisplay.Text = dr.GetString(dr.GetOrdinal("Bio"));
                        friendBioDisplay.Text = "Profile is set to private.";
                        weekly.Text = "Weekly Average: Private";
                        monthly.Text = "Monthly Average: Private";

                        userInfo.Visibility = Visibility.Collapsed;
                        friendInfo.Visibility = Visibility.Visible;
                    }
                    dr.Close();
                }
                else
                {
                    error.Text = "Select friend.";
                }
            }   
        }

        private void goBack(object sender, RoutedEventArgs e)
        {
            userInfo.Visibility = Visibility.Visible;
            friendInfo.Visibility = Visibility.Collapsed;
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
