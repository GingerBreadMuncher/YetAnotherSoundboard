using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media;
using PlaySound_API;
using System.Collections.Generic;
using System.Linq;

namespace YetAnotherSoundboard
{
    public partial class MainWindow : Window
    {
        private SoundProcessor soundProcessor;
        private SoundLibrary soundLibrary;
        List<Button> buttons = new List<Button>();
        private string soundTitle = "";

        public MainWindow()
        {
            InitializeComponent();
            soundProcessor = new SoundProcessor();
            soundLibrary = new SoundLibrary();
            inputList.SelectedIndex = 0;
            inputList.ItemsSource = soundProcessor.audioDevices;
            inputList.SelectionChanged += InputList_SelectionChanged;
            volumeSlider.ValueChanged += OnVolumeChanged;
        }

        private void OnVolumeChanged(object sender, EventArgs e)
        {
            soundProcessor.UpdateVolume((float)volumeSlider.Value);
        }

        private void InputList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            soundProcessor.UpdateDeviceNumber(inputList.SelectedIndex);
        }

        private void DragBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void AddSound_Click(object sender, RoutedEventArgs e)
        {
            int buttonColumn = Grid.GetColumn(AddSoundButton) + 1;
            int buttonRow = Grid.GetRow(AddSoundButton) + 1;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select audio file...";
            fileDialog.DefaultExt = ".mp3";
            fileDialog.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav";
            bool? fileDiagSelected = fileDialog.ShowDialog();
            if (fileDiagSelected == true)
            {
                SoundFile soundFile = new SoundFile("fileName", fileDialog.FileName);
                soundLibrary.AddSoundFile(soundFile);
                #region Generating a button
                Button soundButton = new Button();
                buttons.Add(soundButton);

                soundButton.Width = 150;
                soundButton.Height = 150;
                soundButton.BorderThickness = new Thickness(0);
                soundButton.Background = new SolidColorBrush(Colors.Transparent);
                soundTitle = "Sound" + soundProcessor.soundsActivated;
                soundButton.Name = soundTitle;

                StackPanel soundPanel = new StackPanel();
                Image soundImage = new Image();
                soundImage.Source = new BitmapImage(new Uri("/Images/play_icon_white.png", UriKind.Relative));
                soundImage.Height = 100;
                soundImage.Width = 150;
                TextBlock soundText = new TextBlock();
                if (buttons.Count == 1) { soundProcessor.soundsActivated = 1; }
                soundText.Text = soundTitle;
                soundText.FontSize = 30;
                soundText.FontWeight = FontWeights.SemiBold;
                soundText.Foreground = Brushes.White;
                soundText.HorizontalAlignment = HorizontalAlignment.Center;
                soundText.VerticalAlignment = VerticalAlignment.Bottom;
                TextBox renameBox = new TextBox();
                renameBox.Visibility = Visibility.Hidden;
                renameBox.Background = new SolidColorBrush(Colors.Transparent);
                renameBox.Foreground = Brushes.White;
                renameBox.Text = soundTitle;
                renameBox.TextAlignment = TextAlignment.Center;
                renameBox.VerticalContentAlignment = VerticalAlignment.Center;
                renameBox.HorizontalAlignment = HorizontalAlignment.Center;
                renameBox.Margin = new Thickness(0, -50, 0, 0);
                renameBox.Height = 40;
                renameBox.FontSize = 25;
                renameBox.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        soundTitle = renameBox.Text;
                        soundText.Text = soundTitle;
                        renameBox.Visibility = Visibility.Hidden;
                        soundText.Visibility = Visibility.Visible;
                    }

                    double baseFontSize = 30;

                    if (soundText.Text.Length > 8)
                    {
                        soundText.FontSize = baseFontSize - renameBox.Text.Length + 4;
                        double newMargin = renameBox.Text.Length;
                        soundImage.Margin = new Thickness (0, -newMargin, 0, 0);
                    }
                    else { soundText.FontSize = baseFontSize; }
                };
                renameBox.MaxLength = 16;
                renameBox.VerticalAlignment = VerticalAlignment.Center;
                soundPanel.Children.Add(soundImage);
                soundPanel.Children.Add(soundText);
                soundPanel.Children.Add(renameBox);
                soundButton.Content = soundPanel;
                soundButton.Click += (sender, e) => soundProcessor.PlaySound(soundFile);
                #endregion

                ContextMenu contextMenu = new ContextMenu();
                MenuItem deleteMenuItem = new MenuItem();
                deleteMenuItem.Header = "Delete";
                deleteMenuItem.Click += (sender, e) =>
                {
                    soundLibrary.RemoveSoundFile(soundFile);
                    grid.Children.Remove(soundButton);
                    buttons.Remove(soundButton);
                    if (AddSoundButton.Visibility == Visibility.Hidden) { AddSoundButton.Visibility = Visibility.Visible;}
                    int soundButtonRow = Grid.GetRow(soundButton);
                    if (!buttons.Any(b => Grid.GetRow(b) == soundButtonRow))
                    {
                        foreach (Button btn in buttons.Where(b => Grid.GetRow(b) > soundButtonRow))
                        {
                            Grid.SetRow(btn, Grid.GetRow(btn) - 1);
                        }

                        if (Grid.GetRow(AddSoundButton) > 2)
                        {
                            Grid.SetRow(AddSoundButton, Grid.GetRow(AddSoundButton) - 1);
                        }
                    }
                    UpdateButtonPositions();
                };
                contextMenu.Items.Add(deleteMenuItem);
                soundButton.ContextMenu = contextMenu;

                
                MenuItem renameSound = new MenuItem();
                renameSound.Header = "Rename";
                renameSound.Click += (sender, e) =>
                {
                    soundText.Visibility = Visibility.Hidden;
                    renameBox.Visibility = Visibility.Visible;
                };
                contextMenu.Items.Add(renameSound);

                Grid.SetRow(soundButton, buttonRow - 1);
                Grid.SetColumn(soundButton, buttonColumn - 1);
                grid.Children.Add(soundButton);

                soundProcessor.soundsActivated += 1;
                Grid.SetColumn(AddSoundButton, buttonColumn);

                if (buttonColumn >= grid.ColumnDefinitions.Count)
                {
                    Grid.SetRow(AddSoundButton, buttonRow);
                    Grid.SetColumn(AddSoundButton, 0);
                }
                if (grid.RowDefinitions.Count <= buttonRow && buttonColumn >= grid.ColumnDefinitions.Count)
                {
                    AddSoundButton.Visibility = Visibility.Hidden;
                }


                void UpdateButtonPositions()
                {
                    int columnCount = grid.ColumnDefinitions.Count;

                    for (int i = 0; i < buttons.Count; i++)
                    {
                        Grid.SetColumn(buttons[i], i % columnCount);
                        Grid.SetRow(buttons[i], 2 + i / columnCount);
                    }

                    Grid.SetColumn(AddSoundButton, buttons.Count % columnCount);
                    Grid.SetRow(AddSoundButton, 2 + buttons.Count / columnCount);
                }
            }
        }
        private void StopAudio_Click(object sender, RoutedEventArgs e) { soundProcessor.StopAudio(); }

        private void PauseAudio_Click(object sender, RoutedEventArgs e) { soundProcessor.PauseAudio(); }
    }
}
