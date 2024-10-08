using System;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    [Flags]
    public enum AudioFlags
    {
        None = 0,
        AudioPaused = 1,
        AudioManualPaused = 1 << 1,
        AudioRunning = 1 << 2,
        AudioPlaying = 1 << 3,
        AudioStopping = 1 << 4,
        AudioStopped = 1 << 5
    }
}