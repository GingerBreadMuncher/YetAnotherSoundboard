namespace PlaySound_API;

public class SoundLibrary
{
    public List<SoundFile> soundFiles { get; private set; }
    
    public SoundLibrary()
    {
        soundFiles = new List<SoundFile>();
    }
    
    public void AddSoundFile(SoundFile soundFile)
    {
        soundFiles.Add(soundFile);
    }

    public SoundFile FindSoundFile(string fileName) => soundFiles.Find(x => x.soundName == fileName);
}