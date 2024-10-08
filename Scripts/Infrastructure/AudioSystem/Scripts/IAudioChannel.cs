using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    public interface IAudioChannel
    {
        string Id { get; }
        int FreeSourceCount { get; }
        IReadOnlyList<AudioSourceHandler> PlayingSources { get; }
        AudioType AudioType { get; }
        AudioMixerGroup AudioMixer { get; }
        IReadOnlyList<AudioSource> Sources { get; }
        void Initialize();
        void Uninitialize();
        void SetVolume(float volume);
        float GetVolume();
        IAudioSourceHandler Play(AudioClip clip, AudioSettings settings);
        IAudioSourceHandler PlayOneShot(AudioClip clip, AudioSettings settings);
        void Stop(AudioClip clip);
        void Pause(AudioClip clip);
        void PauseAll();
        void Unpause(AudioClip clip);
        void UnpauseAll();
        bool IsPlaying(AudioClip clip);
        bool IsPlayingAny();
    }
}