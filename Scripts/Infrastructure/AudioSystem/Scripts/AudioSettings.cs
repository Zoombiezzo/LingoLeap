namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    public struct AudioSettings
    {
        public float Volume;
        public float Pitch;
        public float DurationResume;
        public float DurationPause;
        public float DurationPlay;
        public float DurationStop;
        public bool Loop;
        public bool CheckDuplicate;
        public static AudioSettings Default { get; } = new()
        {
            Volume = 1f,
            Pitch = 1f,
            Loop = false,
            DurationPause = 0f,
            DurationPlay = 0f,
            DurationStop = 0f,
            DurationResume = 0f,
            CheckDuplicate = false
        };
        
        public static AudioSettings DefaultWithoutDuplicate { get; } = new()
        {
            Volume = 1f,
            Pitch = 1f,
            Loop = false,
            DurationPause = 0f,
            DurationPlay = 0f,
            DurationStop = 0f,
            DurationResume = 0f,
            CheckDuplicate = true
        };
        
        public static AudioSettings Looped { get; } = new()
        {
            Volume = 1f,
            Pitch = 1f,
            Loop = true,
            DurationPause = 0f,
            DurationPlay = 0f,
            DurationStop = 0f,
            DurationResume = 0f,
            CheckDuplicate = false
        };
    }
}