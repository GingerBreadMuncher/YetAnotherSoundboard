namespace PlaySound_API;

public class SoundFile
{
    public string soundName { get; private set; }
    public string soundFilePath { get; private set; }
    
    public SoundFile(string _soundName, string _soundFilePath)
    {
        soundName = _soundName;
        soundFilePath = _soundFilePath;
    }
}