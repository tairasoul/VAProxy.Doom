using UnityEngine;

namespace DoomInProxy.Doom.Unity;

internal static class DoomAudioSource
{
    public static AudioSource Create(string name)
    {
        var gameObject = new GameObject($"DoomAudio_{name}")
        {
            hideFlags = HideFlags.HideAndDontSave,
        };

        var audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.spatialBlend = 0;

        return audio;
    }
}