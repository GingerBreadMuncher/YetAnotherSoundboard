﻿using Microsoft.Win32;
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
using PlaySound_API;

namespace YetAnotherSoundboard
{
    public partial class MainWindow : Window
    {
        private SoundProcessor soundProcessor;
        private SoundLibrary soundLibrary;
        
        public MainWindow()
        {
            InitializeComponent();
            soundProcessor = new SoundProcessor();
            soundLibrary = new SoundLibrary();
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
                soundButton.Click += soundProcessor.PlaySound(soundFile);

                Grid.SetRow(soundButton, buttonRow-1);
                Grid.SetColumn(soundButton, buttonColumn-1);
                grid.Children.Add(soundButton);

                soundFilePaths.Add(fileDialog.FileName, soundsActivated);
                soundsActivated += 1;
                Grid.SetColumn(AddSoundButton, buttonColumn);
                #endregion
            }
            else { MessageBox.Show("File was not selected!", "Error", MessageBoxButton.OK); }

            if (buttonColumn >= grid.ColumnDefinitions.Count)
            {
                Grid.SetRow(AddSoundButton, buttonRow);
                Grid.SetColumn(AddSoundButton, 0);
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
