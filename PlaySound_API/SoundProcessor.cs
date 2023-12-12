using System.Collections.ObjectModel;

namespace PlaySound_API;

public class SoundProcessor
{
    private WaveOutEvent outputDevice;
    private AudioFileReader audioFile;
    private MMDeviceEnumerator deviceEnumerator;
    private ObservableCollection<string> audioDevices;
    private int soundsActivated = 1;
    
    public SoundProcessor()
    {
        deviceEnumerator = new MMDeviceEnumerator();
        audioDevices = new ObservableCollection<string>();
        PopulateAudioDevices();
    }
    
    private void GetAudioDevices()
    {
        var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        foreach (var device in devices)
        {
            //audioDevices.Add(device.FriendlyName);
            inputList.Items.Add(device.FriendlyName);
        }
        inputList.SelectedIndex = 0;
    }
    public void PlaySound(SoundFile soundFile)
    {
        if (outputDevice == null)
        {
            outputDevice = new WaveOutEvent();
            outputDevice.DeviceNumber = inputList.SelectedIndex;
            outputDevice.PlaybackStopped += OnPlaybackStopped;
        }
        if (audioFile == null)
        {
            audioFile = new AudioFileReader(soundFile.soundFilePath);
            outputDevice.Init(audioFile);
            outputDevice.Play();
        }
    }
}