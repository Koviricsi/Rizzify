using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Rizzify
{
    /// <summary>  
    /// Interaction logic for MainWindow.xaml  
    /// </summary>  
    public partial class MainWindow : Window
    {
        double lastVolume = 0.5;
        CheckBox lastCheckbox = null;
        List<string> musicFiles = new List<string>();
        List<string> favoriteFiles = new List<string>();
        int nowPlaying = -1;
        MediaPlayer mediaPlayer = new MediaPlayer();
        bool isDragging = false;
        List<string> history = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            
            mediaPlayer.Volume = lastVolume;

            if (!Directory.Exists("Music"))
            {
                Directory.CreateDirectory("Music");
            }

            LoadFiles();
            LoadFavorites();
            ShowMusicFiles();

            csuszka.ApplyTemplate();
            var track = csuszka.Template.FindName("PART_Track", csuszka) as Track;
            track.Thumb.DragStarted += (s, e) =>
            {
                isDragging = true;
                cbPlay.IsChecked = false;
                lastCheckbox.IsChecked = false;
                mediaPlayer.Pause();
            };
            track.Thumb.DragCompleted += (s, e) =>
            {
                isDragging = false;
                if (mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    mediaPlayer.Position = TimeSpan.FromSeconds(csuszka.Value);
                }
            };

            CompositionTarget.Rendering += (s, e) =>
            {
                if (!isDragging && mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    csuszka.Value = mediaPlayer.Position.TotalSeconds;
                }
            };
        }

        private void LoadFiles()
        {
            string[] mp3Files = Directory.GetFiles("Music", "*.mp3");
            string[] wavFiles = Directory.GetFiles("Music", "*.wav");
            musicFiles.AddRange(mp3Files);
            musicFiles.AddRange(wavFiles);
        }

        private void LoadFavorites()
        {
            if (File.Exists("favorites.txt"))
            {
                string[] lines = File.ReadAllLines("favorites.txt");
                favoriteFiles.AddRange(lines);
            }
            else
            {
                File.Create("favorites.txt").Close();
            }
        }

        private void ShowMusicFiles()
        {
            int i = 0;
            foreach (string file in musicFiles)
            {
                string fileName = System.IO.Path.GetFileName(file).Split('.')[0];
                string duration = "Ismeretlen";
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri(file, UriKind.RelativeOrAbsolute));
                player.MediaOpened += (s, e) =>
                {
                    duration = player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

                    Grid grid = new Grid();

                    Grid.SetRow(grid, i);
                    grid.Style = (Style)FindResource("playlist_grid");

                    ColumnDefinition nameCol = new ColumnDefinition();
                    nameCol.Width = new GridLength(1, GridUnitType.Star);

                    for (int j = 0; j < 4; j++)
                    {
                        ColumnDefinition col = new ColumnDefinition();
                        col.Width = new GridLength(1, GridUnitType.Auto);
                        grid.ColumnDefinitions.Add(col);
                    }

                    grid.ColumnDefinitions.Insert(1, nameCol);

                    //Zene ikon
                    Viewbox iconVb = new Viewbox();
                    iconVb.Margin = new Thickness(6);
                    iconVb.Stretch = Stretch.Uniform;
                    Grid.SetColumn(iconVb, 0);
                    iconVb.Child = new System.Windows.Shapes.Path
                    {
                        Data = Geometry.Parse("M36.513,0L13.838,3.688C11.351,4.09,9.336,6.461,9.336,8.979v16.916\r\nc-3.674,0-6.651,2.979-6.651,6.652s2.978,6.651,6.651,6.651c3.674,0,6.652-2.979,6.652-6.651c0-0.331-0.002-0.649-0.049-0.966\r\nV12.544l13.923-2.566v11.546c-3.674,0-6.652,2.978-6.652,6.65c0,3.675,2.978,6.653,6.652,6.653s6.651-2.979,6.651-6.653\r\nc0-0.327,0-0.948,0-0.948S36.513,0,36.513,0z"),
                        Fill = (Brush)FindResource("gray4")
                    };
                    grid.Children.Add(iconVb);

                    //Zene név
                    Viewbox nameVb = new Viewbox();
                    nameVb.Margin = new Thickness(12);
                    nameVb.Stretch = Stretch.Uniform;
                    Grid.SetColumn(nameVb, 1);
                    nameVb.Child = new TextBlock
                    {
                        Text = fileName,
                        Foreground = (Brush)FindResource("white"),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    grid.Children.Add(nameVb);

                    //Időtartam
                    Viewbox durationVb = new Viewbox();
                    durationVb.Margin = new Thickness(12);
                    durationVb.Stretch = Stretch.Uniform;
                    Grid.SetColumn(durationVb, 2);
                    durationVb.Child = new TextBlock
                    {
                        Text = duration,
                        Foreground = (Brush)FindResource("white"),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    grid.Children.Add(durationVb);

                    //Favorite gomb
                    Viewbox favoriteVb = new Viewbox();
                    favoriteVb.Margin = new Thickness(6);
                    favoriteVb.Stretch = Stretch.Uniform;
                    Grid.SetColumn(favoriteVb, 3);
                    CheckBox favoriteCb = new CheckBox
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Style = (Style)FindResource("favorite_bt"),
                        IsChecked = favoriteFiles.Contains(file)
                    };

                    favoriteCb.Click += (a, b) =>
                    {
                        if (favoriteCb.IsChecked == true)
                        {
                            if (!favoriteFiles.Contains(file))
                            {
                                favoriteFiles.Add(file);
                                File.WriteAllLines("favorites.txt", favoriteFiles);
                            }
                        }
                        else
                        {
                            if (favoriteFiles.Contains(file))
                            {
                                favoriteFiles.Remove(file);
                                File.WriteAllLines("favorites.txt", favoriteFiles);
                            }
                        }
                    };

                    favoriteVb.Child = favoriteCb;
                    grid.Children.Add(favoriteVb);

                    //Lejátszás gomb
                    Viewbox playVb = new Viewbox();
                    playVb.Margin = new Thickness(6);
                    playVb.Stretch = Stretch.Uniform;
                    Grid.SetColumn(playVb, 4);
                    CheckBox playCb = new CheckBox
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Style = (Style)FindResource("control_cb_pp"),
                        IsChecked = false
                    };

                    playCb.Tag = i;

                    playCb.Click += (a, b) =>
                    {
                        if (playCb.IsChecked == true)
                        {
                            if (nowPlaying == (int)playCb.Tag)
                            {
                                mediaPlayer.Play();
                            }
                            else
                            {
                                if (nowPlaying != -1)
                                {
                                    lastCheckbox.IsChecked = false;
                                }
                                mediaPlayer.Stop();
                                mediaPlayer.Open(new Uri(file, UriKind.RelativeOrAbsolute));
                                mediaPlayer.MediaOpened += (c, d) =>
                                {
                                    csuszka.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                                    csuszka.Value = 0;
                                };
                                mediaPlayer.Play();
                                nowPlaying = (int)playCb.Tag;
                                lastCheckbox = (CheckBox)a;
                            }
                            cbPlay.IsChecked = true;
                            if (history.Count > 0)
                            {
                                if (history.Last() != file)
                                {
                                    history.Add(file);
                                }
                            }
                        }
                        else 
                        {
                            mediaPlayer.Pause();
                            cbPlay.IsChecked = false;
                        }
                        tbMusicTitle.Text = fileName; 
                    };


                    playVb.Child = playCb;
                    grid.Children.Add(playVb);

                    content_grid.Children.Add(grid);

                    i++;
                };
            }
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
                mediaPlayer.Volume = 0;
                btMute.IsChecked = true;
            }
            else
            {
                mediaPlayer.Volume = slSound.Value;
                lastVolume = slSound.Value;
                btMute.IsChecked = false;
            }
        }

        private void btMute_Click(object sender, RoutedEventArgs e)
        {
            if (btMute.IsChecked == true)
            {
                mediaPlayer.Volume = slSound.Value;
                slSound.Value = 0;
            }
            else
            {
                slSound.Value = lastVolume;
            }
        }

        private void btFavorite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbPlay_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && nowPlaying != -1)
            {
                if (cb.IsChecked == true)
                {
                    mediaPlayer.Play();
                    lastCheckbox.IsChecked = true;
                }
                else
                {
                    mediaPlayer.Pause();
                    lastCheckbox.IsChecked = false;
                }
            }
        }

        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            cbPlay.IsChecked = false;
            lastCheckbox.IsChecked = false;
            mediaPlayer.Stop();
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
        {
            cbPlay.IsChecked = false;
            lastCheckbox.IsChecked = false;
            mediaPlayer.Stop();
        }
    }
}
