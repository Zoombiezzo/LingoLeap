namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    public interface IAudioSourceHandler
    {
        bool IsPlaying { get; }
        string ClipName { get; }
        AudioFlags Flags { get; }
        public void SetSettings(AudioSettings settings);
        public void SetDefaultSettings();
    }
}