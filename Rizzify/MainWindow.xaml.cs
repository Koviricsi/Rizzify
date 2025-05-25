using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rizzify
{
    public partial class MainWindow : Window
    {
        private List<string> mp3List = new List<string>();
        private bool isFavoritesVisible;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btMaxMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void btFavorites_Click(object sender, RoutedEventArgs e)
        {
            isFavoritesVisible = !isFavoritesVisible;

            
            btFavorites.Content = isFavoritesVisible ? "♥" : "♡";

            if (isFavoritesVisible)
            {
                try
                {
                    string folderPath = @"C:\Zene";

                    if (Directory.Exists(folderPath))
                    {
                        string[] files = Directory.GetFiles(folderPath, "*.mp3");
                        mp3List = files.Select(f => Path.GetFileName(f)).ToList();

                        MusicListBox.ItemsSource = null;
                        MusicListBox.ItemsSource = mp3List;
                        MusicListBox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show($"A mappa nem található: {folderPath}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt a fájlok betöltésekor:\n{ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MusicListBox.Visibility = Visibility.Collapsed;
            }
        }


    }
}

