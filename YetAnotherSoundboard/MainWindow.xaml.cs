using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media;

namespace YetAnotherSoundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            deviceEnumerator = new MMDeviceEnumerator();
            audioDevices = new ObservableCollection<string>();
            soundFilePaths = new Dictionary<string, int>();
            PopulateAudioDevices();
        }

        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private MMDeviceEnumerator deviceEnumerator;
        private ObservableCollection<string> audioDevices;
        private Dictionary<string, int> soundFilePaths;
        private string soundFilePath = "";
        private int soundsActivated = 1;

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
            int buttonColumn = Grid.GetColumn(AddSoundButton);
            int buttonRow = Grid.GetRow(AddSoundButton);
            buttonColumn++; buttonRow++;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select audio file...";
            fileDialog.DefaultExt = ".mp3";
            fileDialog.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav";
            bool? fileDiagSelected = fileDialog.ShowDialog();
            if (fileDiagSelected == true)
            {
                #region Generating a button
                Button soundButton = new Button();

                soundButton.Width = 150;
                soundButton.Height = 150;
                soundButton.BorderThickness = new Thickness(0);
                soundButton.Background = new SolidColorBrush(Colors.Transparent);
                soundButton.Name = "Sound" + soundsActivated;

                StackPanel soundPanel = new StackPanel();
                Image soundImage = new Image();
                soundImage.Source = new BitmapImage(new Uri("/Images/play_icon_white.png", UriKind.Relative));
                soundImage.Height = 100;
                soundImage.Width = 150;
                TextBlock soundText = new TextBlock();
                soundText.Text = "Sound " + soundsActivated;
                soundText.FontSize = 30;
                soundText.FontWeight = FontWeights.SemiBold;
                soundText.Foreground = Brushes.White;
                soundText.HorizontalAlignment = HorizontalAlignment.Center;
                soundText.VerticalAlignment = VerticalAlignment.Bottom;
                soundPanel.Children.Add(soundImage);
                soundPanel.Children.Add(soundText);
                soundButton.Content = soundPanel;
                soundButton.Click += Sound1_Click;

                Grid.SetRow(soundButton, buttonRow-1);
                Grid.SetColumn(soundButton, buttonColumn-1);
                grid.Children.Add(soundButton);

                soundFilePaths.Add(fileDialog.FileName, soundsActivated);
                soundsActivated += 1;
                Grid.SetColumn(AddSoundButton, buttonColumn);
                #endregion
                soundFilePath = fileDialog.FileName;
            }
            else { MessageBox.Show("File was not selected!", "Error", MessageBoxButton.OK); }

            if (buttonColumn >= grid.ColumnDefinitions.Count)
            {
                Grid.SetRow(AddSoundButton, buttonRow);
                Grid.SetColumn(AddSoundButton, 0);
            }


        }

        private void PopulateAudioDevices()
        {
            var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            foreach (var device in devices)
            {
                //audioDevices.Add(device.FriendlyName);
                inputList.Items.Add(device.FriendlyName);
            }
            inputList.SelectedIndex = 0;
        }


        private void Sound1_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.DeviceNumber = inputList.SelectedIndex;
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (audioFile == null)
            {
                audioFile = new AudioFileReader(soundFilePath);
                outputDevice.Init(audioFile);
                outputDevice.Play();
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;
        }

        private void StopAudio_Click(object sender, RoutedEventArgs e)
        {
            outputDevice.Stop();
        }

        private void PauseAudio_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice.PlaybackState == PlaybackState.Playing) { outputDevice.Pause(); }
            else if (outputDevice.PlaybackState == PlaybackState.Paused) { outputDevice.Play(); }
        }
    }
}
