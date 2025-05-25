using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace Rizzify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double volume = 0.5;
        double lastvolume = 0.5;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btMaxMin_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void slSound_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slSound.Value <= 0)
            {
                volume = 0;
                btMute.IsChecked = true;
            }
            else
            {
                volume = slSound.Value;
                lastvolume = slSound.Value;
                btMute.IsChecked = false;
            }
        }

        private void btMute_Click(object sender, RoutedEventArgs e)
        {
            if (btMute.IsChecked == true)
            {
                volume = slSound.Value;
                slSound.Value = 0;
            }
            else
            {
                slSound.Value = lastvolume;
            }
        }
    }
}
