﻿using System.Windows;
using System.Windows.Input;

namespace CalCounter
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void goProfile(object sender, RoutedEventArgs e)
        {

        }
        private void goCalendar(object sender, RoutedEventArgs e)
        {

        }
        private void goStats(object sender, RoutedEventArgs e)
        {
        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
