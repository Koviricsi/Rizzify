mainwindow.xaml.cs :

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

xml

<Window x:Class="Rizzify.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:Rizzify"
        mc:Ignorable="d"
        MinHeight="600" MinWidth="400"
        Height="600" Width="400"
        WindowStyle="None">
    <Grid>

        <Grid.RowDefinitions>
            <RowDefinition Height="8*"/>
            <RowDefinition Height="92*"/>
        </Grid.RowDefinitions>

        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>

       
        <Rectangle Grid.ColumnSpan="3" Style="{StaticResource header_bg}" MouseDown="Rectangle_MouseDown"/>
        <Image Grid.Column="0" Source="/Images/rizzify_logo.png" HorizontalAlignment="Left"/>

        <Viewbox Stretch="Uniform" Grid.Row="0" Grid.Column="1" HorizontalAlignment="Right">
            <Grid Width="260" Height="Auto">
                <StackPanel Orientation="Horizontal" Grid.Column="1" HorizontalAlignment="Right">
                    <Button x:Name="btFavorites"  Click="btFavorites_Click" Content="♡" FontSize="14"/>
                    <Button x:Name="btMaxMin" Style="{StaticResource minmax_bt}" Click="btMaxMin_Click"/>
                    <Button x:Name="btClose" Style="{StaticResource close_bt}" Click="btClose_Click"/>
                </StackPanel>

            </Grid>
        </Viewbox>

        
        <StackPanel Grid.Row="1" Grid.ColumnSpan="2" Margin="10">
            <ListBox Name="MusicListBox" Height="200" Visibility="Collapsed"/>
        </StackPanel>

    </Grid>
</Window>
