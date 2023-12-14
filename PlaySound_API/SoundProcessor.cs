using System.Collections.ObjectModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace PlaySound_API;

public class SoundProcessor
{
    private WaveOutEvent outputDevice;
    private AudioFileReader audioFile;
    private MMDeviceEnumerator deviceEnumerator;
    public ObservableCollection<string> audioDevices;
    public int soundsActivated = 1;
    public int deviceNumberIndex;
    
    public SoundProcessor()
    {
        deviceEnumerator = new MMDeviceEnumerator();
        audioDevices = new ObservableCollection<string>();
        GetAudioDevices();
    }

    public void UpdateDeviceNumber(int deviceNumber)
    {
        deviceNumberIndex = deviceNumber;
    }

    private void GetAudioDevices()
    {
        var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        foreach (var device in devices)
        {
            audioDevices.Add(device.FriendlyName);
        }
    }
    public void PlaySound(SoundFile soundFile)
    {
        if (outputDevice == null)
        {
            outputDevice = new WaveOutEvent();
            outputDevice.DeviceNumber = deviceNumberIndex;
            outputDevice.PlaybackStopped += OnPlaybackStopped;
        }
        if (audioFile == null)
        {
            audioFile = new AudioFileReader(soundFile.soundFilePath);
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

    public void PauseAudio()
    {
        if (outputDevice?.PlaybackState == PlaybackState.Playing) { outputDevice.Pause(); }
        else if (outputDevice?.PlaybackState == PlaybackState.Paused) { outputDevice.Play(); }
    }

    public void StopAudio()
    {
        outputDevice?.Stop();
    }
}