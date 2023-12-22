using System.Collections.ObjectModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace PlaySound_API;

public class SoundProcessor
{
    private WaveOutEvent outputDevice;
    private WaveOutEvent listenerDevice;
    private AudioFileReader audioFile;
    private MMDeviceEnumerator deviceEnumerator;
    public MMDevice defaultDevice;
    public ObservableCollection<string> audioDevices;
    public bool listenerActivated = false;
    public int soundsActivated = 1;
    public int deviceNumberIndex;
    public float volumeValue = 1;

    public SoundProcessor()
    {
        deviceEnumerator = new MMDeviceEnumerator();
        audioDevices = new ObservableCollection<string>();
        GetAudioDevices();
    }

    public void UpdateVolume(float soundVolume)
    {
        volumeValue = soundVolume;
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
        defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    }

    public void PlaySound(SoundFile soundFile)
    {
        if (outputDevice == null)
        {
            outputDevice = new WaveOutEvent();
            outputDevice.DeviceNumber = deviceNumberIndex;
            outputDevice.PlaybackStopped += OnPlaybackStopped;
        }
        if (listenerDevice == null)
        {
            listenerDevice = new WaveOutEvent();
            listenerDevice.PlaybackStopped += OnPlaybackStopped;
        }
        if (audioFile == null)
        {
            audioFile = new AudioFileReader(soundFile.soundFilePath);
            audioFile.Volume = volumeValue;
            outputDevice.Init(audioFile);
            outputDevice.Play();
            if (listenerActivated)
            {
                var listenerAudioFile = new AudioFileReader(soundFile.soundFilePath);
                listenerDevice.Init(listenerAudioFile);
                listenerDevice.Play();
            }
        }
    }

    private void OnPlaybackStopped(object sender, StoppedEventArgs e)
    {
        outputDevice?.Dispose();
        outputDevice = null;
        listenerDevice?.Dispose();
        listenerDevice = null;
        audioFile?.Dispose();
        audioFile = null;
    }

    public void PauseAudio()
    {
        if(outputDevice == null) return;
        switch (outputDevice.PlaybackState)
        {
            case PlaybackState.Playing:
            {
                outputDevice.Pause();
                listenerDevice.Pause();
                break;
            }
            case PlaybackState.Paused:
            {
                outputDevice.Play();
                if (listenerActivated & listenerDevice == null) {listenerDevice.Play();}
                break;
            }
        }
    }

    public void StopAudio()
    {
        outputDevice?.Stop();
    }
}