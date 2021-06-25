using Unity.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void Play2(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
        
    }

    public void Play3(string soundWantToPlay, string soundStopIfPlaysAlready)
    {
        Sound s1 = Array.Find(sounds, sound => sound.name == soundWantToPlay);
        Sound s2 = Array.Find(sounds, sound => sound.name == soundStopIfPlaysAlready);
        bool res = false;

        /*if (!s2.source.isPlaying)
        {
            res = true;
        }
        if (!s1.source.isPlaying && res)
        {*/
            s2.source.Stop();
            s1.source.Play();
        //}  
    }
}
